namespace PetitMIDI.MML.Event
{
    public class TempoEvent : MMLEvent
    {
        private int tempo;

        public int Tempo
        {
            get
            {
                return this.tempo;
            }
        }

        public TempoEvent(int newTempo)
            : base(EventTag.Tempo)
        {
            this.tempo = newTempo;
        }
    }
}