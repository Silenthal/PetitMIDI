﻿namespace PetitMIDI.MML.Event
{
    public class InvalidEvent : MMLEvent
    {
        public InvalidEvent()
            : base(MMLEventTag.Invalid)
        {
        }
    }
}