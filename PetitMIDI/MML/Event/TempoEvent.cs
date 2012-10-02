namespace PetitMIDI.MML.Event
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
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