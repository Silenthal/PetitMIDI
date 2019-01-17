namespace PetitMIDI.MML.Event
{
    public class PanEvent : MMLEvent
    {
        private int pan;

        public int Pan
        {
            get
            {
                return this.pan;
            }
        }

        public PanEvent(int newPan)
            : base(EventTag.Pan)
        {
            this.pan = newPan;
        }
    }
}