namespace StealthNotes
{
	public class EnhancedTimer : System.Timers.Timer
	{
		public EnhancedTimer(double interval) : base(interval)
		{
		}

		public void Restart()
		{
			Stop();
			Start();
		}
	}
}
