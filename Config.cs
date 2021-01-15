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
		public int MuteInterval { get; set; }
		public bool IgnoreModifierKeys { get; set; }

		public Config()
		{
			configFilePath = Path.Combine(Application.LocalUserAppDataPath, configFileName);
		}

		public Config Load()
		{
			if (!File.Exists(configFilePath))
			{
				DevicesToMute = new HashSet<string>();
				MuteInterval = 2;
				Save();
			}
			else
			{
				var json = File.ReadAllText(configFilePath);
				var config = JsonSerializer.Deserialize<Config>(json);
				DevicesToMute = config.DevicesToMute;
				MuteInterval = (config.MuteInterval < 1 || config.MuteInterval > 10) ? 2 : config.MuteInterval;
				IgnoreModifierKeys = config.IgnoreModifierKeys;
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
