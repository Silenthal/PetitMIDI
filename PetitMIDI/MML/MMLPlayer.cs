namespace PetitMIDI.MML
{
    /// <summary>
    /// A class to play MML.
    /// </summary>
    public class MMLPlayer
    {
        #region Private Members

        /// <summary>
        /// Handle to the current MIDI device.
        /// </summary>
        private MMLSoundGenerator midiOut;

        /// <summary>
        /// Timer for managing play time.
        /// </summary>
        private HighResTimer timer = new HighResTimer();

        /// <summary>
        /// Specifies whether an MML string is currently being played.
        /// </summary>
        private bool running = false;

        /// <summary>
        /// Represents the global tempo of the song.
        /// </summary>
        private int tempo;

        /// <summary>
        /// The channels of the player.
        /// </summary>
        private MMLChannel[] channels;

        #endregion Private Members

        /// <summary>
        /// Initializes a new instance of the <see cref="MMLPlayer"/> class.
        /// </summary>
        public MMLPlayer()
        {
            this.midiOut = new MMLSoundGenerator();
            this.channels = new MMLChannel[8]
            {
                new MMLChannel(0, this.GetNoteTime, this.ChangeTempo, this.midiOut.ChangeDuty, this.PlaySound),
                new MMLChannel(1, this.GetNoteTime, this.ChangeTempo, this.midiOut.ChangeDuty, this.PlaySound),
                new MMLChannel(2, this.GetNoteTime, this.ChangeTempo, this.midiOut.ChangeDuty, this.PlaySound),
                new MMLChannel(3, this.GetNoteTime, this.ChangeTempo, this.midiOut.ChangeDuty, this.PlaySound),
                new MMLChannel(4, this.GetNoteTime, this.ChangeTempo, this.midiOut.ChangeDuty, this.PlaySound),
                new MMLChannel(5, this.GetNoteTime, this.ChangeTempo, this.midiOut.ChangeDuty, this.PlaySound),
                new MMLChannel(6, this.GetNoteTime, this.ChangeTempo, this.midiOut.ChangeDuty, this.PlaySound),
                new MMLChannel(7, this.GetNoteTime, this.ChangeTempo, this.midiOut.ChangeDuty, this.PlaySound),
            };
            this.ResetChannels();
        }

        /// <summary>
        /// Opens a MIDI output device for playback.
        /// </summary>
        /// <param name="deviceID">The device ID.</param>
        /// <returns>True if the device was opened successfully.</returns>
        public void Open(int deviceID = 0)
        {
            midiOut.Open(deviceID);
        }

        /// <summary>
        /// Closes the current MIDI output device.
        /// </summary>
        public void Close()
        {
            this.StopRunning();
            this.midiOut.Close();
        }

        /// <summary>
        /// Stops the evaluation of any MML string currently being played.
        /// </summary>
        public void StopRunning()
        {
            this.running = false;
        }

        /// <summary>
        /// Plays an MML string.
        /// </summary>
        /// <param name="mml">The MML string to play.</param>
        public void Play(string mml)
        {
            if (this.running)
            {
                this.StopRunning();
            }

            if (mml.Length == 0)
            {
                return;
            }

            this.StopAllNotes();

            this.ResetChannels();
            string[] channelMML = MMLStack.PreProcessMML(mml).Split(':');
            if (channelMML.Length == 1)
            {
                this.LoadChannelMML(0, channelMML[0]);
            }
            else
            {
                for (int i = 0; i < channelMML.Length; i++)
                {
                    if (channelMML[i] != string.Empty && char.IsDigit(channelMML[i][0]))
                    {
                        int chan = (int)char.GetNumericValue(channelMML[i], 0);
                        this.LoadChannelMML(chan, channelMML[i].Substring(1));
                    }
                }
            }

            this.running = true;
            this.timer.Start();
            while (this.running)
            {
                for (int index = 0; index < this.channels.Length; index++)
                {
                    this.channels[index].Update(this.timer.ElapsedTime());
                    this.running |= !this.channels[index].IsDone;
                }
            }
        }

        /// <summary>
        /// Clears the MML from all channels playing.
        /// </summary>
        public void ResetChannels()
        {
            this.tempo = 120;
            foreach (MMLChannel c in this.channels)
            {
                c.ClearMML();
                c.ResetChannelDefaults();
            }
        }

        /// <summary>
        /// Loads MML into an existing channel.
        /// </summary>
        /// <param name="channel">The channel to load.</param>
        /// <param name="mml">THe MML to load the channel with.</param>
        public void LoadChannelMML(int channel, string mml)
        {
            if (channel >= 0 && channel < 8)
            {
                this.channels[channel].LoadMML(mml);
            }
        }

        /// <summary>
        /// Plays a sound, usigng a MIDI message.
        /// </summary>
        /// <param name="message">The MIDI message to send.</param>
        /// <param name="noteStyle">The type of note to play.</param>
        private void PlaySound(MIDIMessage message, NoteStyle noteStyle)
        {
            this.midiOut.PlaySound(message, noteStyle);
        }

        /// <summary>
        /// Gets the time that a note takes, based on the note value.
        /// </summary>
        /// <param name="noteValue">The value of the note being played.</param>
        /// <returns>The time the note takes, in seconds.</returns>
        private double GetNoteTime(double noteValue)
        {
            double secondsPerWholeNote = 240.0 / (double)this.tempo;
            double portionOfWhole = 1 / noteValue;
            return secondsPerWholeNote * portionOfWhole;
        }

        /// <summary>
        /// Changes the global tempo of the song being played.
        /// </summary>
        /// <param name="newTempo">The new tempo.</param>
        private void ChangeTempo(int newTempo)
        {
            this.tempo = newTempo;
            if (this.tempo < 1)
            {
                this.tempo = 1;
            }
            else if (this.tempo > 240)
            {
                this.tempo = 240;
            }
        }

        /// <summary>
        /// Stops all notes currently being played, on all channels.
        /// </summary>
        private void StopAllNotes()
        {
            MIDIMessage mst = new MIDIMessage();
            mst.Status = MessageType.ControlChange;
            mst.ControlType = ControlChangeType.AllNotesOff;
            this.PlaySound(mst, NoteStyle.Regular);
        }
    }
}