using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NAudio.Wave;

namespace PetitMIDI.Wave
{
	public enum WaveType
	{
		Sine,
		Square,
		WhiteNoise
	}

	/// <summary>
	/// A class for providing 32-bit samples of a specified type of wave.
	/// </summary>
	public class WaveGenerator : WaveProvider32
	{
		private int sample = 0;
		private float frequency = 440;
		private Random r = new Random();

		/// <summary>
		/// The frequency of the wave.
		/// </summary>
		public float Frequency
		{
			get
			{
				return frequency;
			}
			set
			{
				sample = (int)((sample * frequency) / value);
				frequency = value;
			}
		}

		/// <summary>
		/// The amplitude of the wave.
		/// </summary>
		public float Amplitude { get; set; }

		/// <summary>
		/// Gets or sets the duty cycle of the generated square wave.
		/// </summary>
		public float Duty { get; set; }

		public WaveType GeneratorType;

		public WaveGenerator(WaveType generatorType = WaveType.Sine)
		{
			GeneratorType = generatorType;
			Amplitude = 0.25f;
		}

		/// <summary>
		/// Fill the specified buffer with wave data.
		/// </summary>
		/// <param name="buffer">The buffer to fill with wave data.</param>
		/// <param name="offset">The offset into the specified buffer.</param>
		/// <param name="sampleCount">The number of samples to read.</param>
		/// <returns>The number of samples written to the buffer.</returns>
		public override int Read(float[] buffer, int offset, int sampleCount)
		{
			int sampleRate = WaveFormat.SampleRate;
			float cycleTime = sampleRate / Frequency;
			float ratio = cycleTime * this.Duty;
			for (int n = 0; n < sampleCount; n++)
			{
				switch (GeneratorType)
				{
					case WaveType.Square:
						buffer[n + offset] = (float)(Amplitude * Math.Sign(ratio - sample));
						break;
					case WaveType.WhiteNoise:
						buffer[n + offset] = (float)(2 * r.NextDouble() - 1);
						break;
					case WaveType.Sine:
					default:
						buffer[n + offset] = (float)(Amplitude * Math.Sin(2 * Math.PI * (sample / cycleTime)));
						break;
				}
				sample++;
				if (sample > cycleTime)
				{
					sample = (int)(sample - cycleTime + 0.5f);
				}
			}
			return sampleCount;
		}
	}
}
