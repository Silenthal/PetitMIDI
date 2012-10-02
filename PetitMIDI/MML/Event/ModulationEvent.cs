// -----------------------------------------------------------------------
// <copyright file="ModulationEvent.cs" company="">
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
    public class ModulationEvent : MMLEvent
    {
        public ModulationEvent(bool modOn) : base(EventTag.Modulation)
        {
            this.IsModulationOn = modOn;
        }

        public bool IsModulationOn
        {
            get;
            set;
        }
    }
}
