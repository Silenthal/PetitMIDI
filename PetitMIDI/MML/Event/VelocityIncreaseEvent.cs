namespace PetitMIDI.MML.Event
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class VelocityIncreaseEvent : MMLEvent
    {
        public int IncrementAmount
        {
            get;
            set;
        }

        public VelocityIncreaseEvent(int incrementAmount)
            : base(EventTag.VelocityIncrease)
        {
            this.IncrementAmount = incrementAmount;
        }
    }
}