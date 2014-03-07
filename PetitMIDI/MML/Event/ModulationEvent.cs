namespace PetitMIDI.MML.Event
{
	public class ModulationEvent : MMLEvent
	{
		public bool IsModulationOn;

		public ModulationEvent(bool modOn)
			: base(EventTag.Modulation)
		{
			this.IsModulationOn = modOn;
		}
	}
}