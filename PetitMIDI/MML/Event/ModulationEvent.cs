namespace PetitMIDI.MML.Event
{
    public class ModulationEvent : MMLEvent
    {
        public bool IsModulationOn;

        public ModulationEvent(bool modOn)
            : base(MMLEventTag.Modulation)
        {
            IsModulationOn = modOn;
        }
    }
}