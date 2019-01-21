namespace PetitMIDI.MML.Event
{
    /// <summary>
    /// Represents a generic MIDI event.
    /// </summary>
    public abstract class MMLEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MMLEvent"/> class.
        /// </summary>
        /// <param name="eventType">The type of event.</param>
        protected MMLEvent(MMLEventTag eventType)
        {
            Tag = eventType;
        }

        /// <summary>
        /// Gets or sets a value indicating the event type.
        /// </summary>
        public MMLEventTag Tag
        {
            get;
            protected set;
        }
    }
}