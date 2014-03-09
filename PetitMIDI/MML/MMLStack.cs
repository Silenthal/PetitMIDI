namespace PetitMIDI.MML
{
	using System.Collections.Generic;
	using System.Text;
	using System.Text.RegularExpressions;
	using PetitMIDI.MML.Event;

	/// <summary>
	/// A stack for MML evaluation.
	/// </summary>
	public class MMLStack
	{
		private static int[] noteVal = { 9, 11, 0, 2, 4, 5, 7 };

		/// <summary>
		/// The contents of the stack.
		/// </summary>
		private StringBuilder stackContents;

		private static Regex macroRegex = new Regex(@"\{([A-Z][A-Z0-9_]*)=([^\{\}]+)\}", RegexOptions.Compiled);

		private static Regex loopRegex = new Regex(@"\[([^\[\]]+)\]([0-9]+)", RegexOptions.Compiled);

		/// <summary>
		/// A pointer to the current top of the stack.
		/// </summary>
		private int currentMark;

		/// <summary>
		/// The character to return for an empty stack pop.
		/// </summary>
		private char endCommand = '\0';

		/// <summary>
		/// Initilaizes a new instance of the <see cref="MMLStack"/> class.
		/// </summary>
		public MMLStack()
		{
			this.currentMark = 0;
			this.stackContents = new StringBuilder();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MMLStack"/> class.
		/// </summary>
		/// <param name="s">The string to initialize the stack with.</param>
		public MMLStack(string s)
		{
			this.currentMark = 0;
			this.stackContents = new StringBuilder(s);
		}

		/// <summary>
		/// Gets a value indicating the size of the stack.
		/// </summary>
		public int Size
		{
			get
			{
				return this.stackContents.Length - this.currentMark;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the stack is empty.
		/// </summary>
		public bool IsEmpty
		{
			get
			{
				return this.Size == 0;
			}
		}

		/// <summary>
		/// Clears the stack, deleting all contents.
		/// </summary>
		public void Clear()
		{
			this.Refresh();
			this.stackContents.Clear();
		}

		/// <summary>
		/// Processes an MML string, replacing macros and multiloops.
		/// </summary>
		/// <param name="mml">The MML to modify.</param>
		/// <returns>A clean MML string.</returns>
		public static string PreProcessMML(string mml)
		{
			StringBuilder fst = new StringBuilder(mml.ToUpper());
			fst = fst.Replace(" ", "");
			fst = fst.Replace("{}", "");
			fst = fst.Replace("[]", "");

			//First, check that all groups are closed properly.
			Stack<char> fStack = new Stack<char>();
			bool good = true;
			for (int i = 0; i < fst.Length; i++)
			{
				if (fst[i] == '[' || fst[i] == '{')
				{
					fStack.Push(fst[i]);
				}
				else if (fst[i] == ']')
				{
					if (fStack.Count > 0 && fStack.Peek() == '[')
					{
						fStack.Pop();
					}
					else
					{
						good = false;
						break;
					}
				}
				else if (fst[i] == '}')
				{
					if (fStack.Count > 0 && fStack.Peek() == '{')
					{
						fStack.Pop();
					}
					else
					{
						good = false;
						break;
					}
				}
			}

			if (fStack.Count > 0 || !good)
			{
				return "";
			}

			//Then, replace macro definitions.
			while (macroRegex.IsMatch(fst.ToString()))
			{
				var mfs = macroRegex.Match(fst.ToString()).Groups;
				fst = fst.Replace(mfs[0].Value, "");
				fst = fst.Replace("{" + mfs[1].Value + "}", mfs[2].Value);
			}

			//Then, replace multiloops.
			while (loopRegex.IsMatch(fst.ToString()))
			{
				var mfs = loopRegex.Match(fst.ToString()).Groups;
				int startIndex = mfs[0].Index;
				fst = fst.Remove(startIndex, mfs[0].Length);

				//Copy the resultant MML (number) times.
				for (int i = 0; i < int.Parse(mfs[2].Value); i++)
				{
					fst = fst.Insert(startIndex, mfs[1].Value);
				}
			}

			return fst.ToString();
		}

		/// <summary>
		/// Pops a character or number from the stack.
		/// </summary>
		/// <returns>The number or character at the top of the stack, or an empty string if the stack is empty.</returns>
		public MMLEvent PopEvent()
		{
			if (this.Size == 0)
			{
				return new NoneEvent();
			}

			StringBuilder sb = new StringBuilder();
			int popCount = 0;
			if (this.PeekChar() == '[')
			{
				this.PopChar();
				popCount++;
			}
			else if (this.PeekChar() == ']')
			{
				while (this.PeekChar() != '[')
				{
					this.UndoPop();
				}
				this.PopChar();
			}
			switch (this.PeekChar())
			{
				case 'V': // Velocity
					{
						this.PopChar();
						if (char.IsDigit(this.PeekChar()))
						{
							return new VelocityEvent(int.Parse(this.PopNumber()));
						}
						else
						{
							return new InvalidEvent();
						}
					}

				case '@':
					{
						this.PopChar();
						if (char.IsDigit(this.PeekChar())) // Instrument
						{
							return new InstrumentEvent(int.Parse(this.PopNumber()));
						}
						else if (this.PeekChar() == 'D') // Detune
						{
							this.PopChar();
							if (char.IsDigit(this.PeekChar()))
							{
								return new DetuneEvent(int.Parse(this.PopNumber()));
							}
							else
							{
								return new InvalidEvent();
							}
						}
						else if (this.PeekChar() == 'V') // Volume
						{
							this.PopChar();
							if (char.IsDigit(this.PeekChar()))
							{
								return new VolumeEvent(int.Parse(this.PopNumber()));
							}
							else
							{
								return new InvalidEvent();
							}
						}
						else if (this.PeekChar() == 'E') // Envelope
						{
							this.PopChar();
							if (char.IsDigit(this.PeekChar()))
							{
								EnvelopeEvent e = new EnvelopeEvent(0, 0, 0, 0);
								bool goodParsing = true;
								int temp;
								goodParsing |= int.TryParse(this.PopNumber(), out temp);
								e.Attack = temp;
								goodParsing |= this.PopChar() == ',';
								goodParsing |= int.TryParse(this.PopNumber(), out temp);
								e.Decay = temp;
								goodParsing |= this.PopChar() == ',';
								goodParsing |= int.TryParse(this.PopNumber(), out temp);
								e.Sustain = temp;
								goodParsing |= this.PopChar() == ',';
								goodParsing |= int.TryParse(this.PopNumber(), out temp);
								e.Release = temp;
								if (goodParsing)
								{
									return e;
								}
								else
								{
									return new InvalidEvent();
								}
							}
							else if (this.PeekChar() == 'R') // Envelope Release
							{
								this.PopChar();
								return new EnvelopeReleaseEvent();
							}
							else
							{
								return new InvalidEvent();
							}
						}
						else if (this.PeekChar() == 'M')
						{
							this.PopChar();
							if (this.PeekChar() == 'O') // Modulation On/Off
							{
								this.PopChar();
								if (this.PeekChar() == 'N')
								{
									this.PopChar();
									return new ModulationEvent(true);
								}
								else if (this.PeekChar() == 'F')
								{
									this.PopChar();
									return new ModulationEvent(false);
								}
								else
								{
									return new InvalidEvent();
								}
							}
							else if (this.PeekChar() == 'A') // Tremolo
							{
								this.PopChar();
								if (char.IsDigit(this.PeekChar()))
								{
									TremoloEvent e = new TremoloEvent(0, 0, 0, 0);
									bool goodParsing = true;
									int temp;
									goodParsing |= int.TryParse(this.PopNumber(), out temp);
									e.Depth = temp;
									goodParsing |= this.PopChar() == ',';
									goodParsing |= int.TryParse(this.PopNumber(), out temp);
									e.Range = temp;
									goodParsing |= this.PopChar() == ',';
									goodParsing |= int.TryParse(this.PopNumber(), out temp);
									e.Speed = temp;
									goodParsing |= this.PopChar() == ',';
									goodParsing |= int.TryParse(this.PopNumber(), out temp);
									e.Delay = temp;
									if (goodParsing)
									{
										return e;
									}
									else
									{
										return new InvalidEvent();
									}
								}
								else
								{
									return new InvalidEvent();
								}
							}
							else if (this.PeekChar() == 'P') // Vibrato
							{
								this.PopChar();
								if (char.IsDigit(this.PeekChar()))
								{
									VibratoEvent e = new VibratoEvent(0, 0, 0, 0);
									bool goodParsing = true;
									int temp;
									goodParsing |= int.TryParse(this.PopNumber(), out temp);
									e.Depth = temp;
									goodParsing |= this.PopChar() == ',';
									goodParsing |= int.TryParse(this.PopNumber(), out temp);
									e.Range = temp;
									goodParsing |= this.PopChar() == ',';
									goodParsing |= int.TryParse(this.PopNumber(), out temp);
									e.Speed = temp;
									goodParsing |= this.PopChar() == ',';
									goodParsing |= int.TryParse(this.PopNumber(), out temp);
									e.Delay = temp;
									if (goodParsing)
									{
										return e;
									}
									else
									{
										return new InvalidEvent();
									}
								}
								else
								{
									return new InvalidEvent();
								}
							}
							else
							{
								return new InvalidEvent();
							}
						}
						else
						{
							return new InvalidEvent();
						}
					}

				case 'L': // Note Length
					{
						this.PopChar();
						if (char.IsDigit(this.PeekChar()))
						{
							return new LengthEvent(int.Parse(this.PopNumber()));
						}
						else
						{
							return new InvalidEvent();
						}
					}

				case 'O': // Octave
					{
						this.PopChar();
						if (char.IsDigit(this.PeekChar()))
						{
							return new OctaveEvent(int.Parse(this.PopNumber()));
						}
						else
						{
							return new InvalidEvent();
						}
					}

				case 'P': // Pan
					{
						this.PopChar();
						if (char.IsDigit(this.PeekChar()))
						{
							return new PanEvent(int.Parse(this.PopNumber()));
						}
						else
						{
							return new InvalidEvent();
						}
					}

				case 'T': // Tempo
					{
						this.PopChar();
						if (char.IsDigit(this.PeekChar()))
						{
							return new TempoEvent(int.Parse(this.PopNumber()));
						}
						else
						{
							return new InvalidEvent();
						}
					}

				case '<': // Octave Increase
					{
						this.PopChar();
						return new OctaveIncreaseEvent();
					}

				case '>': // Octave Decrease
					{
						this.PopChar();
						return new OctaveDecreaseEvent();
					}

				case '(': // VelocityIncrease
					{
						this.PopChar();
						if (char.IsDigit(this.PeekChar()))
						{
							return new VelocityIncreaseEvent(int.Parse(this.PopNumber()));
						}
						else
						{
							return new InvalidEvent();
						}
					}

				case ')': // VelocityIncrease
					{
						this.PopChar();
						if (char.IsDigit(this.PeekChar()))
						{
							return new VelocityDecreaseEvent(int.Parse(this.PopNumber()));
						}
						else
						{
							return new InvalidEvent();
						}
					}

				case 'R': // Rest
					{
						this.PopChar();
						RestEvent re = new RestEvent();
						int dotCount = 0;
						bool noteLenSet = false;
						while (true)
						{
							if (char.IsDigit(this.PeekChar()))//A length value.
							{
								if (!noteLenSet)
								{
									re.NoteValue = int.Parse(this.PopNumber());
									noteLenSet = true;
								}
								else
								{
									return new InvalidEvent();
								}
							}
							else if (this.PeekChar() == '.')
							{
								this.PopChar();
								if (dotCount++ < 2)
								{
									re.Multiplier *= 1.5;
								}
							}
							else
							{
								break;
							}
						}

						return re;
					}

				case 'N': // Note
					{
						this.PopChar();
						string pc =  "";
						int conv = 0;
						while (char.IsDigit(this.PeekChar()))
						{
							pc += this.PopChar();
						}
						if (pc.Length == 0 || !int.TryParse(pc, out conv))
						{
							return new InvalidEvent();
						}
						else
						{
							return new PitchEvent(conv);
						}
					}

				case 'C':
				case 'D':
				case 'E':
				case 'F':
				case 'G':
				case 'A':
				case 'B':
					{
						NoteEvent ne = new NoteEvent(noteVal[this.PopChar() - 'A']);
						int dotCount = 0;
						bool noteLenSet = false;
						while (true)
						{
							if (char.IsDigit(this.PeekChar()))
							{
								if (!noteLenSet)
								{
									ne.NoteValue = int.Parse(this.PopNumber());
									noteLenSet = true;
								}
								else
								{
									return new InvalidEvent();
								}
							}
							else if (this.PeekChar() == '.')
							{
								this.PopChar();
								if (dotCount++ < 2)
								{
									ne.Multiplier *= 1.5;
								}
							}
							else if (this.PeekChar() == '+' || this.PeekChar() == '#')
							{
								this.PopChar();
								ne.BaseNote++;
								if (ne.BaseNote > 127)
								{
									return new InvalidEvent();
								}
							}
							else if (this.PeekChar() == '-')
							{
								this.PopChar();
								ne.BaseNote--;
								if (ne.BaseNote < 0)
								{
									return new InvalidEvent();
								}
							}
							else
							{
								break;
							}
						}

						return ne;
					}

				default:
					return new NoneEvent();
			}
		}

		/// <summary>
		/// Pops a number string from the stack.
		/// </summary>
		/// <returns></returns>
		private string PopNumber()
		{
			if (!this.IsEmpty && char.IsDigit(this.PeekChar()))
			{
				StringBuilder sb = new StringBuilder();
				while (char.IsDigit(this.PeekChar()))
				{
					sb.Append(this.PopChar());
				}

				return sb.ToString();
			}
			else
			{
				return "";
			}
		}

		/// <summary>
		/// Resets the stack to its full state.
		/// </summary>
		public void Refresh()
		{
			this.currentMark = 0;
		}

		/// <summary>
		/// Pushes a new MML string to the bottom of the stack.
		/// </summary>
		/// <param name="extraMML">The new MML string.</param>
		public void PushBack(string extraMML)
		{
			this.stackContents.Append(extraMML);
		}

		/// <summary>
		/// Undoes a single pop command.
		/// </summary>
		public void UndoPop()
		{
			if (--currentMark < 0)
			{
				currentMark = 0;
			}
		}

		/// <summary>
		/// Peeks at the character at the top of the stack.
		/// </summary>
		/// <returns>The character at the top of the stack, or a null character if the stack is empty.</returns>
		private char PeekChar()
		{
			if (this.IsEmpty)
			{
				return this.endCommand;
			}

			return this.stackContents[this.currentMark];
		}

		/// <summary>
		/// Pops a character from the top of the stack.
		/// </summary>
		/// <returns>The character at the top of the stack, or a null character if the stack is empty.</returns>
		private char PopChar()
		{
			if (this.IsEmpty)
			{
				return this.endCommand;
			}

			return this.stackContents[this.currentMark++];
		}
	}
}