namespace PetitMIDI
{
    public static class Config
    {
        public static bool IsOctaveReversed = false;
        public static float AmplitudeScale = 0.08f;

        public static class Tempo
        {
            public static int Default = 120;
        }

        public static class Channel
        {
            public static int Count = 8;
        }

        public static class Volume
        {
            public static int Default = Constants.Volume.MaxValue;
        }

        public static class Velocity
        {
            public static int Default = Constants.Velocity.MaxValue;
        }

        public static class Octave
        {
            public static int Default = 5;
        }
    }
}