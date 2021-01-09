using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Timers;
using System.Windows.Forms;

namespace MuteMicWhenTyping
{
	public partial class Form1 : Form
	{
		private InputDevices inputs;
		private Config config;

		private bool isMuted = false;
		private TimeSpan muteInterval = TimeSpan.FromSeconds(2);

		private System.Timers.Timer timer;

		public Form1()
		{
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			Log($"using config from {Path.Combine(Application.LocalUserAppDataPath, "config.json")}");

			config = new Config().Load();

			// enumerate inputs
			inputs = new InputDevices();
			foreach (var input in inputs.ActiveInputs.OrderBy(i => i.FriendlyName))
			{
				var idx = checkedListBox1.Items.Add(input.FriendlyName);
				if (config.DevicesToMute.Contains(input.FriendlyName))
					checkedListBox1.SetItemChecked(idx, true);
			}
			
			checkedListBox1.ItemCheck += checkedListBox1_ItemCheck;

			// unmute all inputs
			UnmuteAllItems();

			// set up unmute timer
			timer = new System.Timers.Timer(muteInterval.TotalMilliseconds);
			timer.Elapsed += OnTimedEvent;
			timer.AutoReset = false;
			timer.Enabled = true;

			var kbh = new LowLevelKeyboardHook();
			kbh.OnKeyPressed += kbh_OnKeyPressed;
			kbh.HookKeyboard();
		}

		private void kbh_OnKeyPressed(object sender, Keys e)
		{
			if (!isMuted)
			{
				Log($"muting selected items");
				MuteSelectedItems();
			}

			isMuted = true;
			timer.Stop();
			timer.Start();
		}

		private void OnTimedEvent(object sender, ElapsedEventArgs e)
		{
			if (isMuted)
			{
				MuteSelectedItems(false);
				isMuted = false;
			}
		}

		public void Log(string msg)
		{
			if (InvokeRequired)
			{
				BeginInvoke(new Action<string>(Log), new object[] { msg });
				return;
			}
			textBox1.Text += $"{msg}\r\n";
		}

		private void MuteSelectedItems(bool mute = true)
		{
			foreach (var item in checkedListBox1.CheckedItems)
			{
				var muting = mute ? "Muting" : "Unmuting";
				Log($"{muting} {item}");
				inputs.MuteInputByFriendlyName(item.ToString(), mute);


				//var idx = checkedListBox1.Items.Add(item.ToString());
				//checkedListBox1.Items[idx].
			}
		}

		private void UnmuteAllItems()
		{
			foreach (var item in checkedListBox1.Items)
			{
				Log($"Unmuting {item}");
				inputs.MuteInputByFriendlyName(item.ToString(), false);
			}
		}

		private void btnMuteAll_Click(object sender, EventArgs e)
		{
			UnmuteAllItems();
		}

		private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
		{
			if (e.NewValue == CheckState.Checked)
				config.DevicesToMute.Add(checkedListBox1.Items[e.Index].ToString());
			else
				config.DevicesToMute.Remove(checkedListBox1.Items[e.Index].ToString());

			config.Save();
		}
	}
}
