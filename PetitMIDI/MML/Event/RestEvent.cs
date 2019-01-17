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
            : base(MMLEventTag.Rest)
        {
            NoteValue = -1;
            Multiplier = 1;
        }
    }
}