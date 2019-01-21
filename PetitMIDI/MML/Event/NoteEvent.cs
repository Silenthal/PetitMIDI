namespace PetitMIDI.MML.Event
{
    /// <summary>
    /// Represents a note event.
    /// </summary>
    public class NoteEvent : MMLEvent
    {
        public int BaseNote;
        public int NoteValue;
        private double Multiplier;
        public int OctaveOffset = 0;

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
            OctaveOffset = 0;
        }

        public void IncrementBaseNote()
        {
            if (++BaseNote > 11)
            {
                BaseNote = 0;
                OctaveOffset++;
            }
        }

        public void DecrementBaseNote()
        {
            if (--BaseNote < 0)
            {
                BaseNote = 11;
                OctaveOffset--;
            }
        }

        public void IncrementMultiplier()
        {
            if (Multiplier == 1)
            {
                Multiplier = 1.5;
            }
            else
            {
                Multiplier = 2.25;
            }
        }
    }
}