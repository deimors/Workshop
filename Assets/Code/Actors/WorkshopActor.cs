using System;
using System.Collections.Generic;
using UniRx;
using Workshop.Core;
using Workshop.Domain.Work;
using Workshop.Domain.Work.Aggregates;

namespace Workshop.Actors
{
	public class WorkshopActor : Actor<WorkshopEvent, WorkshopCommand, WorkshopError>
	{
		private readonly WorkshopAggregate _workshopAggregate = new WorkshopAggregate();

		public WorkshopActor(IObservable<Unit> processQueueTicks) : base(processQueueTicks) { }

		protected override IHandleCommand<WorkshopCommand, WorkshopError> CommandHandler
			=> _workshopAggregate;

		protected override IEnumerable<WorkshopEvent> UncommittedEvents
			=> _workshopAggregate.UncommittedEvents;

		protected override void MarkCommitted()
			=> _workshopAggregate.MarkCommitted();
	}
}
