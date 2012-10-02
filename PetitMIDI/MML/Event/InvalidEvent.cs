namespace PetitMIDI.MML.Event
{
    /// <summary>
    /// Represents an invalid event.
    /// </summary>
    public class InvalidEvent : MMLEvent
    {
        /// <summary>
        /// Instantiates a new instance of the <see cref="InvalidEvent"/> class.
        /// </summary>
        public InvalidEvent()
            : base(EventTag.Invalid)
        {
        }
    }
}