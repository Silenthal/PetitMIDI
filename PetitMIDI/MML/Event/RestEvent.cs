namespace PetitMIDI.MML.Event
{
	public class RestEvent : MMLEvent
	{
		public int NoteValue;
		public double Multiplier;

		public double ActualNoteValue
		{
			get
			{
				return NoteValue / Multiplier;
			}
		}

		public RestEvent()
			: base(EventTag.Rest)
		{
			this.NoteValue = -1;
			this.Multiplier = 1;
		}
	}
}