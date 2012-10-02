namespace PetitMIDI.Wave
{
    using System;
    using NAudio.Wave;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class SineWaveProvider32 : WaveProvider32
    {
        private int sample = 0;

        public SineWaveProvider32()
        {
            Frequency = 1000;
            Amplitude = 0.25f;
        }

        public float Frequency { get; set; }

        public float Amplitude { get; set; }

        public override int Read(float[] buffer, int offset, int sampleCount)
        {
            int sampleRate = WaveFormat.SampleRate;
            for (int n = 0; n < sampleCount; n++)
            {
                buffer[n + offset] = (float)(Amplitude * Math.Sin((2 * Math.PI * sample * Frequency) / sampleRate));
                sample++;
                if (sample >= sampleRate)
                {
                    sample = 0;
                }
            }
            return sampleCount;
        }
    }
}