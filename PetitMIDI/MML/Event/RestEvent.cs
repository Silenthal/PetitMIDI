// -----------------------------------------------------------------------
// <copyright file="RestEvent.cs" company="">
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
    public class RestEvent : MMLEvent
    {
        /// <summary>
        /// Instantiates a new instance of the <see cref="RestEvent"/> class.
        /// </summary>
        public RestEvent()
            : base(EventTag.Rest)
        {
            this.NoteValue = -1;
            this.Multiplier = 1;
        }

        public int NoteValue
        {
            get;
            set;
        }

        public double Multiplier
        {
            get;
            set;
        }

        public double ActualNoteValue
        {
            get
            {
                return NoteValue / Multiplier;
            }
        }
    }
}
