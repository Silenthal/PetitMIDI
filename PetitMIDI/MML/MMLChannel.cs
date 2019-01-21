namespace PetitMIDI.MML
{
    using PetitMIDI.Audio;
    using PetitMIDI.MML.Event;

    /// <summary>
    /// Represents a music channel.
    /// </summary>
    public class MMLChannel
    {
        /// <summary>
        /// Sends a message to go to the main thread.
        /// </summary>
        /// <param name="message">The MIDI message.</param>
        public delegate void SendEventDelegate(MIDIMessage message);

        public delegate double GetTimeDelegate();

        public delegate void ChangeTempoDelegate(int newTempo);

        public delegate double GetNoteTimeDelegate(double noteValue);

        public delegate void ChangeDutyDelegate(int channel, float newDuty);

        public delegate void ChangeNoteStyleDelegate(int channel, NoteStyle noteType);

        public delegate void ChangeEnvelopeDelegate(int channel, int attack, int delay, int sustain, int release);

        /// <summary>
        /// The ID of the current channel.
        /// </summary>
        private int channelID;

        /// <summary>
        /// The instrument of the current channel.
        /// </summary>
        private int instrument;

        /// <summary>
        /// Represents the default velocity of any notes played.
        /// </summary>
        private int velocity;

        /// <summary>
        /// Represents the default octave of any notes played.
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
        /// Indicates the types of notes the channel is playing.
        /// </summary>
        private NoteStyle noteStyle;

        private SendEventDelegate SendEvent;

        private ChangeTempoDelegate ChangeTempo;

        private GetNoteTimeDelegate GetNoteTime;

        private ChangeDutyDelegate ChangeDuty;

        private ChangeNoteStyleDelegate ChangeNoteStyle;

        private ChangeEnvelopeDelegate ChangeEnvelope;

        private MMLStack mStack;

        public MMLChannel(int channel,
            GetNoteTimeDelegate noteTimeFunction,
            ChangeTempoDelegate tempoFunction,
            ChangeDutyDelegate dutyFunction,
            SendEventDelegate eventFunction,
            ChangeNoteStyleDelegate noteStyleFunction,
            ChangeEnvelopeDelegate changeEnvelopeFunction)
        {
            channelID = channel;

            GetNoteTime = noteTimeFunction;
            ChangeTempo = tempoFunction;
            SendEvent = eventFunction;
            ChangeDuty = dutyFunction;
            ChangeNoteStyle = noteStyleFunction;
            ChangeEnvelope = changeEnvelopeFunction;

            mStack = new MMLStack();

            ResetChannelDefaults();
        }

        /// <summary>
        /// Returns true if the channel is currently empty.
        /// </summary>
        public bool IsDone => mStack.IsEmpty;

        /// <summary>
        /// Resets the default values for the MML being played.
        /// </summary>
        /// <param name="channelOpen">If the channel is currently open, then do </param>
        public void ResetChannelDefaults()
        {
            velocity = Config.Velocity.Default;
            octave = Config.Octave.Default;
            noteTimeValue = 4;
            nextUpdateTime = 0;
            lastNotePlayed = 0;
            noteStyle = NoteStyle.Regular;
            mStack.Refresh();
            ChangeNoteStyle(channelID, noteStyle);
        }

        /// <summary>
        /// Clears the <see cref="MMLStack"/> associated with this channel.
        /// </summary>
        public void ClearMML()
        {
            mStack.Clear();
            ChangeInstrument(0);
            ChangeVolume(Constants.Volume.MaxValue);
        }

        /// <summary>
        /// Loads a new MML string into the stack.
        /// </summary>
        /// <param name="mml"></param>
        public void LoadMML(string mml)
        {
            mStack.PushBack(mml);
            ChangeInstrument(0);
            ChangeVolume(Constants.Volume.MaxValue);
        }

        /// <summary>
        /// Plays an event from the list of MML, based on the time given.
        /// </summary>
        /// <param name="time">The current time.</param>
        public void Update(double time)
        {
            if (time < nextUpdateTime)
            {
                return;
            }
            if (mStack.IsEmpty)
            {
                StopNote(lastNotePlayed);
                return;
            }
            bool tieTriggered = false;
            MMLEvent command = mStack.PopEvent();
            if (command.Tag == MMLEventTag.None)
            {
                return;
            }
            else if (command.Tag == MMLEventTag.Tie)
            {
                command = mStack.PopEvent();
                tieTriggered = true;
            }
            else if (isNoteBeingPlayed)
            {
                StopNote(lastNotePlayed);
            }
            switch (command.Tag)
            {
                case MMLEventTag.Velocity:
                    {
                        ChangeVelocity((command as VelocityEvent).Velocity);
                    }

                    break;

                case MMLEventTag.VelocityIncrease:
                    {
                        ChangeVelocity(velocity + (command as VelocityIncreaseEvent).IncrementAmount);
                    }

                    break;

                case MMLEventTag.VelocityDecrease:
                    {
                        ChangeVelocity(velocity - (command as VelocityDecreaseEvent).DecrementAmount);
                    }

                    break;

                case MMLEventTag.Volume:
                    {
                        ChangeVolume((command as VolumeEvent).Volume);
                    }

                    break;

                case MMLEventTag.Instrument:
                    {
                        ChangeInstrument((command as InstrumentEvent).Instrument);
                    }

                    break;

                case MMLEventTag.Length:
                    {
                        ChangeNoteLength((command as LengthEvent).Length);
                    }

                    break;

                case MMLEventTag.Octave:
                    {
                        ChangeOctave((command as OctaveEvent).Octave);
                    }

                    break;

                case MMLEventTag.Pan:
                    {
                        ChangePan((command as PanEvent).Pan);
                    }

                    break;

                case MMLEventTag.Tempo:
                    {
                        ChangeTempo((command as TempoEvent).Tempo);
                    }

                    break;

                case MMLEventTag.OctaveIncrease:
                    {
                        IncreaseOctave();
                    }

                    break;

                case MMLEventTag.OctaveDecrease:
                    {
                        DecreaseOctave();
                    }

                    break;

                case MMLEventTag.Envelope:
                    {
                        EnvelopeEvent ev = command as EnvelopeEvent;
                        ChangeEnvelope(channelID, ev.Attack, ev.Decay, ev.Sustain, ev.Release);
                    }
                    break;

                case MMLEventTag.EnvelopeRelease:
                    {
                        if (noteStyle == NoteStyle.Regular || noteStyle == NoteStyle.Drums)
                        {
                            ChangeEnvelope(channelID, 64, 64, 64, 64);
                        }
                        else
                        {
                            ChangeEnvelope(channelID, 0, 0, 127, 0);
                        }
                    }
                    break;

                case MMLEventTag.Rest:
                    {
                        RestEvent restTemp = command as RestEvent;
                        if (restTemp.NoteValue == -1)
                        {
                            restTemp.NoteValue = noteTimeValue;
                        }

                        nextUpdateTime += GetNoteTime(restTemp.ActualNoteValue);
                    }

                    break;

                case MMLEventTag.Note:
                    {
                        NoteEvent noteTemp = command as NoteEvent;
                        noteTemp.BaseNote += (octave + noteTemp.OctaveOffset) * 12;
                        if (noteTemp.BaseNote < 0 || noteTemp.BaseNote > 127)
                        {
                            break;
                        }
                        if (noteTemp.NoteValue == -1)
                        {
                            noteTemp.NoteValue = noteTimeValue;
                        }
                        if (!tieTriggered)
                        {
                            PlayNote(noteTemp.BaseNote);
                            lastNotePlayed = noteTemp.BaseNote;
                        }
                        nextUpdateTime += GetNoteTime(noteTemp.ActualNoteValue);
                    }

                    break;

                case MMLEventTag.Pitch:
                    {
                        PitchEvent noteTemp = command as PitchEvent;
                        PlayNote(noteTemp.Pitch);
                        lastNotePlayed = noteTemp.Pitch;
                        nextUpdateTime += GetNoteTime(noteTimeValue);
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
            isNoteBeingPlayed = true;
            int tempNote = baseNote;
            MIDIMessage message = new MIDIMessage();
            message.Status = MessageType.NoteOn;
            message.Data1 = baseNote;
            message.Data2 = velocity;
            message.Channel = channelID;
            if (noteStyle == NoteStyle.Drums)
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
            SendEvent(message);
        }

        /// <summary>
        /// Stops a note.
        /// </summary>
        /// <param name="baseNote">The base note value.</param>
        /// <param name="forcePSG">If true, force the note being stopped to be a PSG note.</param>
        private void StopNote(int baseNote, bool forcePSG = false)
        {
            isNoteBeingPlayed = false;
            MIDIMessage message = new MIDIMessage();
            message.Status = MessageType.NoteOn;
            message.Data1 = baseNote;
            message.Data2 = 0;
            message.Channel = channelID;
            if (noteStyle == NoteStyle.Drums)
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

            SendEvent(message);
        }

        /// <summary>
        /// Changes the velocity of notes being played.
        /// </summary>
        /// <param name="newVelocity">The new velocity.</param>
        private void ChangeVelocity(int newVelocity)
        {
            velocity = Utility.Clamp(newVelocity, Constants.Velocity.MinValue, Constants.Velocity.MaxValue);
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
            mst.ControlValue = Utility.Clamp(newVolume, Constants.Volume.MinValue, Constants.Volume.MaxValue); ;
            SendEvent(mst);
        }

        /// <summary>
        /// Changes the default note length for notes played.
        /// </summary>
        /// <param name="newNoteLength">The new note length.</param>
        private void ChangeNoteLength(int newNoteLength)
        {
            noteTimeValue = Utility.Clamp(newNoteLength, Constants.NoteLength.MinValue, Constants.NoteLength.MaxValue);
        }

        /// <summary>
        /// Changes the instrument on the specified channel.
        /// </summary>
        /// <param name="newInstrument">The instrument to change to.</param>
        private void ChangeInstrument(int newInstrument)
        {
            // Note: These instrument ranges map to the same thing:
            // 144-151 (7 square waves plus one noise)
            // 152-159
            // 160-167
            // 168-175
            instrument = newInstrument;
            if (instrument < 128)
            {
                ChangeNoteStyle(channelID, NoteStyle.Regular);
                noteStyle = NoteStyle.Regular;
                MIDIMessage mst = new MIDIMessage();
                mst.Channel = channelID;
                mst.Status = MessageType.ProgramChange;
                mst.Data1 = instrument;
                SendEvent(mst);
            }
            else if (instrument > 143 & instrument < 176)
            {
                while (instrument > 151)
                {
                    instrument -= 8;
                }
                if (instrument == 151)
                {
                    ChangeNoteStyle(channelID, NoteStyle.Noise);
                    noteStyle = NoteStyle.Noise;
                }
                else
                {
                    ChangeNoteStyle(channelID, NoteStyle.PSG);
                    noteStyle = NoteStyle.PSG;
                    ChangeDuty(channelID, .125f * (instrument - 143));
                }
            }
            else
            {
                ChangeNoteStyle(channelID, NoteStyle.Drums);
                noteStyle = NoteStyle.Drums;
            }
        }

        /// <summary>
        /// Changes the pan (location of sound between left and right speakers).
        /// </summary>
        /// <param name="newPan">The pan value (0-127 inclusive).</param>
        private void ChangePan(int newPan)
        {
            MIDIMessage mst = new MIDIMessage();
            mst.Status = MessageType.ControlChange;
            mst.Channel = channelID;
            mst.ControlType = ControlChangeType.Pan;
            mst.ControlValue = Utility.Clamp(newPan, Constants.Pan.MinValue, Constants.Pan.MaxValue);
            SendEvent(mst);
        }

        /// <summary>
        /// Changes the global octave value of notes being played.
        /// </summary>
        /// <param name="newOctave">The octave to change to.</param>
        private void ChangeOctave(int newOctave)
        {
            octave = Utility.Clamp(newOctave, Constants.Octave.MinValue, Constants.Octave.MaxValue) + 1;
        }

        /// <summary>
        /// Increases the default octave.
        /// </summary>
        private void IncreaseOctave()
        {
            ChangeOctave(octave);
        }

        /// <summary>
        /// Decreases the default octave.
        /// </summary>
        private void DecreaseOctave()
        {
            ChangeOctave(octave - 2);
        }

        /// <summary>
        /// Enables a vibrato effect (pitch fluctuation).
        /// </summary>
        /// <param name="depth">The depth of the vibrato.</param>
        /// <param name="range">The effective range over which the vibrato varies.</param>
        /// <param name="speed">The speed of the variance in the vibrato.</param>
        /// <param name="delay">The delay of the start of the vibrato.</param>
        private void EnableVibrato(int depth, int range, int speed, int delay)
        {
            // TODO: Not sure how to handle sustain level just yet.
            MIDIMessage message = new MIDIMessage();
            message.Channel = channelID;
            message.Status = MessageType.ControlChange;
            message.ControlType = ControlChangeType.SoundController08; // Vibrato Depth
            message.ControlValue = depth & 0xFF;
            SendEvent(message);
            message.ControlType = ControlChangeType.SoundController07; // Vibrato Rate
            message.ControlValue = range & 0xFF;
            SendEvent(message);
            message.ControlType = ControlChangeType.SoundController09; // Vibrato Delay
            message.ControlValue = delay & 0xFF;
            SendEvent(message);
        }

        /// <summary>
        /// Disables an active vibrato effect (pitch fluctuation).
        /// </summary>
        private void DisableVibrato()
        {
            // TODO: Not sure how to handle sustain level just yet.
            EnableVibrato(128, 128, 128, 128);
        }
    }
}