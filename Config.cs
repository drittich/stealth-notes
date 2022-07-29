using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;

namespace StealthNotes
{
	public class Config
	{
		private string configFilePath;
		private static string configFileName = "config.json";

		public HashSet<string> DevicesToMute { get; set; }
		public int MuteIntervalMs { get; set; }
		public bool IgnoreModifierKeys { get; set; }
		public bool StartMinimized { get; set; }
		//public int RefreshDevicesIntervalMs { get; internal set; }

		public Config()
		{
			configFilePath = Path.Combine(Application.LocalUserAppDataPath, configFileName);
		}

		public Config Load()
		{
			if (!File.Exists(configFilePath))
			{
				DevicesToMute = new HashSet<string>();
				MuteIntervalMs = 20;
				Save();
			}
			else
			{
				var json = File.ReadAllText(configFilePath);
				var config = JsonSerializer.Deserialize<Config>(json);
				DevicesToMute = config.DevicesToMute;
				MuteIntervalMs = (config.MuteIntervalMs < 1 || config.MuteIntervalMs > 1000) ? 200 : config.MuteIntervalMs;
				IgnoreModifierKeys = config.IgnoreModifierKeys;
				StartMinimized = config.StartMinimized;
			}

			return this;
		}

		public void Save()
		{
			Directory.CreateDirectory(Application.LocalUserAppDataPath);

			var json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
			File.WriteAllText(configFilePath, json);
		}
	}
}
