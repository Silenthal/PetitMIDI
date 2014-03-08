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
		private float frequency = 440f;
		private float ampScale = 0.08f;
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
		/// Represents the amplitude of the wave.
		/// </summary>
		public float Amplitude = 1f;

		/// <summary>
		/// Represents the velocity that the wave hits with.
		/// </summary>
		public float Velocity = 1f;

		/// <summary>
		/// Gets or sets the duty cycle of the generated square wave.
		/// </summary>
		public float Duty = 0.125f;

		private bool isGateActive = false;

		public WaveType GeneratorType = WaveType.Sine;

		public WaveGenerator(WaveType generatorType = WaveType.Sine)
		{
			GeneratorType = generatorType;
		}

		public void Gate(bool isActive)
		{
			isGateActive = isActive;
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
			float appliedAmplitude = Amplitude * Velocity * ampScale;
			int sampleRate = WaveFormat.SampleRate;
			float cycleTime = sampleRate / Frequency;
			float ratio = cycleTime * this.Duty;
			float currentSample = 0;
			for (int n = 0; n < sampleCount; n++)
			{
				switch (GeneratorType)
				{
					case WaveType.Square:
						currentSample = appliedAmplitude * (float)(Math.Sign(ratio - sample));
						break;

					case WaveType.WhiteNoise:
						currentSample = appliedAmplitude * (float)(2 * r.NextDouble() - 1);
						break;

					case WaveType.Sine:
					default:
						currentSample = appliedAmplitude * (float)(Math.Sin(2 * Math.PI * (sample / cycleTime)));
						break;
				}
				if (mixType == MixType.Overwrite)
				{
					if (isGateActive)
					{
						buffer[n + offset] = currentSample;
					}
					else
					{
						buffer[n + offset] = 0;
					}
				}
				else
				{
					if (isGateActive)
					{
						buffer[n + offset] += currentSample;
					}
					else
					{
						// Do nothing.
					}
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