namespace PetitMIDI.MML.Event
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class VelocityEvent : MMLEvent
    {
        private int velocity;

        public int Velocity
        {
            get
            {
                return this.velocity;
            }
        }

        public VelocityEvent(int newVelocity)
            : base(EventTag.Velocity)
        {
            this.velocity = newVelocity;
        }
    }
}