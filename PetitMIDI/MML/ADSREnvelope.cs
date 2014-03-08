// Adapted from code from http://www.earlevel.com/main/2013/06/01/envelope-generators/
namespace PetitMIDI.MML
{
	using System;

	/// <summary>
	/// Represents the current state of the envelope.
	/// </summary>
	public enum ADSRState
	{
		Idle,
		Attack,
		Decay,
		Sustain,
		Release
	}

	/// <summary>
	/// Represents an amplitude based ADSR envelope.
	/// </summary>
	public class ADSREnvelope
	{
		/// <summary>
		/// Represents the last output of the envelope.
		/// </summary>
		public float Output { get; private set; }

		private ADSRState state = ADSRState.Idle;

		private float attackRate;
		private float decayRate;
		private float releaseRate;
		private float attackCoef;
		private float decayCoef;
		private float releaseCoef;
		private float sustainLevel;
		private float targetRatioA;
		private float targetRatioDR;
		private float attackBase;
		private float decayBase;
		private float releaseBase;
		private int cyclesActive = 0;
		private int activeAtGate = 0;
		private int activeAtRelease = 0;
		public ADSREnvelope(int sampleRate)
		{
			Reset();
			SetAttack(0.5f, sampleRate);
			SetDecay(0.5f, sampleRate);
			SetRelease(0.5f, sampleRate);
			SetSustain(1.0f);
			SetTargetRatioA(0.3f);
			SetTargetRatioDR(0.0001f);
		}

		/// <summary>
		/// Advances the internal state of the envelope, and returns the value to apply to the signal.
		/// </summary>
		/// <returns>The value on the ADSR curve to apply to the signal.</returns>
		public float Process()
		{
			if (state == ADSRState.Attack)
			{
				cyclesActive++;
				return 1f;
			}
			else
			{
				return 0f;
			}
			/*
			switch (state)
			{
				case ADSRState.Idle:
					break;

				case ADSRState.Attack:
					Output = attackBase + Output * attackCoef;
					if (Output >= 1.0f)
					{
						Output = 1.0f;
						state = ADSRState.Decay;
					}
					break;

				case ADSRState.Decay:
					Output = decayBase + Output * decayCoef;
					if (Output <= sustainLevel)
					{
						Output = sustainLevel;
						state = ADSRState.Sustain;
					}
					break;

				case ADSRState.Sustain:
					break;

				case ADSRState.Release:
					Output = releaseBase + Output * releaseCoef;
					if (Output <= 0.0f)
					{
						Output = 0.0f;
						state = ADSRState.Idle;
					}
					break;
			}
			return Output;
			*/
		}

		public ADSRState GetCurrentState()
		{
			return state;
		}

		public void Gate(bool isActive)
		{
			if (isActive)
			{
				state = ADSRState.Attack;
				activeAtGate = cyclesActive;
			}
			else if (state != ADSRState.Idle)
			{
				state = ADSRState.Release;
				activeAtRelease = cyclesActive;
			}
		}

		public void SetAttack(float time, float sampleRate)
		{
			attackRate = time * sampleRate;
			attackCoef = calcCoef(attackRate, targetRatioA);
			attackBase = (float)((1.0 + targetRatioA) * (1.0 - attackCoef));
		}

		public void SetDecay(float time, float sampleRate)
		{
			decayRate = time * sampleRate;
			decayCoef = calcCoef(decayRate, targetRatioDR);
			decayBase = (float)((sustainLevel - targetRatioDR) * (1.0 - decayCoef));
		}

		public void SetRelease(float time, float sampleRate)
		{
			releaseRate = time * sampleRate;
			releaseCoef = calcCoef(releaseRate, targetRatioDR);
			releaseBase = (float)(-targetRatioDR * (1.0 - releaseCoef));
		}

		public void SetSustain(float level)
		{
			sustainLevel = level;
			decayBase = (float)((sustainLevel - targetRatioDR) * (1.0 - decayCoef));
		}

		public void SetTargetDecibelA(float decibels)
		{
			SetTargetRatioA((float)Math.Pow(10, decibels / 20));
		}

		public void SetTargetDecibelDR(float decibels)
		{
			SetTargetRatioDR((float)Math.Pow(10, decibels / 20));
		}

		public void SetTargetRatioA(float targetRatio)
		{
			if (targetRatio < 0.000000001f)
				targetRatio = 0.000000001f;  // -180 dB
			targetRatioA = targetRatio;
			attackBase = (float)((1.0 + targetRatioA) * (1.0 - attackCoef));
		}

		public void SetTargetRatioDR(float targetRatio)
		{
			if (targetRatio < 0.000000001f)
				targetRatio = 0.000000001f;  // -180 dB
			targetRatioDR = targetRatio;
			decayBase = (float)((sustainLevel - targetRatioDR) * (1.0 - decayCoef));
			releaseBase = (float)(-targetRatioDR * (1.0 - releaseCoef));
		}

		public void Reset()
		{
			state = ADSRState.Idle;
			Output = 0.0f;
		}

		private float calcCoef(float rate, float targetRatio)
		{
			return (float)Math.Exp(-Math.Log((1.0 + targetRatio) / targetRatio) / rate);
		}
	}
}