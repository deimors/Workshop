using Functional.Maybe;
using OneOf;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
		Maybe<TError> Process(IHandleCommand<TCommand, TError> commandHandler);
	}

	public class CommandQueueItem<TCommand, TError> : ICommandQueueItem<TCommand, TError>
	{
		private readonly TCommand _command;

		private bool _processed = false;

		public CommandQueueItem(TCommand command)
		{
			_command = command;
		}
		
		public virtual Maybe<TError> Process(IHandleCommand<TCommand, TError> commandHandler)
		{
			if (_processed)
				throw new Exception("Command already processed");

			_processed = true;

			return commandHandler.HandleCommand(_command);	
		}
	}

	public class ObservableCommandQueueItem<TCommand, TError> : CommandQueueItem<TCommand, TError>, IObservable<CommandResult<TError>>
	{
		private readonly ISubject<CommandResult<TError>> _resultSubject = new Subject<CommandResult<TError>>();
		
		public ObservableCommandQueueItem(TCommand command) : base(command) { }

		public IDisposable Subscribe(IObserver<CommandResult<TError>> observer)
			=> _resultSubject.Subscribe(observer);

		public override Maybe<TError> Process(IHandleCommand<TCommand, TError> commandHandler) 
			=> base.Process(commandHandler).Match(SendErrorResult, SendSuccessResult);

		private void SendErrorResult(TError error)
		{
			_resultSubject.OnNext(new CommandResult<TError>.Failure(error));
			_resultSubject.OnCompleted();
		}

		private void SendSuccessResult()
		{
			_resultSubject.OnNext(new CommandResult<TError>.Success());
			_resultSubject.OnCompleted();
		}
	}

	public class CommandQueue<TCommand, TError> : IEnqueueCommand<TCommand>, IEnqueueCommand<TCommand, TError>
	{
		private readonly ConcurrentQueue<ICommandQueueItem<TCommand, TError>> _commandQueue = new ConcurrentQueue<ICommandQueueItem<TCommand, TError>>();

		private readonly Action commit;
		private readonly Action<TError> onError;

		public CommandQueue(Action commit, Action<TError> onError)
		{
			this.commit = commit;
			this.onError = onError;
		}

		void IEnqueueCommand<TCommand>.Enqueue(TCommand command)
			=> _commandQueue.Enqueue(new CommandQueueItem<TCommand, TError>(command));

		IObservable<CommandResult<TError>> IEnqueueCommand<TCommand, TError>.Enqueue(TCommand command)
		{
			var queueItem = new ObservableCommandQueueItem<TCommand, TError>(command);
			_commandQueue.Enqueue(queueItem);
			return queueItem;
		}

		public void ProcessQueue(IHandleCommand<TCommand, TError> commandHandler)
		{
			ICommandQueueItem<TCommand, TError> queueItem;

			while (_commandQueue.TryDequeue(out queueItem))
				queueItem.Process(commandHandler).Match(onError, commit);
		}
	}

	public class EventHistory<TEvent> : IObservable<TEvent>
	{
		private readonly ConcurrentQueue<TEvent> _eventHistory = new ConcurrentQueue<TEvent>();

		private readonly ISubject<TEvent> _eventSubject = new Subject<TEvent>();

		public IDisposable Subscribe(IObserver<TEvent> observer)
			=> _eventHistory.ToObservable()
				.Concat(_eventSubject)
				.Subscribe(observer);

		public void CommitEvents(IEnumerable<TEvent> uncommittedEvents)
		{
			uncommittedEvents = uncommittedEvents.ToArray();
			
			foreach (var newEvent in uncommittedEvents)
				_eventHistory.Enqueue(newEvent);
			
			foreach (var newEvent in uncommittedEvents)
				_eventSubject.OnNext(newEvent);
		}
	}

	public class WorkshopActor : IObservable<WorkshopEvent>, IEnqueueCommand<WorkshopCommand>, IEnqueueCommand<WorkshopCommand, WorkshopError>
	{
		private readonly WorkshopAggregate _workshopAggregate = new WorkshopAggregate();

		private readonly CommandQueue<WorkshopCommand, WorkshopError> _commandQueue;

		private readonly EventHistory<WorkshopEvent> _history = new EventHistory<WorkshopEvent>();

		public WorkshopActor(IObservable<Unit> processQueueTicks)
		{
			_commandQueue = new CommandQueue<WorkshopCommand, WorkshopError>(CommitEvents, OnError);

			processQueueTicks.Subscribe(_ => ProcessQueue());
		}

		public IDisposable Subscribe(IObserver<WorkshopEvent> observer)
			=> _history.Subscribe(observer);

		void IEnqueueCommand<WorkshopCommand>.Enqueue(WorkshopCommand command)
			=> (_commandQueue as IEnqueueCommand<WorkshopCommand>).Enqueue(command);

		IObservable<CommandResult<WorkshopError>> IEnqueueCommand<WorkshopCommand, WorkshopError>.Enqueue(WorkshopCommand command)
			=> (_commandQueue as IEnqueueCommand<WorkshopCommand, WorkshopError>).Enqueue(command);

		private void ProcessQueue()
			=> _commandQueue.ProcessQueue(_workshopAggregate);

		private void CommitEvents()
		{
			_history.CommitEvents(_workshopAggregate.UncommittedEvents);
			
			_workshopAggregate.MarkCommitted();
		}

		private void OnError(WorkshopError error)
			=> Debug.LogError($"Error: {error.ToString()}");
	}
}
