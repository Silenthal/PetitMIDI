namespace PetitMIDI
{
    using System.Runtime.InteropServices;

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
                return rawData;
            }

            set
            {
                rawData = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the channel the MIDI message is directed to.
        /// </summary>
        public int Channel
        {
            get
            {
                return status & 0x0F;
            }

            set
            {
                status = (byte)((status & 0xF0) | (value & 0x0F));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the status parameter of the MIDI message.
        /// </summary>
        public MessageType Status
        {
            get
            {
                return (MessageType)(status & 0xF0);
            }

            set
            {
                status = (byte)((status & 0x0F) | ((byte)value));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the data1 parameter of the MIDI message.
        /// </summary>
        public int Data1
        {
            get
            {
                return data1 & 0x7F;
            }

            set
            {
                data1 = (byte)(value & 0x7F);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the data2 parameter of the MIDI message.
        /// </summary>
        public int Data2
        {
            get
            {
                return data2 & 0x7F;
            }

            set
            {
                data2 = (byte)(value & 0x7F);
            }
        }

        /// <summary>
        /// Sets a value indicating the parameter being changed in a Control Change message.
        /// </summary>
        public ControlChangeType ControlType
        {
            get
            {
                return (ControlChangeType)data1;
            }
            set
            {
                data1 = (byte)value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the value of the parameter being changed in a Control Change message.
        /// </summary>
        public int ControlValue
        {
            get
            {
                return Data2;
            }

            set
            {
                Data2 = value;
            }
        }
    }
}