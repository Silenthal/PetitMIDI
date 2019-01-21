namespace PetitMIDI
{
    public static class Config
    {
        public static bool IsOctaveReversed = false;
        public static float AmplitudeScale = 0.08f;

        public static class Tempo
        {
            public const int MinValue = 1;
            public const int MaxValue = 240;
            public static int Default = 120;
        }

        public static class Channel
        {
            public const int MinValue = 0;
            public static int MaxValue => Count - 1;
            public static int Count = 8;
        }

        public static class Volume
        {
            public const int MinValue = 0;
            public const int MaxValue = 127;
            public static int Default = MaxValue;
        }

        public static class Velocity
        {
            public const int MinValue = 0;
            public const int MaxValue = 127;
            public static int Default = MaxValue;
        }

        public static class Octave
        {
            public const int MinValue = 0;
            public const int MaxValue = 8;
            public static int Default = 5;
        }
    }
}