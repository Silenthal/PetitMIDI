namespace PetitMIDI.MML.Event
{
    public class VelocityDecreaseEvent : MMLEvent
    {
        public int DecrementAmount;

        public VelocityDecreaseEvent(int decrementAmount)
            : base(MMLEventTag.VelocityDecrease)
        {
            DecrementAmount = decrementAmount;
        }
    }
}