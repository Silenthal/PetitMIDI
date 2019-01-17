namespace PetitMIDI.MML.Event
{
    public class EnvelopeEvent : MMLEvent
    {
        public int Attack;
        public int Decay;
        public int Sustain;
        public int Release;

        public EnvelopeEvent(int attack, int decay, int sustain, int release)
            : base(MMLEventTag.Envelope)
        {
            Attack = attack;
            Decay = decay;
            Sustain = sustain;
            Release = release;
        }
    }
}