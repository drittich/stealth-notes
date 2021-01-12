using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Timers;
using System.Windows.Forms;

using Gma.System.MouseKeyHook;

namespace StealthNotes
{
	public partial class Form1 : Form
	{
		private const string version = "v0.4-beta";
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
			Text = $"Stealth Notes {version}";

			Log($"using config from {Path.Combine(Application.LocalUserAppDataPath, "config.json")}");
			config = new Config().Load();

			SetupUnMuteTimer(config.MuteInterval);

			tbMuteInterval.Value = config.MuteInterval;
			SetNewInterval(config.MuteInterval);

			InitGrid();
			SetupKeyboardHook();

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

		private void GlobalHookKeyPress(object sender, KeyEventArgs e)
		{
			if (!isMuted)
			{
				isMuted = true;
				MuteSelectedItems();
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
				var input = inputs.GetDeviceByName(deviceName);
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

				if (reloadDevicesOnNextUnmute)
				{
					reloadDevicesOnNextUnmute = false;
					ReloadDevices();
				}
			}
		}

		public void Log(string msg)
		{
			Debug.WriteLine(msg);
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
			var strMuting = mute ? "Muting" : "Unmuting";
			Log($"{strMuting} {name}");
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

		private void btnMuteAll_Click(object sender, EventArgs e)
		{
			MuteAllInputs();
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
	}
}
