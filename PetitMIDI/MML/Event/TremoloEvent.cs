namespace PetitMIDI.MML.Event
{
    public class TremoloEvent : MMLEvent
    {
        public int Depth;
        public int Range;
        public int Speed;
        public int Delay;

        public TremoloEvent(int depth, int range, int speed, int delay)
            : base(MMLEventTag.Tremolo)
        {
            Depth = depth;
            Range = range;
            Speed = speed;
            Delay = delay;
        }
    }
}