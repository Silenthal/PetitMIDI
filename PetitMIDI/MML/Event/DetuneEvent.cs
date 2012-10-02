namespace PetitMIDI.MML.Event
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class DetuneEvent : MMLEvent
    {
        public DetuneEvent(int detune)
            : base(EventTag.Detune)
        {
            this.Detune = detune;
        }

        public int Detune
        {
            get;
            set;
        }
    }
}