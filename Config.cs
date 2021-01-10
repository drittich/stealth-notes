using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

using Newtonsoft.Json;

namespace StealthNotes
{
	public class Config
	{
		private string configFilePath;
		private static string configFileName = "config.json";

		public HashSet<string> DevicesToMute { get; set; }
		public int MuteInterval { get; set; }

		public Config()
		{
			configFilePath = GetConfigFilePath();
		}

		public Config Load()
		{
			if (!File.Exists(configFilePath))
			{
				DevicesToMute = new HashSet<string>();
				Save();
			}
			else
			{
				var data = File.ReadAllText(GetConfigFilePath());
				var config = JsonConvert.DeserializeObject<Config>(data);
				DevicesToMute = config.DevicesToMute;
				MuteInterval = config.MuteInterval == 0 ? 5 : config.MuteInterval;
			}

			return this;
		}

		public void Save()
		{
			var data = JsonConvert.SerializeObject(this);
			File.WriteAllText(configFilePath, data);
		}

		private string GetConfigFilePath()
		{
			return Path.Combine(Application.LocalUserAppDataPath, configFileName);
		}
	}
}
