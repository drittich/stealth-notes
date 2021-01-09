using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

using Newtonsoft.Json;

namespace MuteMicWhenTyping
{
	public class Config
	{
		public List<string> DevicesToMute { get; set; }
		private string configFilePath;
		private static string configFileName = "config.json";

		public Config()
		{
			configFilePath = GetConfigFilePath();
		}

		public Config Load()
		{
			if (!File.Exists(configFilePath))
			{
				DevicesToMute = new List<string>();
				Save();
			}
			else
			{
				var data = File.ReadAllText(GetConfigFilePath());
				var config = JsonConvert.DeserializeObject<Config>(data);
				DevicesToMute = config.DevicesToMute;
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
