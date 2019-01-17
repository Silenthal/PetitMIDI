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
            : base(MMLEventTag.Note)
        {
            BaseNote = note & 0x7F;
            NoteValue = -1;
            Multiplier = 1;
        }
    }
}