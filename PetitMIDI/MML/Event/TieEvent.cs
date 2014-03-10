using System.Collections.Generic;

namespace PetitMIDI.MML.Event
{
	public class TieEvent : MMLEvent
	{
		public TieEvent()
			: base(EventTag.Tie)
		{
		}
	}
}