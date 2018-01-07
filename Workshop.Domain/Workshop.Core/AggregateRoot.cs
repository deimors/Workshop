using System.Collections.Generic;
using System.Linq;

namespace Workshop.Core
{
	public abstract class AggregateRoot<TEvent>
	{
		private readonly Queue<TEvent> _uncommittedEvents = new Queue<TEvent>();

		protected abstract void ApplyEvent(TEvent @event);

		public IEnumerable<TEvent> UncommittedEvents => _uncommittedEvents.AsEnumerable();

		public void MarkCommitted() => _uncommittedEvents.Clear();

		public void LoadFromHistory(IEnumerable<TEvent> events)
		{
			foreach (var @event in events)
				ApplyEvent(@event);
		}

		public void Record(TEvent @event)
		{
			ApplyEvent(@event);
			_uncommittedEvents.Enqueue(@event);
		}
	}
}
