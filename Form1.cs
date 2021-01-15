using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Timers;
using System.Windows.Forms;

using Gma.System.MouseKeyHook;

namespace StealthNotes
{
	public partial class Form1 : Form
	{
		private const string version = "v0.5-beta";
		private InputDevices inputs;
		private Config config;

		private bool isMuted = false;

		private EnhancedTimer timer;
		private Color defaultCellBgColor = Color.White;
		private bool reloadDevicesOnNextUnmute = false;

		private IKeyboardMouseEvents m_GlobalHook;

		public Form1()
		{
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			Icon = Properties.Resources.MicOn;
			notifyIcon1.Icon = Properties.Resources.MicOn;

			Text = $"Stealth Notes {version}";

			config = new Config().Load();

			SetupUnMuteTimer(config.MuteInterval);

			tbMuteInterval.Value = config.MuteInterval;
			SetNewInterval(config.MuteInterval);

			InitGrid();
			SetupKeyboardHook();
			chkIgnoreModifierKeys.Checked = config.IgnoreModifierKeys;

			SetRefreshDevicesButton();
		}

		private void SetRefreshDevicesButton()
		{
			btnReloadDevices.Font = new Font("Wingdings 3", 12, FontStyle.Bold);
			btnReloadDevices.Text = char.ConvertFromUtf32(81); // or 80
			btnReloadDevices.Width = 40;
			btnReloadDevices.Height = 40;
		}

		private void SetupKeyboardHook()
		{
			m_GlobalHook = Hook.GlobalEvents();
			m_GlobalHook.KeyDown += GlobalHookKeyPress;
		}

		private static List<Keys> ignoreKeycodes = new List<Keys> { Keys.LMenu, Keys.LControlKey, Keys.LWin, Keys.RMenu, Keys.RControlKey, Keys.RWin, Keys.LShiftKey, Keys.RShiftKey };
		private static List<Keys> ignoreModifiers = new List<Keys> { Keys.Control, Keys.Alt, Keys.Shift };

		private void GlobalHookKeyPress(object sender, KeyEventArgs e)
		{
			if (config.IgnoreModifierKeys)
			{
				if (ignoreKeycodes.Any(kc => e.KeyCode.HasFlag(kc)))
					return;
				if (ignoreModifiers.Any(m => e.Modifiers.HasFlag(m)))
					return;
			}

			if (!isMuted)
			{
				isMuted = true;
				MuteSelectedItems();
				notifyIcon1.Icon = Properties.Resources.MicOff;
				if (InvokeRequired)
					Invoke(new MethodInvoker(delegate { Icon = Properties.Resources.MicOff; }));
				else
					Icon = Properties.Resources.MicOff;
			}

			timer.Restart();
		}

		private void InitGrid()
		{
			inputs = new InputDevices();

			PopulateGrid();
		}

		private void PopulateGrid()
		{
			foreach (var deviceName in inputs.DeviceNames)
			{
				var input = inputs.GetInputByName(deviceName);
				var selected = config.DevicesToMute.Contains(input.FriendlyName);
				var name = input.FriendlyName;
				var muted = input.AudioEndpointVolume.Mute;
				var rowIdx = dgvInputs.Rows.Add(selected, name, muted);

				var row = dgvInputs.Rows[rowIdx];
				row.Cells[(int)DeviceGridColumns.Muted].Value = muted;
				row.Cells[(int)DeviceGridColumns.Name].Style.BackColor = muted ? Color.LightPink : defaultCellBgColor;
			}
		}

		private void SetupUnMuteTimer(int intervalSeconds)
		{
			timer = new EnhancedTimer(config.MuteInterval * 1000);
			timer.Elapsed += OnTimedEvent;
			timer.AutoReset = false;
			timer.Enabled = true;
			timer.Interval = intervalSeconds * 1000;
		}

		private void OnTimedEvent(object sender, ElapsedEventArgs e)
		{
			if (isMuted)
			{
				isMuted = false;
				MuteSelectedItems(false);
				notifyIcon1.Icon = Properties.Resources.MicOn;

				if (InvokeRequired)
					Invoke(new MethodInvoker(delegate { Icon = Properties.Resources.MicOn; }));
				else
					Icon = Properties.Resources.MicOn;

				if (reloadDevicesOnNextUnmute)
				{
					reloadDevicesOnNextUnmute = false;
					ReloadDevices();
				}
			}
		}

