namespace PetitMIDI.MML
{
    using PetitMIDI.MML.Event;
    using System.Collections.Generic;
    using System.Text;
    using System.Text.RegularExpressions;

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
            currentMark = 0;
            stackContents = new StringBuilder();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MMLStack"/> class.
        /// </summary>
        /// <param name="s">The string to initialize the stack with.</param>
        public MMLStack(string s)
        {
            currentMark = 0;
            stackContents = new StringBuilder(s);
        }

        /// <summary>
        /// Gets a value indicating the size of the stack.
        /// </summary>
        public int Size
        {
            get
            {
                return stackContents.Length - currentMark;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the stack is empty.
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return Size == 0;
            }
        }

        /// <summary>
        /// Clears the stack, deleting all contents.
        /// </summary>
        public void Clear()
        {
            Refresh();
            stackContents.Clear();
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
            if (Size == 0)
            {
                return new NoneEvent();
            }

            StringBuilder sb = new StringBuilder();
            int popCount = 0;
            if (PeekChar() == '[')
            {
                PopChar();
                popCount++;
            }
            else if (PeekChar() == ']')
            {
                while (PeekChar() != '[')
                {
                    UndoPop();
                }
                PopChar();
            }
            switch (PeekChar())
            {
                case 'V': // Velocity
                    {
                        PopChar();
                        if (char.IsDigit(PeekChar()))
                        {
                            return new VelocityEvent(int.Parse(PopNumber()));
                        }
                        else
                        {
                            return new InvalidEvent();
                        }
                    }

                case '@':
                    {
                        PopChar();
                        if (char.IsDigit(PeekChar())) // Instrument
                        {
                            return new InstrumentEvent(int.Parse(PopNumber()));
                        }
                        else if (PeekChar() == 'D') // Detune
                        {
                            PopChar();
                            if (char.IsDigit(PeekChar()))
                            {
                                return new DetuneEvent(int.Parse(PopNumber()));
                            }
                            else
                            {
                                return new InvalidEvent();
                            }
                        }
                        else if (PeekChar() == 'V') // Volume
                        {
                            PopChar();
                            if (char.IsDigit(PeekChar()))
                            {
                                return new VolumeEvent(int.Parse(PopNumber()));
                            }
                            else
                            {
                                return new InvalidEvent();
                            }
                        }
                        else if (PeekChar() == 'E') // Envelope
                        {
                            PopChar();
                            if (char.IsDigit(PeekChar()))
                            {
                                EnvelopeEvent e = new EnvelopeEvent(0, 0, 0, 0);
                                bool goodParsing = true;
                                int temp;
                                goodParsing |= int.TryParse(PopNumber(), out temp);
                                e.Attack = temp;
                                goodParsing |= PopChar() == ',';
                                goodParsing |= int.TryParse(PopNumber(), out temp);
                                e.Decay = temp;
                                goodParsing |= PopChar() == ',';
                                goodParsing |= int.TryParse(PopNumber(), out temp);
                                e.Sustain = temp;
                                goodParsing |= PopChar() == ',';
                                goodParsing |= int.TryParse(PopNumber(), out temp);
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
                            else if (PeekChar() == 'R') // Envelope Release
                            {
                                PopChar();
                                return new EnvelopeReleaseEvent();
                            }
                            else
                            {
                                return new InvalidEvent();
                            }
                        }
                        else if (PeekChar() == 'M')
                        {
                            PopChar();
                            if (PeekChar() == 'O') // Modulation On/Off
                            {
                                PopChar();
                                if (PeekChar() == 'N')
                                {
                                    PopChar();
                                    return new ModulationEvent(true);
                                }
                                else if (PeekChar() == 'F')
                                {
                                    PopChar();
                                    return new ModulationEvent(false);
                                }
                                else
                                {
                                    return new InvalidEvent();
                                }
                            }
                            else if (PeekChar() == 'A') // Tremolo
                            {
                                PopChar();
                                if (char.IsDigit(PeekChar()))
                                {
                                    TremoloEvent e = new TremoloEvent(0, 0, 0, 0);
                                    bool goodParsing = true;
                                    int temp;
                                    goodParsing |= int.TryParse(PopNumber(), out temp);
                                    e.Depth = temp;
                                    goodParsing |= PopChar() == ',';
                                    goodParsing |= int.TryParse(PopNumber(), out temp);
                                    e.Range = temp;
                                    goodParsing |= PopChar() == ',';
                                    goodParsing |= int.TryParse(PopNumber(), out temp);
                                    e.Speed = temp;
                                    goodParsing |= PopChar() == ',';
                                    goodParsing |= int.TryParse(PopNumber(), out temp);
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
                            else if (PeekChar() == 'P') // Vibrato
                            {
                                PopChar();
                                if (char.IsDigit(PeekChar()))
                                {
                                    VibratoEvent e = new VibratoEvent(0, 0, 0, 0);
                                    bool goodParsing = true;
                                    int temp;
                                    goodParsing |= int.TryParse(PopNumber(), out temp);
                                    e.Depth = temp;
                                    goodParsing |= PopChar() == ',';
                                    goodParsing |= int.TryParse(PopNumber(), out temp);
                                    e.Range = temp;
                                    goodParsing |= PopChar() == ',';
                                    goodParsing |= int.TryParse(PopNumber(), out temp);
                                    e.Speed = temp;
                                    goodParsing |= PopChar() == ',';
                                    goodParsing |= int.TryParse(PopNumber(), out temp);
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
                        PopChar();
                        if (char.IsDigit(PeekChar()))
                        {
                            return new LengthEvent(int.Parse(PopNumber()));
                        }
                        else
                        {
                            return new InvalidEvent();
                        }
                    }

                case 'O': // Octave
                    {
                        PopChar();
                        if (char.IsDigit(PeekChar()))
                        {
                            return new OctaveEvent(int.Parse(PopNumber()));
                        }
                        else
                        {
                            return new InvalidEvent();
                        }
                    }

                case 'P': // Pan
                    {
                        PopChar();
                        if (char.IsDigit(PeekChar()))
                        {
                            return new PanEvent(int.Parse(PopNumber()));
                        }
                        else
                        {
                            return new InvalidEvent();
                        }
                    }

                case 'T': // Tempo
                    {
                        PopChar();
                        if (char.IsDigit(PeekChar()))
                        {
                            int outNum = int.Parse(PopNumber());
                            if (outNum < 1 || outNum > 512)
                            {
                                return new InvalidEvent();
                            }
                            return new TempoEvent(outNum);
                        }
                        else
                        {
                            return new InvalidEvent();
                        }
                    }

                case '<': // Octave Increase
                    {
                        PopChar();
                        return new OctaveIncreaseEvent();
                    }

                case '>': // Octave Decrease
                    {
                        PopChar();
                        return new OctaveDecreaseEvent();
                    }

                case '(': // VelocityIncrease
                    {
                        PopChar();
                        if (char.IsDigit(PeekChar()))
                        {
                            return new VelocityIncreaseEvent(int.Parse(PopNumber()));
                        }
                        else
                        {
                            return new InvalidEvent();
                        }
                    }

                case ')': // VelocityIncrease
                    {
                        PopChar();
                        if (char.IsDigit(PeekChar()))
                        {
                            return new VelocityDecreaseEvent(int.Parse(PopNumber()));
                        }
                        else
                        {
                            return new InvalidEvent();
                        }
                    }

                case '&': // Tie
                    {
                        PopChar();
                        if (PeekChar() >= 'A' && PeekChar() <= 'G')
                        {
                            return new TieEvent();
                        }
                        else
                        {
                            return new InvalidEvent();
                        }
                    }

                case 'R': // Rest
                    {
                        PopChar();
                        RestEvent re = new RestEvent();
                        int dotCount = 0;
                        bool noteLenSet = false;
                        while (true)
                        {
                            if (char.IsDigit(PeekChar()))//A length value.
                            {
                                if (!noteLenSet)
                                {
                                    re.NoteValue = int.Parse(PopNumber());
                                    noteLenSet = true;
                                }
                                else
                                {
                                    return new InvalidEvent();
                                }
                            }
                            else if (PeekChar() == '.')
                            {
                                PopChar();
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
                        PopChar();
                        string pc = "";
                        int conv = 0;
                        while (char.IsDigit(PeekChar()))
                        {
                            pc += PopChar();
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
                        NoteEvent ne = new NoteEvent(noteVal[PopChar() - 'A']);
                        int dotCount = 0;
                        bool noteLenSet = false;
                        while (true)
                        {
                            if (char.IsDigit(PeekChar()))
                            {
                                if (!noteLenSet)
                                {
                                    ne.NoteValue = int.Parse(PopNumber());
                                    noteLenSet = true;
                                }
                                else
                                {
                                    return new InvalidEvent();
                                }
                            }
                            else if (PeekChar() == '.')
                            {
                                PopChar();
                                if (dotCount++ < 2)
                                {
                                    ne.Multiplier *= 1.5;
                                }
                            }
                            else if (PeekChar() == '+' || PeekChar() == '#')
                            {
                                PopChar();
                                ne.BaseNote++;
                                if (ne.BaseNote > 127)
                                {
                                    return new InvalidEvent();
                                }
                            }
                            else if (PeekChar() == '-')
                            {
                                PopChar();
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
            if (!IsEmpty && char.IsDigit(PeekChar()))
            {
                StringBuilder sb = new StringBuilder();
                while (char.IsDigit(PeekChar()))
                {
                    sb.Append(PopChar());
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
            currentMark = 0;
        }

        /// <summary>
        /// Pushes a new MML string to the bottom of the stack.
        /// </summary>
        /// <param name="extraMML">The new MML string.</param>
        public void PushBack(string extraMML)
        {
            stackContents.Append(extraMML);
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
            if (IsEmpty)
            {
                return endCommand;
            }

            return stackContents[currentMark];
        }

        /// <summary>
        /// Pops a character from the top of the stack.
        /// </summary>
        /// <returns>The character at the top of the stack, or a null character if the stack is empty.</returns>
        private char PopChar()
        {
            if (IsEmpty)
            {
                return endCommand;
            }

            return stackContents[currentMark++];
        }
    }
}