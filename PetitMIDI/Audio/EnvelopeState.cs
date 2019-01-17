// Adapted from code from http://www.earlevel.com/main/2013/06/01/envelope-generators/
namespace PetitMIDI.Audio
{
    /// <summary>
    /// Represents the current state of the envelope.
    /// </summary>
    public enum EnvelopeState
    {
        Idle,
        Attack,
        Decay,
        Sustain,
        Release
    }
}