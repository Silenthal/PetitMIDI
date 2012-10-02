namespace PetitMIDI.MML.Event
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class PanEvent : MMLEvent
    {
        private int pan;

        public PanEvent(int newPan)
            : base(EventTag.Pan)
        {
            this.pan = newPan;
        }

        public int Pan
        {
            get
            {
                return this.pan;
            }
        }
    }
}