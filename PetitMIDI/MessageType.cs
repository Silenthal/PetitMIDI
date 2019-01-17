namespace PetitMIDI
{
    /// <summary>
    /// Enumerates the types of MIDI messages that can be sent.
    /// </summary>
    public enum MessageType : byte
    {
        NoteOff = 0x80,
        NoteOn = 0x90,
        PolyphonicKeyPressure = 0xA0,
        ControlChange = 0xB0,
        ProgramChange = 0xC0,
        ChannelPressure = 0xD0,
        PitchWheelChange = 0xE0,
        SystemExclusive = 0xF0,
    }
}