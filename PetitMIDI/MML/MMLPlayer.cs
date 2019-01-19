namespace PetitMIDI.MML
{
    using PetitMIDI.Audio;
    using System.Diagnostics;

    /// <summary>
    /// A class to play MML.
    /// </summary>
    public class MMLPlayer
    {
        #region Private Members

        /// <summary>
        /// Handle to the current MIDI device.
        /// </summary>
        private SoundGenerator midiOut;

        /// <summary>
        /// Timer for managing play time.
        /// </summary>
        private Stopwatch timer = new Stopwatch();

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
        private MMLChannel[] channels = new MMLChannel[Config.Channel.Count];

        #endregion Private Members

        /// <summary>
        /// Initializes a new instance of the <see cref="MMLPlayer"/> class.
        /// </summary>
        public MMLPlayer()
        {
            midiOut = new SoundGenerator();
            for (int i = 0; i < channels.Length; i++)
            {
                channels[i] = new MMLChannel(i, GetNoteTime, ChangeTempo, midiOut.ChangeDuty, midiOut.Send, midiOut.ChangeMode, midiOut.EnableEnvelope);
            }
            ResetChannels();
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
            StopRunning();
            midiOut.Close();
        }

        /// <summary>
        /// Stops the evaluation of any MML string currently being played.
        /// </summary>
        public void StopRunning()
        {
            running = false;
        }

        /// <summary>
        /// Plays an MML string.
        /// </summary>
        /// <param name="mml">The MML string to play.</param>
        public void Play(string mml)
        {
            if (running)
            {
                StopRunning();
            }

            if (mml.Length == 0)
            {
                return;
            }

            StopAllNotes();

            ResetChannels();
            string[] channelMML = MMLStack.PreProcessMML(":0" + mml).Split(':');
            if (channelMML.Length == 1)
            {
                LoadChannelMML(0, channelMML[0]);
            }
            else
            {
                for (int i = 0; i < channelMML.Length; i++)
                {
                    if (channelMML[i] != string.Empty && char.IsDigit(channelMML[i][0]))
                    {
                        int chan = (int)char.GetNumericValue(channelMML[i], 0);
                        LoadChannelMML(chan, channelMML[i].Substring(1));
                    }
                }
            }

            running = true;
            timer.Restart();
            while (running)
            {
                for (int index = 0; index < channels.Length; index++)
                {
                    channels[index].Update(timer.Elapsed.TotalSeconds);
                    running |= !channels[index].IsDone;
                }
            }
        }

        /// <summary>
        /// Clears the MML from all channels playing.
        /// </summary>
        public void ResetChannels()
        {
            tempo = Config.Tempo.Default;
            foreach (MMLChannel c in channels)
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
            if (channel >= 0 && channel <= channels.Length)
            {
                channels[channel].LoadMML(mml);
            }
        }

        /// <summary>
        /// Gets the time that a note takes, based on the note value.
        /// </summary>
        /// <param name="noteValue">The value of the note being played.</param>
        /// <returns>The time the note takes, in seconds.</returns>
        private double GetNoteTime(double noteValue)
        {
            double secondsPerWholeNote = (double)Config.Tempo.MaxValue / tempo;
            double portionOfWhole = 1 / noteValue;
            return secondsPerWholeNote * portionOfWhole;
        }

        /// <summary>
        /// Changes the global tempo of the song being played.
        /// </summary>
        /// <param name="newTempo">The new tempo.</param>
        private void ChangeTempo(int newTempo)
        {
            tempo = newTempo;
            if (tempo < Config.Tempo.MinValue)
            {
                tempo = Config.Tempo.MinValue;
            }
            else if (tempo > Config.Tempo.MaxValue)
            {
                tempo = Config.Tempo.MaxValue;
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
            midiOut.Send(mst);
        }
    }
}