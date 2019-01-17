namespace PetitMIDI.MML.Event
{
    public class InstrumentEvent : MMLEvent
    {
        public int Instrument { get; }

        public InstrumentEvent(int newInstrument)
            : base(MMLEventTag.Instrument)
        {
            Instrument = newInstrument;
        }
    }
}