namespace PetitMIDI.MML.Event
{
    public class VolumeEvent : MMLEvent
    {
        public VolumeEvent(int newVolume)
            : base(MMLEventTag.Volume)
        {
            this.Volume = newVolume;
        }

        public int Volume;
    }
}