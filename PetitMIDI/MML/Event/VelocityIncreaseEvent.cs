namespace PetitMIDI.MML.Event
{
    public class VelocityIncreaseEvent : MMLEvent
    {
        public int IncrementAmount;

        public VelocityIncreaseEvent(int incrementAmount)
            : base(MMLEventTag.VelocityIncrease)
        {
            IncrementAmount = incrementAmount;
        }
    }
}