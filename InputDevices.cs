using System.Collections.Generic;
using System.Linq;

using NAudio.CoreAudioApi;

namespace StealthNotes
{
	internal class InputDevices
	{
		public InputDevices()
		{
			ActiveInputs = GetActiveInputs();
		}

		public Dictionary<string, MMDevice> ActiveInputs { get; }

		public List<string> DeviceNames
		{
			get
			{
				return ActiveInputs.Keys.OrderBy(k => k).ToList();
			}
		}

		public void MuteInputByName(string name, bool mute = true)
		{
			var device = ActiveInputs[name];

			if (device != null && device.AudioEndpointVolume.Mute != mute)
				device.AudioEndpointVolume.Mute = mute;
		}

		private Dictionary<string, MMDevice> GetActiveInputs()
		{
			var enumerator = new MMDeviceEnumerator();
			var enabledDevices = enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active)
				.Where(device => device.State == DeviceState.Active)
				.ToDictionary(device => device.FriendlyName, device => device);
			return enabledDevices;
		}

		public void RemoveActiveInput(string name)
		{
			ActiveInputs.Remove(name);
		}

		public MMDevice GetDeviceByName(string name)
		{
			return ActiveInputs[name];
		}
	}
}
