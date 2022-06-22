using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Timers;
using System.Windows.Forms;

using Gma.System.MouseKeyHook;

using Microsoft.Win32;


namespace StealthNotes
{
	public partial class Form1 : Form
	{
		[DllImport("User32.dll")]
		public static extern IntPtr GetDC(IntPtr hwnd);
		[DllImport("User32.dll")]
		public static extern void ReleaseDC(IntPtr hwnd, IntPtr dc);

		private const string version = "v0.7";
		private InputDevices inputs;
		private Config config;

		private bool isMuted = false;

		private EnhancedTimer timer;
		private Color defaultCellBgColor = Color.White;
		private bool reloadDevicesOnNextUnmute = false;

		private Rectangle alertRectangle = new Rectangle(0, 0, 240, 240);

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

			SetupUnMuteTimer(config.MuteIntervalMs);

			tbMuteInterval.Value = config.MuteIntervalMs;
			SetNewInterval(config.MuteIntervalMs);

			InitGrid();
			SetupKeyboardHook();
			chkIgnoreModifierKeys.Checked = config.IgnoreModifierKeys;

			SetRefreshDevicesButton();

			chkStartMinimized.Checked = config.StartMinimized;
			if (chkStartMinimized.Checked)
				WindowState = FormWindowState.Minimized;
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

			// don't mute mic/show visual alert if we're not currently using any inputs
			if (!IsInUseByApplication())
				return;

			ShowAlert();

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

		private void ShowAlert()
		{
			IntPtr desktopPtr = GetDC(IntPtr.Zero);
			Graphics g = Graphics.FromHdc(desktopPtr);

			SolidBrush b = new SolidBrush(Color.DeepPink);
			g.FillRectangle(b, alertRectangle);

			g.Dispose();
			ReleaseDC(IntPtr.Zero, desktopPtr);
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
			timer = new EnhancedTimer(config.MuteIntervalMs * 1000);
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

				// hide visual alert
				InvalidateRect(IntPtr.Zero, alertRectangle, true);

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

		private bool IsInUseByApplication()
		{
			foreach (DataGridViewRow row in dgvInputs.Rows)
			{
				var selected = (bool)row.Cells[(int)DeviceGridColumns.Selected].Value;
				if (selected)
				{
					var name = row.Cells[(int)DeviceGridColumns.Name].Value.ToString();
					if (inputs.IsActiveInApplication(name, "Teams"))
						return true;
				}
			}
			return false;
		}

		private void MuteAllInputs(bool mute = true)
		{
			if (!IsInUseByApplication())
				return;

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
			int intervalms = (sender as TrackBar).Value;

			SetNewInterval(intervalms);

			config.MuteIntervalMs = intervalms;
			config.Save();
		}

		private void SetNewInterval(int intervalMilliseconds)
		{
			lblUnmuteDuration.Text = $"Unmute after {intervalMilliseconds} ms";
			timer.Interval = intervalMilliseconds;
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
			notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;

			if (FormWindowState.Minimized == WindowState)
			{
				notifyIcon1.Visible = true;
				FormBorderStyle = FormBorderStyle.FixedToolWindow;
				//notifyIcon1.ShowBalloonTip(500);
				ShowInTaskbar = false;
			}
			else if (FormWindowState.Normal == WindowState)
			{
				notifyIcon1.Visible = false;
				FormBorderStyle = FormBorderStyle.FixedSingle;
			}
		}

		private void chkIgnoreAltTab_Click(object sender, EventArgs e)
		{
			config.IgnoreModifierKeys = (sender as CheckBox).Checked;
			config.Save();
		}

		private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
		{
			WindowState = FormWindowState.Normal;
			notifyIcon1.Visible = false;
			ShowInTaskbar = true;
		}

		// screen lock event handler
		private void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
		{
			if (e.Reason == SessionSwitchReason.SessionUnlock)
			{
				ReloadDevices();
			}
		}

		[DllImport("user32.dll")]
		static extern bool InvalidateRect(IntPtr hWnd, RECT lpRect, bool bErase);

		[StructLayout(LayoutKind.Sequential)]
		public struct RECT
		{
			public int Left, Top, Right, Bottom;

			public RECT(int left, int top, int right, int bottom)
			{
				Left = left;
				Top = top;
				Right = right;
				Bottom = bottom;
			}

			public RECT(Rectangle r) : this(r.Left, r.Top, r.Right, r.Bottom) { }

			public int X
			{
				get { return Left; }
				set { Right -= (Left - value); Left = value; }
			}

			public int Y
			{
				get { return Top; }
				set { Bottom -= (Top - value); Top = value; }
			}

			public int Height
			{
				get { return Bottom - Top; }
				set { Bottom = value + Top; }
			}

			public int Width
			{
				get { return Right - Left; }
				set { Right = value + Left; }
			}

			public Point Location
			{
				get { return new Point(Left, Top); }
				set { X = value.X; Y = value.Y; }
			}

			public Size Size
			{
				get { return new Size(Width, Height); }
				set { Width = value.Width; Height = value.Height; }
			}

			public static implicit operator Rectangle(RECT r)
			{
				return new Rectangle(r.Left, r.Top, r.Width, r.Height);
			}

			public static implicit operator RECT(Rectangle r)
			{
				return new RECT(r);
			}

			public static bool operator ==(RECT r1, RECT r2)
			{
				return r1.Equals(r2);
			}

			public static bool operator !=(RECT r1, RECT r2)
			{
				return !r1.Equals(r2);
			}

			public bool Equals(RECT r)
			{
				return r.Left == Left && r.Top == Top && r.Right == Right && r.Bottom == Bottom;
			}

			public override bool Equals(object obj)
			{
				if (obj is RECT)
					return Equals((RECT)obj);
				else if (obj is Rectangle)
					return Equals(new RECT((Rectangle)obj));
				return false;
			}

			public override int GetHashCode()
			{
				return ((Rectangle)this).GetHashCode();
			}

			public override string ToString()
			{
				return string.Format(System.Globalization.CultureInfo.CurrentCulture, "{{Left={0},Top={1},Right={2},Bottom={3}}}", Left, Top, Right, Bottom);
			}
		}

		private void chkStartMinimized_CheckedChanged(object sender, EventArgs e)
		{
			var chk = sender as CheckBox;
			config.StartMinimized = chk.Checked;
			config.Save();
		}
	}
}
