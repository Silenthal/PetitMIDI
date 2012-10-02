namespace PetitMIDI.MML.Event
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class VolumeEvent : MMLEvent
    {
        public VolumeEvent(int newVolume)
            : base(EventTag.Volume)
        {
            this.Volume = newVolume;
        }

        public int Volume
        {
            get;
            set;
        }
    }
}