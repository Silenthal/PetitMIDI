namespace PetitMIDI.MML.Event
{
    public class TempoEvent : MMLEvent
    {
        public int Tempo { get; }

        public TempoEvent(int newTempo)
            : base(MMLEventTag.Tempo)
        {
            Tempo = newTempo;
        }
    }
}