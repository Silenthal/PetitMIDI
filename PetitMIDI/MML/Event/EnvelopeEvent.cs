namespace PetitMIDI.MML.Event
{
    public class EnvelopeEvent : MMLEvent
    {
        public int Attack;
        public int Decay;
        public int Sustain;
        public int Release;

        public EnvelopeEvent(int attack, int decay, int sustain, int release)
            : base(EventTag.Envelope)
        {
            this.Attack = attack;
            this.Decay = decay;
            this.Sustain = sustain;
            this.Release = release;
        }
    }
}