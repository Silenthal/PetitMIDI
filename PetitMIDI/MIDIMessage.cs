namespace PetitMIDI
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// Enumerates the Control Change parameters that can be adjusted.
    /// </summary>
    public enum ControlChangeType : byte
    {
        BankSelect = 0x00,
        ModulationWheel = 0x01,
        BreathController = 0x02,
        FootController = 0x04,
        PortamentoTime = 0x05,
        DataEntry = 0x06,
        ChannelVolume = 0x07,
        Balance = 0x08,
        Pan = 0x0A,
        ExpressionController = 0x0B,
        EffectControl1 = 0x0C,
        EffectControl2 = 0x0D,
        GeneralPurposeController1 = 0x10,
        GeneralPurposeController2 = 0x11,
        GeneralPurposeController3 = 0x12,
        GeneralPurposeController4 = 0x13,
        BankSelectLSB = 0x20,
        ModulationWheelLSB = 0x21,
        BreathControllerLSB = 0x22,
        FootControllerLSB = 0x24,
        PortamentoTimeLSB = 0x25,
        DataEntryLSB = 0x26,
        ChannelVolumeLSB = 0x27,
        BalanceLSB = 0x28,
        PanLSB = 0x2A,
        ExpressionControllerLSB = 0x2B,
        EffectControl1LSB = 0x2C,
        EffectControl2LSB = 0x2D,
        GeneralPurposeController1LSB = 0x30,
        GeneralPurposeController2LSB = 0x31,
        GeneralPurposeController3LSB = 0x32,
        GeneralPurposeController4LSB = 0x33,
        DamperPedal = 0x40,
        PortamentoOnOff = 0x41,
        SostenutoOnOff = 0x42,
        SoftPedalOnOff = 0x43,
        LegatoFootswitch = 0x44,
        Hold2 = 0x45,
        SoundController01 = 0x46,
        SoundController02 = 0x47,
        SoundController03 = 0x48,
        SoundController04 = 0x49,
        SoundController05 = 0x4A,
        SoundController06 = 0x4B,
        SoundController07 = 0x4C,
        SoundController08 = 0x4D,
        SoundController09 = 0x4E,
        SoundController10 = 0x4F,
        SoundVariation = 0x46,
        TimbreHarmonicIntensity = 0x47,
        ReleaseTime = 0x48,
        AttackTime = 0x49,
        Brightness = 0x4A,
        DecayTime = 0x4B,
        VibratoRate = 0x4C,
        VibratoDepth = 0x4D,
        VibratoDelay = 0x4E,
        GeneralPurposeController5 = 0x50,
        GeneralPurposeController6 = 0x51,
        GeneralPurposeController7 = 0x52,
        GeneralPurposeController8 = 0x53,
        PortamentoControl = 0x54,
        HighResolutionVelocityPrefix = 0x58,
        Effects1Depth = 0x5B,
        Effects2Depth = 0x5C,
        Effects3Depth = 0x5D,
        Effects4Depth = 0x5E,
        Effects5Depth = 0x5F,
        ReverbSendLevel = 0x5B,
        TremoloDepth = 0x5C,
        ChorusSendLevel = 0x5D,
        CelesteDepth = 0x5E,
        DetuneDepth = 0x5E,
        PhaserDepth = 0x5F,
        DataEntryIncrement = 0x60,
        DataEntryDecrement = 0x61,
        NonRegisteredParameterNumberLSB = 0x62,
        NonRegisteredParameterNumberMSB = 0x63,
        RegisteredParameterNumberLSB = 0x64,
        RegisteredParameterNumberMSB = 0x65,
        AllSoundOff = 0x78,
        ResetAllControllers = 0x79,
        LocalControlOnOff = 0x7A,
        AllNotesOff = 0x7B,
        OmniModeOff = 0x7C,
        OmniModeOn = 0x7D,
        MonoModeOn = 0x7E,
        PolyModeOn = 0x7F
    }

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

    /// <summary>
    /// Describes a MIDI message.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct MIDIMessage
    {
        /// <summary>
        /// The raw data of the message.
        /// </summary>
        [FieldOffset(0)]
        private int rawData;

        /// <summary>
        /// The status parameter of the MIDI message.
        /// </summary>
        [FieldOffset(0)]
        private byte status;

        /// <summary>
        /// The data1 parameter of the MIDI message.
        /// </summary>
        [FieldOffset(1)]
        private byte data1;

        /// <summary>
        /// The data2 parameter of the MIDI message.
        /// </summary>
        [FieldOffset(2)]
        private byte data2;

        /// <summary>
        /// Gets or sets a value indicating the packed representation of this MIDI message as a single number.
        /// </summary>
        public int RawData
        {
            get
            {
                return this.rawData;
            }

            set
            {
                this.rawData = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the channel the MIDI message is directed to.
        /// </summary>
        public int Channel
        {
            get
            {
                return this.status & 0x0F;
            }

            set
            {
                this.status = (byte)((this.status & 0xF0) | (value & 0x0F));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the status parameter of the MIDI message.
        /// </summary>
        public MessageType Status
        {
            get
            {
                return (MessageType)(this.status & 0xF0);
            }

            set
            {
                this.status = (byte)((this.status & 0x0F) | ((byte)value));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the data1 parameter of the MIDI message.
        /// </summary>
        public int Data1
        {
            get
            {
                return this.data1 & 0x7F;
            }

            set
            {
                this.data1 = (byte)(value & 0x7F);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the data2 parameter of the MIDI message.
        /// </summary>
        public int Data2
        {
            get
            {
                return this.data2 & 0x7F;
            }

            set
            {
                this.data2 = (byte)(value & 0x7F);
            }
        }

        /// <summary>
        /// Sets a value indicating the parameter being changed in a Control Change message.
        /// </summary>
        public ControlChangeType ControlType
        {
            set
            {
                this.data1 = (byte)value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the value of the parameter being changed in a Control Change message.
        /// </summary>
        public int ControlValue
        {
            get
            {
                return this.data2 & 0x7F;
            }

            set
            {
                this.data2 = (byte)(value & 0x7F);
            }
        }
    }
}