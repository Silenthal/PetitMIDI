namespace PetitMIDI.MML.Event
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class VelocityDecreaseEvent : MMLEvent
    {
        public int DecrementAmount
        {
            get;
            set;
        }
        public VelocityDecreaseEvent(int decrementAmount)
            : base(EventTag.VelocityDecrease)
        {
            this.DecrementAmount = decrementAmount;
        }
    }
}