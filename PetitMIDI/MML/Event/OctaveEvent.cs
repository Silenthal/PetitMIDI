namespace PetitMIDI.MML.Event
{
    public class OctaveEvent : MMLEvent
    {
        public int Octave { get; }

        public OctaveEvent(int newOctave)
            : base(MMLEventTag.Octave)
        {
            Octave = newOctave;
        }
    }
}