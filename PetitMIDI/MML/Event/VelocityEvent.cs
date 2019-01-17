namespace PetitMIDI.MML.Event
{
    public class VelocityEvent : MMLEvent
    {
        public int Velocity { get; }

        public VelocityEvent(int newVelocity)
            : base(MMLEventTag.Velocity)
        {
            Velocity = newVelocity;
        }
    }
}