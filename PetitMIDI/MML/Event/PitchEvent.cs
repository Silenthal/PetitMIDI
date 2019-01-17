namespace PetitMIDI.MML.Event
{
    /// <summary>
    /// Represents a note event that specifies exact pitch.
    /// </summary>
    internal class PitchEvent : MMLEvent
    {
        public int Pitch;

        public PitchEvent(int note)
            : base(EventTag.Pitch)
        {
            this.Pitch = note & 0x7F;
        }
    }
}