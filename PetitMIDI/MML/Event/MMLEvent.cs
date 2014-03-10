namespace PetitMIDI.MML.Event
{
	/// <summary>
	/// Describes the type of MML event.
	/// </summary>
	public enum EventTag
	{
		None,
		Invalid,
		Note,
		Tie,
		Pitch,
		Rest,
		Length,
		Tempo,
		Velocity,
		VelocityIncrease,
		VelocityDecrease,
		Pan,
		Instrument,
		Octave,
		OctaveIncrease,
		OctaveDecrease,
		Volume,
		Detune,
		Envelope,
		EnvelopeRelease,
		Modulation,
		Tremolo,
		Vibrato
	}

	/// <summary>
	/// Represents a generic MIDI event.
	/// </summary>
	public abstract class MMLEvent
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MMLEvent"/> class.
		/// </summary>
		/// <param name="eventType">The type of event.</param>
		protected MMLEvent(EventTag eventType)
		{
			this.Tag = eventType;
		}

		/// <summary>
		/// Gets or sets a value indicating the event type.
		/// </summary>
		public EventTag Tag
		{
			get;
			protected set;
		}
	}
}