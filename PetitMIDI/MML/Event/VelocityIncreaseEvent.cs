namespace PetitMIDI.MML.Event
{
    public class VelocityIncreaseEvent : MMLEvent
    {
        public int IncrementAmount;

        public VelocityIncreaseEvent(int incrementAmount)
            : base(EventTag.VelocityIncrease)
        {
            this.IncrementAmount = incrementAmount;
        }
    }
}