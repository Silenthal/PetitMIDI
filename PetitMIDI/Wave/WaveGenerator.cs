using NAudio.Wave;
using System;

namespace PetitMIDI.Wave
{
	public enum WaveType
	{
		Sine,
		Square,
		WhiteNoise
	}

	public enum MixType
	{
		Overwrite,
		Mix
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

		public bool IsEnabled = false;

		public WaveType GeneratorType;

		public WaveGenerator(WaveType generatorType = WaveType.Sine)
		{
			GeneratorType = generatorType;
			Amplitude = 0.25f;
		}

		/// <summary>
		/// Fills the specified buffer with wave data.
		/// </summary>
		/// <param name="buffer">The buffer to fill with wave data.</param>
		/// <param name="offset">The offset into the specified buffer.</param>
		/// <param name="sampleCount">The number of samples to read.</param>
		/// <returns>The number of samples written to the buffer.</returns>
		public override int Read(float[] buffer, int offset, int sampleCount)
		{
			return Read(buffer, offset, sampleCount, MixType.Overwrite);
		}

		/// <summary>
		/// Fills the specified buffer with wave data.
		/// </summary>
		/// <param name="buffer">The buffer to fill with wave data.</param>
		/// <param name="offset">The offset into the specified buffer.</param>
		/// <param name="sampleCount">The number of samples to read.</param>
		/// <param name="mixType">The type of mixing to use when outputting.</param>
		/// <returns>The number of samples written to the buffer.</returns>
		public int Read(float[] buffer, int offset, int sampleCount, MixType mixType)
		{
			if (!IsEnabled)
			{
				for (int i = 0; i < sampleCount; i++)
				{
					buffer[i + offset] = 0;
				}
				return sampleCount;
			}
			int sampleRate = WaveFormat.SampleRate;
			float cycleTime = sampleRate / Frequency;
			float ratio = cycleTime * this.Duty;
			for (int n = 0; n < sampleCount; n++)
			{
				switch (GeneratorType)
				{
					case WaveType.Square:
						buffer[n + offset] = (mixType == MixType.Overwrite ? 0 : buffer[n + offset]) + (float)(Amplitude * Math.Sign(ratio - sample));
						break;

					case WaveType.WhiteNoise:
						buffer[n + offset] = (mixType == MixType.Overwrite ? 0 : buffer[n + offset]) + (float)(2 * r.NextDouble() - 1);
						break;

					case WaveType.Sine:
					default:
						buffer[n + offset] = (mixType == MixType.Overwrite ? 0 : buffer[n + offset]) + (float)(Amplitude * Math.Sin(2 * Math.PI * (sample / cycleTime)));
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