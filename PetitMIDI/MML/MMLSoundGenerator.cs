namespace PetitMIDI.MML
{
	using NAudio.CoreAudioApi;
	using NAudio.Midi;
	using NAudio.Wave;
	using PetitMIDI.Wave;

	public enum NoteStyle
	{
		Regular,
		Drums,
		PSG,
		Noise
	}

	/// <summary>
	/// Represents the sound creator for MML.
	/// </summary>
	public class MMLSoundGenerator
	{
		private MidiOut midiOut = new MidiOut(0);

		private WasapiOut waveOut = new WasapiOut(AudioClientShareMode.Shared, 3);

		private MixProvider mixer = new MixProvider();

		private NoteStyle[] noteMode = new NoteStyle[8];

		public MMLSoundGenerator()
		{
			this.mixer.SetWaveFormat(44100, 1);
			Open(0);
		}

		public void Open(int deviceID = 0)
		{
			Close();
			for (int i = 0; i < noteMode.Length; i++)
			{
				noteMode[i] = NoteStyle.Regular;
			}
			this.midiOut = new MidiOut(deviceID);
			this.waveOut = new WasapiOut(AudioClientShareMode.Shared, 5);
			this.waveOut.Init(this.mixer);
			this.waveOut.Play();
		}

		public void Close()
		{
			MIDIMessage ms = new MIDIMessage();
			ms.ControlType = ControlChangeType.AllNotesOff;
			this.midiOut.Send(ms.RawData);
			this.midiOut.Close();
			for (int i = 0; i < 8; i++)
			{
				this.mixer.Gate(i, false);
			}
			this.waveOut.Stop();
		}

		public void ChangeMode(int channel, NoteStyle mode)
		{
			if (channel >= 0 && channel < 8)
			{
				noteMode[channel] = mode;
				if (mode == NoteStyle.Noise)
				{
					this.mixer.SetGeneratorType(channel, WaveType.WhiteNoise);
				}
				else
				{
					this.mixer.SetGeneratorType(channel, WaveType.Square);
				}
			}
		}

		public void EnableEnvelope(int channel, int attack, int decay, int sustain, int release)
		{
			if (noteMode[channel] == NoteStyle.Regular || noteMode[channel] == NoteStyle.Drums)
			{
				// TODO: Not sure how to handle sustain level just yet.
				MIDIMessage message = new MIDIMessage();
				message.Channel = channel;
				message.Status = MessageType.ControlChange;
				message.ControlType = ControlChangeType.SoundController04; // Attack
				message.ControlValue = attack;
				this.midiOut.Send(message.RawData);
				message.ControlType = ControlChangeType.SoundController06; // Decay
				message.ControlValue = decay;
				this.midiOut.Send(message.RawData);
				message.ControlType = ControlChangeType.SoundController03; // Release
				message.ControlValue = release;
				this.midiOut.Send(message.RawData);
			}
			else
			{
				this.mixer.SetEnvelope(channel, attack, decay, sustain, release);
			}
		}

		public void Send(MIDIMessage message)
		{
			if (noteMode[message.Channel] == NoteStyle.Regular || noteMode[message.Channel] == NoteStyle.Drums)
			{
				this.midiOut.Send(message.RawData);
			}
			else
			{
				switch (message.Status)
				{
					case MessageType.NoteOn:
						{
							this.mixer.SetFrequency(message.Channel, this.frequencyTable[message.Data1]);
							this.mixer.SetVelocity(message.Channel, message.Data2 / 127.0f);
							this.mixer.Gate(message.Channel, true);
						}
						break;

					case MessageType.NoteOff:
						{
							this.mixer.Gate(message.Channel, false);
						}
						break;

					case MessageType.ControlChange:
						{
							switch (message.ControlType)
							{
								case ControlChangeType.ChannelVolume:
									{
										float adjustedAmplitude = message.ControlValue / 127.0f;
										mixer.SetAmplitude(message.Channel, adjustedAmplitude);
									}
									break;
								default:
									break;
							}
						}
						break;
				}
			}
		}

		public void ChangeDuty(int channel, float newDuty)
		{
			this.mixer.SetDuty(channel, newDuty);
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