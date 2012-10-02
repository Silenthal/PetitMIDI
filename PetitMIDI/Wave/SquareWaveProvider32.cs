namespace PetitMIDI.Wave
{
    using System;
    using NAudio.Wave;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class SquareWaveProvider32 : WaveProvider32
    {
        private int sample = 0;
        private const double period = 2 * Math.PI;

        public SquareWaveProvider32()
        {
            Frequency = 1000;
            Amplitude = 0.25f;
            Duty = .9f;
        }

        public float Frequency { get; set; }

        public float Amplitude { get; set; }

        public float Duty { get; set; }

        public override int Read(float[] buffer, int offset, int sampleCount)
        {
            int sampleRate = this.WaveFormat.SampleRate;
            float timePeriod = sampleRate / this.Frequency;
            float ratio = timePeriod * this.Duty;
            for (int n = 0; n < sampleCount; n++)
            {
                float samplePoint = sample % timePeriod;
                buffer[n + offset] = (float)(Amplitude * Math.Sign(ratio - samplePoint));
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