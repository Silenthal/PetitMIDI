namespace PetitMIDI.MML
{
    using NAudio.Midi;
    using NAudio.Wave;
    using PetitMIDI.Wave;
    using NAudio.CoreAudioApi;

    public enum NoteStyle
    {
        Regular,
        Drums,
        PSG
    }

    /// <summary>
    /// Represents the sound creator for MML.
    /// </summary>
    public class MMLSoundGenerator
    {
        private MidiOut midiOut;

        private WaveGenerator[] swp;

        private WasapiOut[] waveOutChannels;

        public MMLSoundGenerator()
        {
            this.midiOut = new NAudio.Midi.MidiOut(0);
            this.waveOutChannels = new WasapiOut[8];
            this.swp = new WaveGenerator[8];
            for (int i = 0; i < 8; i++)
            {
                swp[i] = new WaveGenerator(WaveType.Square);
                this.swp[i].SetWaveFormat(44100, 1);
                this.swp[i].Frequency = 400;
                this.swp[i].Amplitude = 0.2f;
                this.waveOutChannels[i] = new WasapiOut(AudioClientShareMode.Shared, 5);
                this.waveOutChannels[i].Init(swp[i]);
            }
        }

        public MMLSoundGenerator(int devID)
        {
            this.midiOut = new NAudio.Midi.MidiOut(devID);
        }

        public void Open(int deviceID = 0)
        {
            if (this.midiOut != null)
            {
                this.midiOut.Close();
            }
            this.midiOut = new MidiOut(deviceID);
        }

        public void Close()
        {
            this.midiOut.Close();
            for (int i = 0; i < 8; i++)
            {
                this.waveOutChannels[i].Stop();
            }
        }

        public void PlaySound(MIDIMessage message, NoteStyle noteType)
        {
            if (message.Status == MessageType.NoteOn && noteType == NoteStyle.PSG)
            {
                this.swp[message.Channel].Frequency = this.frequencyTable[message.Data1];
                PlayPSGNote(message);
            }
            else if (message.Status == MessageType.NoteOff && noteType == NoteStyle.PSG)
            {
                StopPSGNote(message);
            }
            else
            {
                MIDIMessage mst = message;
                if (noteType == NoteStyle.Drums)
                {
                    mst.Channel = 9;
                }
                this.midiOut.Send(mst.RawData);
            }
        }

        private void PlayPSGNote(MIDIMessage message)
        {
            StopPSGNote(message);
            this.waveOutChannels[message.Channel].Play();
        }

        private void StopPSGNote(MIDIMessage message)
        {
            this.waveOutChannels[message.Channel].Stop();
        }

        public void ChangeDuty(int channel, float newDuty)
        {
            this.swp[channel].Duty = newDuty;
            if (this.swp[channel].Duty >= 1)
            {
                this.swp[channel].Duty = .875f;
            }
            else if (this.swp[channel].Duty < 0)
            {
                this.swp[channel].Duty = .125f;
            }
        }
        
        private float[] frequencyTable = new float[128]
        {
            8.1758f,
            8.66196f,
            9.17702f,
            9.72272f,
            10.3009f,
            10.9134f,
            11.5623f,
            12.2499f,
            12.9783f,
            13.75f,
            14.5676f,
            15.4339f,
            16.3516f,
            17.3239f,
            18.354f,
            19.4454f,
            20.6017f,
            21.8268f,
            23.1247f,
            24.4997f,
            25.9565f,
            27.5f,
            29.1352f,
            30.8677f,
            32.7032f,
            34.6478f,
            36.7081f,
            38.8909f,
            41.2034f,
            43.6535f,
            46.2493f,
            48.9994f,
            51.9131f,
            55f,
            58.2705f,
            61.7354f,
            65.4064f,
            69.2957f,
            73.4162f,
            77.7817f,
            82.4069f,
            87.3071f,
            92.4986f,
            97.9988f,
            103.826f,
            110f,
            116.541f,
            123.471f,
            130.813f,
            138.591f,
            146.832f,
            155.563f,
            164.814f,
            174.614f,
            184.997f,
            195.998f,
            207.652f,
            220f,
            233.082f,
            246.942f,
            261.626f,
            277.183f,
            293.665f,
            311.127f,
            329.628f,
            349.228f,
            369.994f,
            391.995f,
            415.305f,
            440f,
            466.164f,
            493.883f,
            523.251f,
            554.365f,
            587.33f,
            622.254f,
            659.255f,
            698.456f,
            739.989f,
            783.991f,
            830.609f,
            880f,
            932.328f,
            987.767f,
            1046.5f,
            1108.73f,
            1174.66f,
            1244.51f,
            1318.51f,
            1396.91f,
            1479.98f,
            1567.98f,
            1661.22f,
            1760f,
            1864.65f,
            1975.53f,
            2093f,
            2217.46f,
            2349.32f,
            2489.02f,
            2637.02f,
            2793.83f,
            2959.96f,
            3135.96f,
            3322.44f,
            3520f,
            3729.31f,
            3951.07f,
            4186.01f,
            4434.92f,
            4698.64f,
            4978.03f,
            5274.04f,
            5587.65f,
            5919.91f,
            6271.93f,
            6644.88f,
            7040f,
            7458.62f,
            7902.13f,
            8372.02f,
            8869.84f,
            9397.27f,
            9956.06f,
            10548.1f,
            11175.3f,
            11839.8f,
            12543.9f,
        };
    }
}