namespace PetitMIDI.MML.Event
{
    /// <summary>
    /// Describes the type of MML event.
    /// </summary>
    public enum MMLEventTag
    {
        None,
        Invalid,
        Note,
        Tie,
        Pitch,
        Rest,
        Length,
        Tempo,
        Velocity,
        VelocityIncrease,
        VelocityDecrease,
        Pan,
        Instrument,
        Octave,
        OctaveIncrease,
        OctaveDecrease,
        Volume,
        Detune,
        Envelope,
        EnvelopeRelease,
        Modulation,
        Tremolo,
        Vibrato
    }
}