namespace PetitMIDI.MML.Event
{
    public class LengthEvent : MMLEvent
    {
        private int length;

        public int Length
        {
            get
            {
                return this.length;
            }
        }

        public LengthEvent(int newLength)
            : base(EventTag.Length)
        {
            this.length = newLength;
        }
    }
}