		private void MuteSelectedItems(bool mute = true)
		{
			foreach (DataGridViewRow row in dgvInputs.Rows)
			{
				var selected = (bool)row.Cells[(int)DeviceGridColumns.Selected].Value;
				if (selected)
				{
					var name = row.Cells[(int)DeviceGridColumns.Name].Value.ToString();
					MuteInput(name, mute);
				}
			}
		}

		private void MuteAllInputs(bool mute = true)
		{
			foreach (DataGridViewRow row in dgvInputs.Rows)
			{
				var name = row.Cells[(int)DeviceGridColumns.Name].Value.ToString();
				MuteInput(name, mute);
			}
		}

		private void MuteInput(string name, bool mute = true)
		{
			try
			{
				inputs.MuteInputByName(name, mute);
				var row = GetRowByName(name);
				row.Cells[(int)DeviceGridColumns.Muted].Value = mute;
				row.Cells[(int)DeviceGridColumns.Name].Style.BackColor = mute ? Color.LightPink : defaultCellBgColor;
			}
			catch (System.Runtime.InteropServices.COMException)
			{
				reloadDevicesOnNextUnmute = true;
				inputs.RemoveActiveInput(name);
			}
		}

		private void btnUnmuteAll_Click(object sender, EventArgs e)
		{
			MuteAllInputs(false);
		}

		// Ref: https://stackoverflow.com/questions/34090190/c-sharp-datagridview-checkbox-checked-event
		private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{
			// ignore header
			if (e.RowIndex == -1)
				return;

			// ignore name cell
			if (e.ColumnIndex != (int)DeviceGridColumns.Selected && e.ColumnIndex != (int)DeviceGridColumns.Muted)
				return;

			dgvInputs.CommitEdit(DataGridViewDataErrorContexts.Commit);

			var name = dgvInputs.Rows[e.RowIndex].Cells[(int)DeviceGridColumns.Name].Value.ToString();
			var isChecked = (bool)dgvInputs.CurrentCell.Value == true;

			if (e.ColumnIndex == (int)DeviceGridColumns.Selected)
			{
				if (isChecked)
					config.DevicesToMute.Add(name);
				else
					config.DevicesToMute.Remove(name);

				config.Save();
			}
			else if (e.ColumnIndex == (int)DeviceGridColumns.Muted)
			{
				MuteInput(name, isChecked);
			}
		}

		private DataGridViewRow GetRowByName(string name)
		{
			foreach (DataGridViewRow row in dgvInputs.Rows)
				if (row.Cells[(int)DeviceGridColumns.Name].Value.ToString() == name)
					return row;

			return null;
		}

		private void dataGridView1_SelectionChanged(object sender, EventArgs e)
		{
			dgvInputs.ClearSelection();
		}

		private void btnReload_Click(object sender, EventArgs e)
		{
			ReloadDevices();
		}

		private void ReloadDevices()
		{
			inputs = new InputDevices();
			dgvInputs.Rows.Clear();
			PopulateGrid();
		}

		private void trackBar1_Scroll(object sender, EventArgs e)
		{
			config.Save();
		}

		private void trackBar1_ValueChanged(object sender, EventArgs e)
		{
			int intervalSeconds = (sender as TrackBar).Value;

			SetNewInterval(intervalSeconds);

			config.MuteInterval = intervalSeconds;
			config.Save();
		}

		private void SetNewInterval(int intervalSeconds)
		{
			lblUnmuteDuration.Text = $"Unmute after {intervalSeconds} second{(intervalSeconds == 1 ? "" : "s")}";
			timer.Interval = intervalSeconds * 1000;
		}

		private enum DeviceGridColumns
		{
			Selected, Name, Muted
		}

		private void btnReloadDevices_Click(object sender, EventArgs e)
		{
			ReloadDevices();
		}

		private void Form1_Resize(object sender, EventArgs e)
		{
			//if the form is minimized
			//hide it from the task bar
			//and show the system tray icon (represented by the NotifyIcon control)
			if (WindowState == FormWindowState.Minimized)
			{
				Hide();
				notifyIcon1.Visible = true;
			}
		}

		private void chkIgnoreAltTab_Click(object sender, EventArgs e)
		{
			config.IgnoreModifierKeys = (sender as CheckBox).Checked;
			config.Save();
		}

		private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
		{
			Show();
			WindowState = FormWindowState.Normal;
			notifyIcon1.Visible = false;
		}
	}
}
