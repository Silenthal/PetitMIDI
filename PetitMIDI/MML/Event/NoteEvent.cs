namespace PetitMIDI.MML.Event
{
    /// <summary>
    /// Represents a note event.
    /// </summary>
    public class NoteEvent : MMLEvent
    {
        public int BaseNote;
        public int NoteValue;
        public double Multiplier;

        public double ActualNoteValue
        {
            get
            {
                return NoteValue / Multiplier;
            }
        }

        /// <summary>
        /// Instantiates a new instance of the <see cref="NoteEvent"/> class.
        /// </summary>
        /// <param name="note">The value of the note to play.</param>
        public NoteEvent(int note)
            : base(EventTag.Note)
        {
            this.BaseNote = note & 0x7F;
            this.NoteValue = -1;
            this.Multiplier = 1;
        }
    }
}