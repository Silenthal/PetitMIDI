namespace PetitMIDI.MML.Event
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class OctaveEvent : MMLEvent
    {
        private int octave;

        public int Octave
        {
            get
            {
                return this.octave;
            }
        }

        public OctaveEvent(int newOctave)
            : base(EventTag.Octave)
        {
            this.octave = newOctave;
        }
    }
}