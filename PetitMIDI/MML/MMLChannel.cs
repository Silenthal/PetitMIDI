namespace PetitMIDI.MML
{
    using PetitMIDI.MML.Event;

    /// <summary>
    /// Represents a music channel.
    /// </summary>
    public class MMLChannel
    {
        private struct PSGNoteMessage
        {
            public enum MessageState{Stopped, Initiated, Playing}
            public MessageState State
            {
                get;
                set;
            }
            private static double psgStartDelayTime = .1d;
            private static double psgEndDelayTime = .1d;
            private double noteOn;
            private double noteOff;
            public double noteOnTime
            {
                get
                {
                    return noteOn + psgStartDelayTime;
                }
                set
                {
                    noteOn = value;
                }
            }
            public double noteOffTime
            {
                get
                {
                    return noteOff + psgEndDelayTime;
                }
                set
                {
                    noteOff = value;
                }
            }
            public int noteValue;
        }

        private PSGNoteMessage currentPSGNote;

        /// <summary>
        /// Sends a message to go to the main thread.
        /// </summary>
        /// <param name="message">The MIDI message.</param>
        public delegate void SendEventDelegate(MIDIMessage message, NoteStyle noteStyle);

        public delegate double GetTimeDelegate();

        public delegate void ChangeTempoDelegate(int newTempo);

        public delegate double GetNoteTimeDelegate(double noteValue);

        public delegate void ChangeDutyDelegate(int channel, float newDuty);

        /// <summary>
        /// The ID of the current channel.
        /// </summary>
        private int channelID;

        /// <summary>
        /// The instrument of the current channel.
        /// </summary>
        private int instrument;

        /// <summary>
        /// Contains the volume of this channel.
        /// </summary>
        private int volume;

        /// <summary>
        /// Represents the global default velocity of any notes played.
        /// </summary>
        private int velocity;

        /// <summary>
        /// Represents the global default octave of any notes played.
        /// </summary>
        private int octave;

        /// <summary>
        /// Represents the default time value of any notes played.
        /// </summary>
        private int noteTimeValue;

        /// <summary>
        /// Contains the base time of the channel.
        /// </summary>
        private double nextUpdateTime;

        /// <summary>
        /// Contains the last note played.
        /// </summary>
        private int lastNotePlayed;

        /// <summary>
        /// True if the last event was a note event.
        /// </summary>
        private bool isNoteBeingPlayed;

        /// <summary>
        /// Indicates the types of notes the channel is playing/
        /// </summary>
        private NoteStyle noteStyle;

        private SendEventDelegate SendEvent;

        private ChangeTempoDelegate ChangeTempo;

        private GetNoteTimeDelegate GetNoteTime;

        private ChangeDutyDelegate ChangeDuty;

        private MMLStack mStack;

        public MMLChannel(int channel,
            GetNoteTimeDelegate noteTimeFunction,
            ChangeTempoDelegate tempoFunction,
            ChangeDutyDelegate dutyFunction,
            SendEventDelegate eventFunction)
        {
            this.channelID = channel;

            this.GetNoteTime = noteTimeFunction;
            this.ChangeTempo = tempoFunction;
            this.SendEvent = eventFunction;
            this.ChangeDuty = dutyFunction;

            this.mStack = new MMLStack();

            this.ResetChannelDefaults();
            
        }

        /// <summary>
        /// Returns true if the channel is currently empty.
        /// </summary>
        public bool IsDone
        {
            get
            {
                return this.mStack.IsEmpty && !this.isNoteBeingPlayed;
            }
        }

        /// <summary>
        /// Resets the default values for the MML being played.
        /// </summary>
        /// <param name="channelOpen">If the channel is currently open, then do this.</param>
        public void ResetChannelDefaults()
        {
            this.velocity = 127;
            this.volume = 127;
            this.octave = 5;
            this.noteTimeValue = 4;
            this.nextUpdateTime = 0;
            this.lastNotePlayed = 0;
            this.noteStyle = NoteStyle.Regular;
            this.mStack.Refresh();
        }

        /// <summary>
        /// Clears the <see cref="MMLStack"/> associated with this channel.
        /// </summary>
        public void ClearMML()
        {
            this.mStack.Clear();
            this.ChangeInstrument(0);
            this.ChangeVolume(this.volume);
        }

        /// <summary>
        /// Loads a new MML string into the stack.
        /// </summary>
        /// <param name="mml"></param>
        public void LoadMML(string mml)
        {
            mStack.PushBack(mml);
            this.ChangeInstrument(0);
            this.ChangeVolume(this.volume);
        }

        /// <summary>
        /// Plays an event from the list of MML, based on the time given.
        /// </summary>
        /// <param name="time">The current time.</param>
        public void Update(double time)
        {
            if (currentPSGNote.State == PSGNoteMessage.MessageState.Initiated && time >= currentPSGNote.noteOnTime)
            {
                PlayNote(currentPSGNote.noteValue, true);
                currentPSGNote.State = PSGNoteMessage.MessageState.Playing;
            }

            if (currentPSGNote.State == PSGNoteMessage.MessageState.Playing && time >= currentPSGNote.noteOffTime)
            {
                StopNote(currentPSGNote.noteValue, true);
                currentPSGNote.State = PSGNoteMessage.MessageState.Stopped;
            }

            if (time < this.nextUpdateTime)
            {
                return;
            }

            if (this.isNoteBeingPlayed)
            {
                this.StopNote(this.lastNotePlayed);
            }

            if (this.mStack.IsEmpty)
            {
                return;
            }

            MMLEvent command = mStack.PopEvent();
            if (command.Tag == EventTag.None)
            {
                return;
            }
            switch (command.Tag)
            {
                case EventTag.Velocity:
                    {
                        this.ChangeVelocity((command as VelocityEvent).Velocity);
                    }

                    break;

                case EventTag.VelocityIncrease:
                    {
                        this.ChangeVelocity(this.velocity + (command as VelocityIncreaseEvent).IncrementAmount);
                    }

                    break;

                case EventTag.VelocityDecrease:
                    {
                        this.ChangeVelocity(this.velocity - (command as VelocityDecreaseEvent).DecrementAmount);
                    }

                    break;

                case EventTag.Volume:
                    {
                        this.ChangeVolume((command as VolumeEvent).Volume);
                    }

                    break;

                case EventTag.Instrument:
                    {
                        this.ChangeInstrument((command as InstrumentEvent).Instrument);
                    }

                    break;

                case EventTag.Length:
                    {
                        this.ChangeNoteLength((command as LengthEvent).Length);
                    }

                    break;

                case EventTag.Octave:
                    {
                        this.ChangeOctave((command as OctaveEvent).Octave);
                    }

                    break;

                case EventTag.Pan:
                    {
                        this.ChangePan((command as PanEvent).Pan);
                    }

                    break;

                case EventTag.Tempo:
                    {
                        this.ChangeTempo((command as TempoEvent).Tempo);
                    }

                    break;

                case EventTag.OctaveIncrease:
                    {
                        this.IncreaseOctave();
                    }

                    break;

                case EventTag.OctaveDecrease:
                    {
                        this.DecreaseOctave();
                    }

                    break;

                case EventTag.Rest:
                    {
                        RestEvent restTemp = command as RestEvent;
                        if (restTemp.NoteValue == -1)
                        {
                            restTemp.NoteValue = this.noteTimeValue;
                        }

                        this.nextUpdateTime += this.GetNoteTime(restTemp.ActualNoteValue);
                    }

                    break;

                case EventTag.Note:
                    {
                        NoteEvent noteTemp = command as NoteEvent;
                        noteTemp.BaseNote += this.octave * 12;
                        if (noteTemp.NoteValue == -1)
                        {
                            noteTemp.NoteValue = this.noteTimeValue;
                        }

                        if (this.noteStyle == NoteStyle.PSG)
                        {
                            currentPSGNote.noteOnTime = this.nextUpdateTime;
                            currentPSGNote.noteOffTime = this.nextUpdateTime + this.GetNoteTime(noteTemp.ActualNoteValue);
                            currentPSGNote.noteValue = noteTemp.BaseNote;
                            currentPSGNote.State = PSGNoteMessage.MessageState.Initiated;
                        }
                        else
                        {
                            this.PlayNote(noteTemp.BaseNote);
                            this.lastNotePlayed = noteTemp.BaseNote;
                        }

                        this.nextUpdateTime += this.GetNoteTime(noteTemp.ActualNoteValue);
                    }

                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Plays a note.
        /// </summary>
        /// <param name="baseNote">The base note value.</param>
        /// <param name="forcePSG">If true, forces the note being started to be a PSG note.</param>
        private void PlayNote(int baseNote, bool forcePSG = false)
        {
            this.isNoteBeingPlayed = true;
            int tempNote = baseNote;
            MIDIMessage message = new MIDIMessage();
            message.Status = MessageType.NoteOn;
            message.Data1 = baseNote;
            message.Data2 = this.velocity;
            message.Channel = channelID;
            if (this.noteStyle == NoteStyle.Drums)
            {
                message.Channel = 9;
                if (message.Data1 < 0x23)
                {
                    message.Data1 = 0x23;
                }
                else if (message.Data1 > 0x51)
                {
                    message.Data1 = 0x51;
                }
            }

            SendEvent(message, forcePSG ? NoteStyle.PSG : this.noteStyle);
        }

        /// <summary>
        /// Stops a note.
        /// </summary>
        /// <param name="baseNote">The base note value.</param>
        /// <param name="forcePSG">If true, force the note being stopped to be a PSG note.</param>
        private void StopNote(int baseNote, bool forcePSG = false)
        {
            this.isNoteBeingPlayed = false;
            MIDIMessage message = new MIDIMessage();
            message.Status = MessageType.NoteOff;
            message.Data1 = baseNote;
            message.Data2 = this.velocity;
            message.Channel = channelID;
            if (this.noteStyle == NoteStyle.Drums)
            {
                if (message.Data1 < 0x23)
                {
                    message.Data1 = 0x23;
                }
                else if (message.Data1 > 0x51)
                {
                    message.Data1 = 0x51;
                }
            }

            SendEvent(message, this.noteStyle);
        }

        /// <summary>
        /// Changes the velocity of notes being played.
        /// </summary>
        /// <param name="newVelocity">The new velocity.</param>
        private void ChangeVelocity(int newVelocity)
        {
            this.velocity = newVelocity;
            if (this.velocity < 0)
            {
                this.velocity = 0;
            }
            if (this.velocity > 127)
            {
                this.velocity = 127;
            }
        }

        /// <summary>
        /// Changes the volume of the current channel.
        /// </summary>
        /// <param name="newVolume">The volume to adjust the channel to.</param>
        private void ChangeVolume(int newVolume)
        {
            MIDIMessage mst = new MIDIMessage();
            mst.Channel = channelID;
            mst.Status = MessageType.ControlChange;
            mst.ControlType = ControlChangeType.ChannelVolume;
            mst.ControlValue = volume;
            SendEvent(mst, this.noteStyle);
        }

        /// <summary>
        /// Changes the default note length for notes played.
        /// </summary>
        /// <param name="newNoteLength">The new note length.</param>
        private void ChangeNoteLength(int newNoteLength)
        {
            this.noteTimeValue = newNoteLength;
            if (this.noteTimeValue < 1)
            {
                this.noteTimeValue = 1;
            }
            else if (this.noteTimeValue > 192)
            {
                this.noteTimeValue = 192;
            }
        }

        /// <summary>
        /// Changes the instrument on the specified channel.
        /// </summary>
        /// <param name="newInstrument">The instrument to change to.</param>
        private void ChangeInstrument(int newInstrument)
        {
            this.instrument = newInstrument;
            if (this.instrument < 128)
            {
                noteStyle = NoteStyle.Regular;
                MIDIMessage mst = new MIDIMessage();
                mst.Channel = channelID;
                mst.Status = MessageType.ProgramChange;
                mst.Data1 = this.instrument;
                SendEvent(mst, this.noteStyle);
            }
            else if (this.instrument > 143 & this.instrument < 151)
            {
                noteStyle = NoteStyle.PSG;
                ChangeDuty(this.channelID, .125f * (this.instrument - 143));
            }
            else
            {
                noteStyle = NoteStyle.Drums;
            }
            
        }

        /// <summary>
        /// Changes the pan (location of sound between left and right speakers).
        /// </summary>
        /// <param name="pan">The pan value (0-127 inclusive).</param>
        private void ChangePan(int pan)
        {
            MIDIMessage mst = new MIDIMessage();
            mst.Status = MessageType.ControlChange;
            mst.Channel = channelID;
            mst.ControlType = ControlChangeType.Pan;
            mst.ControlValue = pan;
            SendEvent(mst, this.noteStyle);
        }

        /// <summary>
        /// Changes the global octave value of notes being played.
        /// </summary>
        /// <param name="newOctave">The octave to change to.</param>
        private void ChangeOctave(int newOctave)
        {
            this.octave = newOctave + 1;
            if (this.octave < 1)
            {
                this.octave = 1;
            }
            else if (this.octave > 8)
            {
                this.octave = 8;
            }
        }

        /// <summary>
        /// Increases the default octave.
        /// </summary>
        private void IncreaseOctave()
        {
            octave++;
            if (this.octave > 9)
            {
                this.octave = 9;
            }
        }

        /// <summary>
        /// Decreases the default octave.
        /// </summary>
        private void DecreaseOctave()
        {
            octave--;
            if (this.octave < 1)
            {
                this.octave = 1;
            }
        }
    }
}