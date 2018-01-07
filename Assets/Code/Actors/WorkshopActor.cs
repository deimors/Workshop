using Functional.Maybe;
using OneOf;
using System;
using System.Collections.Concurrent;
using System.Linq;
using UniRx;
using Workshop.Domain.Work;
using Workshop.Domain.Work.Aggregates;

namespace Workshop.Actors
{
	public class WorkshopCommand : OneOfBase<WorkshopCommand.AddWorker, WorkshopCommand.AddJob>
	{
		public class AddWorker : WorkshopCommand
		{
			public WorkerIdentifier WorkerId { get; }

			public AddWorker(WorkerIdentifier workerId)
			{
				WorkerId = workerId;
			}
		}

		public class AddJob : WorkshopCommand
		{
			public Job Job { get; }

			public AddJob(Job job)
			{
				Job = job;
			}
		}
	}

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

			if (_commandQueue.TryDequeue(out command))
				HandleCommand(command).Match(error => HandleError(command, error), CommitEvents);
		}

		private Maybe<WorkshopError> HandleCommand(WorkshopCommand command)
			=> command.Match(
				addWorker => _workshopAggregate.AddWorker(addWorker.WorkerId),
				addJob => _workshopAggregate.AddJob(addJob.Job)
			);

		private void HandleError(WorkshopCommand command, WorkshopError error)
			=> _eventSubject.OnError(new WorkshopCommandException(command, error));

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
