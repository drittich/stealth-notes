using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

using NAudio.CoreAudioApi;

namespace MuteMicWhenTyping
{
	class InputDevices
	{
		public InputDevices()
		{
			ActiveInputs = GetActiveInputs();
		}

		public List<MMDevice> ActiveInputs { get; }

		public void MuteInputByFriendlyName(string friendlyName, bool mute = true)
		{
			var device = ActiveInputs.Where(x => x.FriendlyName == friendlyName).SingleOrDefault();

			if (device != null && device.AudioEndpointVolume.Mute != mute)
				device.AudioEndpointVolume.Mute = mute;
		}

		public bool IsDeviceMuted(string friendlyName)
		{
			var device = ActiveInputs.Where(x => x.FriendlyName == friendlyName).SingleOrDefault();

			if (device == null)
				return false;

			return device.AudioEndpointVolume.Mute;
		}

		private List<MMDevice> GetActiveInputs()
		{
			var enumerator = new MMDeviceEnumerator();
			var enabledDevices = enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active).Where(x => x.State == DeviceState.Active).ToList();
			return enabledDevices;
		}
	}
}
