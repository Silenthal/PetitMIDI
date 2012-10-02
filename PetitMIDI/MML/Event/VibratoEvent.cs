// -----------------------------------------------------------------------
// <copyright file="VibratoEvent.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace PetitMIDI.MML.Event
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class VibratoEvent : MMLEvent
    {
        public VibratoEvent(int depth, int range, int speed, int delay) : base(EventTag.Vibrato)
        {
            this.Depth = depth;
            this.Range = range;
            this.Speed = speed;
            this.Delay = delay;
        }
        public int Depth
        {
            get;
            set;
        }

        public int Range
        {
            get;
            set;
        }

        public int Speed
        {
            get;
            set;
        }

        public int Delay
        {
            get;
            set;
        }
    }
}
