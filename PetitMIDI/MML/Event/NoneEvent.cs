namespace PetitMIDI.MML.Event
{
    /// <summary>
    /// Represents no event.
    /// </summary>
    public class NoneEvent : MMLEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NoneEvent"/> class.
        /// </summary>
        public NoneEvent()
            : base(EventTag.None)
        { }
    }
}