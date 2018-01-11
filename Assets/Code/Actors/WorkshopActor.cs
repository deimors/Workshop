using Functional.Maybe;
using OneOf;
using System;
using System.Collections.Concurrent;
using System.Linq;
using UniRx;
using UnityEngine;
using Workshop.Domain.Work;
using Workshop.Domain.Work.Aggregates;

namespace Workshop.Actors
{
	public class WorkshopCommandException : Exception
	{
		public WorkshopCommand Command { get; }
		public WorkshopError Error { get; }

		public WorkshopCommandException(WorkshopCommand command, WorkshopError error)
		{
			Command = command;
			Error = error;
		}
	}

	public interface IQueueWorkshopCommands
	{
		void QueueCommand(WorkshopCommand command);
	}

	public class WorkshopActor : IObservable<WorkshopEvent>, IQueueWorkshopCommands
	{
		private readonly WorkshopAggregate _workshopAggregate = new WorkshopAggregate();

		private readonly ConcurrentQueue<WorkshopEvent> _eventHistory = new ConcurrentQueue<WorkshopEvent>();

		private readonly ConcurrentQueue<WorkshopCommand> _commandQueue = new ConcurrentQueue<WorkshopCommand>();

		private readonly ISubject<WorkshopEvent> _eventSubject = new Subject<WorkshopEvent>();

		public WorkshopActor(IObservable<Unit> processQueueTicks)
		{
			processQueueTicks.Subscribe(_ => ProcessQueue());
		}

		public IDisposable Subscribe(IObserver<WorkshopEvent> observer)
			=> _eventHistory.ToObservable()
				.Concat(_eventSubject)
				.Subscribe(observer);

		public void QueueCommand(WorkshopCommand command)
			=> _commandQueue.Enqueue(command);

		private void ProcessQueue()
		{
			WorkshopCommand command;

			while (_commandQueue.TryDequeue(out command))
				ProcessCommand(command);
		}

		private Maybe<WorkshopError> ProcessCommand(WorkshopCommand command) 
			=> _workshopAggregate
				.HandleCommand(command)
				.Match(error => HandleError(command, error), CommitEvents);

		private void HandleError(WorkshopCommand command, WorkshopError error)
		{
			Debug.LogError($"{command.GetType().Name} threw {error.ToString()}");
		}

		private void CommitEvents()
		{
			var uncommittedEvents = _workshopAggregate.UncommittedEvents.ToArray();

			foreach (var newEvent in uncommittedEvents)
				_eventHistory.Enqueue(newEvent);
			
			_workshopAggregate.MarkCommitted();

			foreach (var newEvent in uncommittedEvents)
				_eventSubject.OnNext(newEvent);
		}
	}
}
