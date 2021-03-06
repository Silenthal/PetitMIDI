﻿namespace PetitMIDI.MML.Event
{
    public class VibratoEvent : MMLEvent
    {
        public int Depth;
        public int Range;
        public int Speed;
        public int Delay;

        public VibratoEvent(int depth, int range, int speed, int delay)
            : base(MMLEventTag.Vibrato)
        {
            Depth = depth;
            Range = range;
            Speed = speed;
            Delay = delay;
        }
    }
}