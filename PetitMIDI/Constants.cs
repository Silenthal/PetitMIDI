namespace PetitMIDI
{
    public class Constants
    {
        public static class Note
        {
            public const int MinValue = 0;
            public const int MaxValue = 127;
        }

        public static class Pan
        {
            public const int MinValue = 0;
            public const int MaxValue = 127;
        }

        public static class NoteLength
        {
            public const int MinValue = 1;
            public const int MaxValue = 192;
        }

        public static class Detune
        {
            public const int MinValue = 0;
            public const int MaxValue = 127;
        }

        public static class Instrument
        {
            public const int MinValue = 0;
            public const int MaxValue = 255;
        }

        public static class Tempo
        {
            public const int MinValue = 1;
            public const int MaxValue = 512;
        }

        public static class Volume
        {
            public const int MinValue = 0;
            public const int MaxValue = 127;
        }

        public static class Velocity
        {
            public const int MinValue = 0;
            public const int MaxValue = 127;
        }

        public static class Octave
        {
            public const int MinValue = 0;
            public const int MaxValue = 8;
        }
    }
}