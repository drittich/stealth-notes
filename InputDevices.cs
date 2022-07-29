using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

using NAudio.CoreAudioApi;

namespace StealthNotes
{
	internal class InputDevices
	{
		public InputDevices()
		{
			ActiveInputs = GetActiveInputs();
		}

		private Dictionary<string, MMDevice> ActiveInputs { get; }

		public List<string> DeviceNames
		{
			get
			{
				return ActiveInputs.Keys.OrderBy(k => k).ToList();
			}
		}

		/// <summary>
		/// Mutes a given input if it is in active use
		/// </summary>
		/// <param name="name"></param>
		/// <param name="mute"></param>
		public void MuteInputByName(string name, bool mute = true)
		{
			var device = GetInputByName(name);
			if (device == null)
				return;

			if (device.AudioEndpointVolume.Mute != mute)
				device.AudioEndpointVolume.Mute = mute;
		}

		/// <summary>
		/// Whether the device is currently in active use
		/// </summary>
		/// <param name="device"></param>
		/// <param name="application"></param>
		/// <returns>True if the input is currently active</returns>
		public bool IsActiveInApplication(string name, IEnumerable<string> applications)
		{
			var device = GetInputByName(name);

			if (device == null)
				return false;

			int count = device.AudioSessionManager.Sessions.Count;
			for (var i = 0; i < count; i++)
			{
				var session = device.AudioSessionManager.Sessions[i];
				var sessionIdentifier = session.GetSessionIdentifier;
				if (session.State == NAudio.CoreAudioApi.Interfaces.AudioSessionState.AudioSessionStateActive && applications.Any(a => sessionIdentifier.Contains(a)))
					return true;
			}
			return false;
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

		public MMDevice GetInputByName(string name)
		{
			var x = ActiveInputs.TryGetValue(name, out var tmp) ? tmp : null;
			int sessionCount = x.AudioSessionManager.Sessions.Count;
			for (var i = 0; i < sessionCount; i++)
			{
				var session = x.AudioSessionManager.Sessions[i];
				var id = session.GetSessionInstanceIdentifier;
				Debug.WriteLine(id);
			}

			return x;
		}
	}
}
