namespace PetitMIDI.MML.Event
{
    public class LengthEvent : MMLEvent
    {
        public int Length { get; }

        public LengthEvent(int newLength)
            : base(MMLEventTag.Length)
        {
            Length = newLength;
        }
    }
}