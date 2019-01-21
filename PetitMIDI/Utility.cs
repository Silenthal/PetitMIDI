namespace PetitMIDI
{
    public static class Utility
    {
        public static int Clamp(int input, int min, int max)
        {
            if (min < max)
            {
                return input < min ? min : input > max ? max : input;
            }
            else if (min == max)
            {
                return min;
            }
            else
            {
                return input < max ? max : input > min ? min : input;
            }
        }
    }
}