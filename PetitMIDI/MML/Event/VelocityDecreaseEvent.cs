namespace PetitMIDI.MML.Event
{
    public class VelocityDecreaseEvent : MMLEvent
    {
        public int DecrementAmount;

        public VelocityDecreaseEvent(int decrementAmount)
            : base(EventTag.VelocityDecrease)
        {
            this.DecrementAmount = decrementAmount;
        }
    }
}