using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Timers;
using System.Windows.Forms;

using Gma.System.MouseKeyHook;

namespace StealthNotes
{
	public partial class Form1 : Form
	{
		private const string version = "v0.1";
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

			trackBar1.Value = config.MuteInterval;
			SetupUnMuteTimer();
			InitGrid();
			SetupKeyboardHook();

			button1.Font = new Font("Wingdings 3", 12, FontStyle.Bold);
			button1.Text = Char.ConvertFromUtf32(81); // or 80
			button1.Width = 40;
			button1.Height = 40;
		}

		private void SetupKeyboardHook()
		{
			// Note: for the application hook, use the Hook.AppEvents() instead
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
			foreach (var input in inputs.ActiveInputs.OrderBy(i => i.FriendlyName))
			{
				var selected = config.DevicesToMute.Contains(input.FriendlyName);
				var name = input.FriendlyName;
				var muted = input.AudioEndpointVolume.Mute;
				var rowIdx = dataGridView1.Rows.Add(selected, name, muted);

				var row = dataGridView1.Rows[rowIdx];
				row.Cells[(int)DeviceGridColumns.Muted].Value = muted;
				row.Cells[(int)DeviceGridColumns.Name].Style.BackColor = muted ? System.Drawing.Color.LightPink : defaultCellBgColor;
			}
		}

		private void SetupUnMuteTimer()
		{
			timer = new EnhancedTimer(config.MuteInterval * 1000);
			timer.Elapsed += OnTimedEvent;
			timer.AutoReset = false;
			timer.Enabled = true;
			timer.Interval = trackBar1.Value * 1000;
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
			foreach (DataGridViewRow row in dataGridView1.Rows)
			{
				var selected = (bool)row.Cells[(int)DeviceGridColumns.Selected].Value;
				if (selected)
				{
					var friendlyName = row.Cells[(int)DeviceGridColumns.Name].Value.ToString();
					MuteInput(friendlyName, mute);
				}
			}
		}

		private void MuteAllInputs(bool mute = true)
		{
			foreach (DataGridViewRow row in dataGridView1.Rows)
			{
				var friendlyName = row.Cells[(int)DeviceGridColumns.Name].Value.ToString();
				MuteInput(friendlyName, mute);
			}
		}

		private void MuteInput(string friendlyName, bool mute = true)
		{
			var strMuting = mute ? "Muting" : "Unmuting";
			Log($"{strMuting} {friendlyName}");
			try
			{
				inputs.MuteInputByFriendlyName(friendlyName, mute);
				var row = GetRowByName(friendlyName);
				row.Cells[(int)DeviceGridColumns.Muted].Value = mute;
				row.Cells[(int)DeviceGridColumns.Name].Style.BackColor = mute ? System.Drawing.Color.LightPink : defaultCellBgColor;
			}
			catch (System.Runtime.InteropServices.COMException)
			{
				reloadDevicesOnNextUnmute = true;
				inputs.RemoveActiveInput(friendlyName);
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

			dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);

			var friendlyName = dataGridView1.Rows[e.RowIndex].Cells[(int)DeviceGridColumns.Name].Value.ToString();
			var isChecked = (bool)dataGridView1.CurrentCell.Value == true;

			if (e.ColumnIndex == (int)DeviceGridColumns.Selected)
			{
				if (isChecked)
					config.DevicesToMute.Add(friendlyName);
				else
					config.DevicesToMute.Remove(friendlyName);

				config.Save();
			}
			else if (e.ColumnIndex == (int)DeviceGridColumns.Muted)
			{
				MuteInput(friendlyName, isChecked);
			}
		}

		private DataGridViewRow GetRowByName(string name)
		{
			foreach (DataGridViewRow row in dataGridView1.Rows)
				if (row.Cells[(int)DeviceGridColumns.Name].Value.ToString() == name)
					return row;

			return null;
		}

		private void dataGridView1_SelectionChanged(object sender, EventArgs e)
		{
			dataGridView1.ClearSelection();
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
			dataGridView1.Rows.Clear();
			PopulateGrid();
		}

		private void trackBar1_Scroll(object sender, EventArgs e)
		{
			timer.Interval = trackBar1.Value * 1000;
		}

		private void trackBar1_ValueChanged(object sender, EventArgs e)
		{
			int interval = (sender as TrackBar).Value;
			lblUnmuteDuration.Text = $"Unmute after {interval} second{(interval == 1 ? "" : "s")}";

			config.MuteInterval = interval;
			config.Save();
		}

		private enum DeviceGridColumns
		{
			Selected, Name, Muted
		}

		private void button1_Click(object sender, EventArgs e)
		{
			ReloadDevices();
		}
	}
}
