namespace PetitMIDI.MML.Event
{
    public class InstrumentEvent : MMLEvent
    {
        private int instrument;

        public int Instrument
        {
            get
            {
                return this.instrument;
            }
        }

        public InstrumentEvent(int newInstrument)
            : base(EventTag.Instrument)
        {
            this.instrument = newInstrument;
        }
    }
}