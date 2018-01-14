using Functional.Maybe;
using OneOf;
using System;
using System.Collections.Concurrent;
using System.Linq;
using UniRx;
using UnityEngine;
using Workshop.Core;
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
	
	public interface IEnqueueCommand<TCommand>
	{
		void Enqueue(TCommand command);
	}

	public interface IEnqueueCommand<TCommand, TError>
	{
		IObservable<CommandResult<TError>> Enqueue(TCommand command);
	}

	public abstract class CommandResult<TError> : OneOfBase<CommandResult<TError>.Success, CommandResult<TError>.Failure>
	{
		public class Success : CommandResult<TError> { }

		public class Failure : CommandResult<TError>
		{
			public TError Error { get; }

			public Failure(TError error)
			{
				Error = error;
			}
		}
	}

	public interface ICommandQueueItem<TCommand, TError>
	{
		void Process(IHandleCommand<TCommand, TError> commandHandler, Action commitEvents);
	}

	public class CommandQueueItem<TCommand, TError> : ICommandQueueItem<TCommand, TError>
	{
		private readonly TCommand _command;

		private bool _processed = false;

		public CommandQueueItem(TCommand command)
		{
			_command = command;
		}
		
		public void Process(IHandleCommand<TCommand, TError> commandHandler, Action commitEvents)
		{
			if (_processed)
				throw new Exception("Command already processed");

			commandHandler
				.HandleCommand(_command)
				.Match(
					OnError, 
					() => { commitEvents(); OnSuccess(); }
				);

			_processed = true;
		}

		protected virtual void OnSuccess() { }

		protected virtual void OnError(TError error)
			=> Debug.LogError($"{_command.GetType().Name} threw {error.ToString()}");
	}

	public class ObservableCommandQueueItem<TCommand, TError> : CommandQueueItem<TCommand, TError>, IObservable<CommandResult<TError>>
	{
		private readonly ISubject<CommandResult<TError>> _resultSubject = new Subject<CommandResult<TError>>();
		
		public ObservableCommandQueueItem(TCommand command) : base(command) { }

		public IDisposable Subscribe(IObserver<CommandResult<TError>> observer)
			=> _resultSubject.Subscribe(observer);
		
		protected override void OnError(TError error)
			=> _resultSubject.OnNext(new CommandResult<TError>.Failure(error));

		protected override void OnSuccess()
			=> _resultSubject.OnNext(new CommandResult<TError>.Success());
	}

	public class WorkshopActor : IObservable<WorkshopEvent>, IEnqueueCommand<WorkshopCommand>, IEnqueueCommand<WorkshopCommand, WorkshopError>
	{
		private readonly WorkshopAggregate _workshopAggregate = new WorkshopAggregate();

		private readonly ConcurrentQueue<WorkshopEvent> _eventHistory = new ConcurrentQueue<WorkshopEvent>();

		private readonly ConcurrentQueue<ICommandQueueItem<WorkshopCommand, WorkshopError>> _commandQueue = new ConcurrentQueue<ICommandQueueItem<WorkshopCommand, WorkshopError>>();

		private readonly ISubject<WorkshopEvent> _eventSubject = new Subject<WorkshopEvent>();

		public WorkshopActor(IObservable<Unit> processQueueTicks)
		{
			processQueueTicks.Subscribe(_ => ProcessQueue());
		}

		public IDisposable Subscribe(IObserver<WorkshopEvent> observer)
			=> _eventHistory.ToObservable()
				.Concat(_eventSubject)
				.Subscribe(observer);

		public void Enqueue(WorkshopCommand command)
			=> _commandQueue.Enqueue(new CommandQueueItem<WorkshopCommand, WorkshopError>(command));

		IObservable<CommandResult<WorkshopError>> IEnqueueCommand<WorkshopCommand, WorkshopError>.Enqueue(WorkshopCommand command)
		{
			var queueItem = new ObservableCommandQueueItem<WorkshopCommand, WorkshopError>(command);
			_commandQueue.Enqueue(queueItem);
			return queueItem;
		}

		private void ProcessQueue()
		{
			ICommandQueueItem<WorkshopCommand, WorkshopError> queueItem;

			while (_commandQueue.TryDequeue(out queueItem))
				queueItem.Process(_workshopAggregate, CommitEvents);
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
