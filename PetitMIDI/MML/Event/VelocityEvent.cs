namespace PetitMIDI.MML.Event
{
	public class VelocityEvent : MMLEvent
	{
		private int velocity;

		public int Velocity
		{
			get
			{
				return this.velocity;
			}
		}

		public VelocityEvent(int newVelocity)
			: base(EventTag.Velocity)
		{
			this.velocity = newVelocity;
		}
	}
}