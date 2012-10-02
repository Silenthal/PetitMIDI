// -----------------------------------------------------------------------
// <copyright file="EnvelopeEvent.cs" company="">
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
    public class EnvelopeEvent : MMLEvent
    {
        public EnvelopeEvent(int attack, int decay, int sustain, int release) : base(EventTag.Envelope)
        {
            this.Attack = attack;
            this.Decay = decay;
            this.Sustain = sustain;
            this.Release = release;
        }
        public int Attack
        {
            get;
            set;
        }

        public int Decay
        {
            get;
            set;
        }

        public int Sustain
        {
            get;
            set;
        }

        public int Release
        {
            get;
            set;
        }
    }
}
