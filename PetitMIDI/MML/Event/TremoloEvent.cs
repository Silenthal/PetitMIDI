namespace PetitMIDI.MML.Event
{
    public class TremoloEvent : MMLEvent
    {
        public int Depth;
        public int Range;
        public int Speed;
        public int Delay;

        public TremoloEvent(int depth, int range, int speed, int delay)
            : base(EventTag.Tremolo)
        {
            this.Depth = depth;
            this.Range = range;
            this.Speed = speed;
            this.Delay = delay;
        }
    }
}