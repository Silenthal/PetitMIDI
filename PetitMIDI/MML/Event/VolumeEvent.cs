namespace PetitMIDI.MML.Event
{
    public class VolumeEvent : MMLEvent
    {
        public VolumeEvent(int newVolume)
            : base(EventTag.Volume)
        {
            this.Volume = newVolume;
        }

        public int Volume;
    }
}