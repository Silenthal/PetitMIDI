namespace PetitMIDI
{
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Security;

    /// <summary>
    /// A high resolution timer.
    /// </summary>
    public class HighResTimer
    {
        /// <summary>
        /// Represents the start time, in ticks.
        /// </summary>
        private long startTime;

        /// <summary>
        /// Represents the frequency of the processor's timer.
        /// </summary>
        private long frequency;

        /// <summary>
        /// Initializes a new instance of the <see cref="HighResTimer"/> class.
        /// </summary>
        public HighResTimer()
        {
            this.startTime = 0;

            if (!UnsafeNativeMethods.QueryPerformanceFrequency(out this.frequency))
            {
                throw new Win32Exception();
            }
        }

        /// <summary>
        /// Starts the timer.
        /// </summary>
        public void Start()
        {
            UnsafeNativeMethods.QueryPerformanceCounter(out this.startTime);
        }

        /// <summary>
        /// Returns the elapsed time, in seconds.
        /// </summary>
        /// <returns>The time elapsed since the start time.</returns>
        public double ElapsedTime()
        {
            long tempTime = 0;
            UnsafeNativeMethods.QueryPerformanceCounter(out tempTime);
            return (tempTime - this.startTime) / (double)this.frequency;
        }

        /// <summary>
        /// Encapsulates P/Invoke methods used by this class.
        /// </summary>
        private static class UnsafeNativeMethods
        {
            /// <summary>
            /// Retrieves the current value of the high-resolution performance counter.
            /// </summary>
            /// <param name="lpPerformanceCount">A pointer to a variable that receives the current performance-counter value, in counts.</param>
            /// <returns>If the function succeeds, the return value is nonzero.</returns>
            [SuppressUnmanagedCodeSecurity]
            [return: MarshalAs(UnmanagedType.Bool)]
            [DllImport("kernel32.dll")]
            public static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

            /// <summary>
            /// Retrieves the frequency of the high-resolution performance counter, if one exists. The frequency cannot change while the system is running.
            /// </summary>
            /// <param name="lpFrequency">A pointer to a variable that receives the current performance-counter frequency, in counts per second.</param>
            /// <returns>Returns true if the installed hardware supports a high-resolution performance counter.</returns>
            [SuppressUnmanagedCodeSecurity]
            [return: MarshalAs(UnmanagedType.Bool)]
            [DllImport("kernel32.dll")]
            public static extern bool QueryPerformanceFrequency(out long lpFrequency);
        }
    }
}