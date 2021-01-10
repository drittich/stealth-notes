using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Timers;
using System.Windows.Forms;

namespace StealthNotes
{
	public partial class Form1 : Form
	{
		private const string version = "v0.1";
		private InputDevices inputs;
		private Config config;

		private bool isMuted = false;
		
		private EnhancedTimer timer;
		private System.Drawing.Color defaultCellBgColor = System.Drawing.Color.White;

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
		}

		private void SetupKeyboardHook()
		{
			var keyboardHook = new LowLevelKeyboardHook();
			keyboardHook.OnKeyPressed += keyboardHook_OnKeyPressed;
			keyboardHook.HookKeyboard();
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
				row.Cells[2].Value = muted;
				row.Cells[1].Style.BackColor = muted ? System.Drawing.Color.LightPink : defaultCellBgColor;
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

		private void keyboardHook_OnKeyPressed(object sender, Keys e)
		{
			if (!isMuted)
			{
				isMuted = true;
				MuteSelectedItems();
			}

			timer.Restart();
		}

		private void OnTimedEvent(object sender, ElapsedEventArgs e)
		{
			if (isMuted)
			{
				isMuted = false;
				MuteSelectedItems(false);
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
				var selected = (bool)row.Cells[0].Value;
				if (selected)
				{
					var friendlyName = row.Cells[1].Value.ToString();
					MuteInput(friendlyName, mute);
				}
			}
		}

		private void MuteAllInputs(bool mute = true)
		{
			foreach (DataGridViewRow row in dataGridView1.Rows)
			{
				var friendlyName = row.Cells[1].Value.ToString();
				MuteInput(friendlyName, mute);
			}
		}

		private void MuteInput(string friendlyName, bool mute = true)
		{
			var muting = mute ? "Muting" : "Unmuting";
			Log($"{muting} {friendlyName}");
			inputs.MuteInputByFriendlyName(friendlyName, mute);
			var row = GetRowByName(friendlyName);
			row.Cells[2].Value = mute;
			row.Cells[1].Style.BackColor = mute ? System.Drawing.Color.LightPink : defaultCellBgColor;
		}

		private void btnUnmuteAll_Click(object sender, EventArgs e)
		{
			MuteAllInputs(false);
		}

		// Ref: https://stackoverflow.com/questions/34090190/c-sharp-datagridview-checkbox-checked-event
		private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.ColumnIndex == 0 && e.RowIndex >= 0)
				dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);

			var friendlyName = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
			if ((bool)dataGridView1.CurrentCell.Value == true)
				config.DevicesToMute.Add(friendlyName);
			else
				config.DevicesToMute.Remove(friendlyName);

			config.Save();
		}

		private DataGridViewRow GetRowByName(string name)
		{
			foreach (DataGridViewRow row in dataGridView1.Rows)
				if (row.Cells[1].Value.ToString() == name)
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
	}
}
