namespace PetitMIDI.MML.Event
{
    public class PanEvent : MMLEvent
    {
        public int Pan { get; }

        public PanEvent(int newPan)
            : base(MMLEventTag.Pan)
        {
            Pan = newPan;
        }
    }
}