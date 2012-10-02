namespace PetitMIDI.MML.Event
{
    /// <summary>
    /// Represents a note event.
    /// </summary>
    public class NoteEvent : MMLEvent
    {
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

        public int BaseNote
        {
            get;
            set;
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
