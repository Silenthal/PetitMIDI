﻿namespace PetitMIDI.Audio
{
    using NAudio.Wave;

    public class MixProvider : WaveProvider32
    {
        private WaveGenerator[] genArr;

        public MixProvider(int channelCount)
        {
            if (channelCount < 1)
            {
                channelCount = 1;
            }
            if (channelCount > Config.Channel.Count)
            {
                channelCount = Config.Channel.Count;
            }
            genArr = new WaveGenerator[channelCount];
            for (int i = 0; i < genArr.Length; i++)
            {
                genArr[i] = new WaveGenerator(WaveType.Square);
            }
        }

        public new void SetWaveFormat(int sampleRate, int channels)
        {
            for (int i = 0; i < genArr.Length; i++)
            {
                genArr[i].SetWaveFormat(sampleRate, channels);
            }
            base.SetWaveFormat(sampleRate, channels);
        }

        public float GetAmplitude(int channel)
        {
            if (channel >= 0 && channel < genArr.Length)
            {
                return genArr[channel].Amplitude;
            }
            else return 0;
        }

        public void SetAmplitude(int channel, float ampVal)
        {
            if (channel >= 0 && channel < genArr.Length)
            {
                genArr[channel].Amplitude = ampVal;
            }
        }

        public float GetVelocity(int channel)
        {
            if (channel >= 0 && channel < genArr.Length)
            {
                return genArr[channel].Velocity;
            }
            else return 0;
        }

        public void SetVelocity(int channel, float velVal)
        {
            if (channel >= 0 && channel < genArr.Length)
            {
                genArr[channel].Velocity = velVal;
            }
        }

        public float GetDuty(int channel)
        {
            if (channel >= 0 && channel < genArr.Length)
            {
                return genArr[channel].Duty;
            }
            else return 0;
        }

        public void SetDuty(int channel, float dutyVal)
        {
            if (channel >= 0 && channel < genArr.Length)
            {
                genArr[channel].Duty = dutyVal;
            }
        }

        public float GetFrequency(int channel)
        {
            if (channel >= 0 && channel < genArr.Length)
            {
                return genArr[channel].Frequency;
            }
            else return 0;
        }

        public void SetFrequency(int channel, float freq)
        {
            if (channel >= 0 && channel < genArr.Length)
            {
                genArr[channel].Frequency = freq;
            }
        }

        public WaveType GetGeneratorType(int channel)
        {
            if (channel >= 0 && channel < genArr.Length)
            {
                return genArr[channel].GeneratorType;
            }
            else return WaveType.Square;
        }

        public void SetGeneratorType(int channel, WaveType waveVal)
        {
            if (channel >= 0 && channel < genArr.Length)
            {
                genArr[channel].GeneratorType = waveVal;
            }
        }

        public void SetEnvelope(int channel, int attack, int delay, int sustain, int release)
        {
            if (channel >= 0 && channel < genArr.Length)
            {
                genArr[channel].SetEnvelope(attack, delay, sustain, release);
            }
        }

        public void Gate(int channel, bool gateVal)
        {
            if (channel >= 0 && channel < genArr.Length)
            {
                genArr[channel].Gate(gateVal);
            }
        }

        public override int Read(float[] buffer, int offset, int sampleCount)
        {
            for (int n = 0; n < sampleCount; n++)
            {
                buffer[n + offset] = 0;
            }
            for (int n = 0; n < genArr.Length; n++)
            {
                genArr[n].Read(buffer, offset, sampleCount, MixType.Mix);
            }
            return sampleCount;
        }
    }
}