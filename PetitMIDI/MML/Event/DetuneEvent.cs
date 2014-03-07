namespace PetitMIDI.MML.Event
{
	public class DetuneEvent : MMLEvent
	{
		public int Detune;

		public DetuneEvent(int detune)
			: base(EventTag.Detune)
		{
			Detune = detune;
		}
	}
}