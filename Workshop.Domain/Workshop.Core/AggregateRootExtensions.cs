using Functional.Maybe;
using System;

namespace Workshop.Core
{
	public static class AggregateRootExtensions
	{
		public interface IAggregateCommand<TEvent, TError> : IRecordEvent<TEvent>
		{
			Maybe<TError> Execute();
		}

		public interface IValidatingAggregateCommand<TEvent, TError> : IAggregateCommand<TEvent, TError> { }

		public interface IValidatedAggregateCommand<TEvent, TError> : IAggregateCommand<TEvent, TError> { }

		internal class AggregateCommandRoot<TEvent, TError> : IValidatingAggregateCommand<TEvent, TError>
		{
			private readonly IRecordEvent<TEvent> _recorder;

			public AggregateCommandRoot(IRecordEvent<TEvent> recorder)
			{
				_recorder = recorder ?? throw new ArgumentNullException(nameof(recorder));
			}

			public Maybe<TError> Execute() 
				=> Maybe<TError>.Nothing;

			public void Record(TEvent @event) 
				=> _recorder.Record(@event);
		}

		internal class AggregateCommandFailIf<TEvent, TError> : IValidatingAggregateCommand<TEvent, TError>
		{
			private readonly IAggregateCommand<TEvent, TError> _parent;
			private readonly Func<bool> _predicate;
			private readonly Func<TError> _errorFactory;
			
			public AggregateCommandFailIf(IAggregateCommand<TEvent, TError> parent, Func<bool> predicate, Func<TError> errorFactory)
			{
				_parent = parent;
				_predicate = predicate;
				_errorFactory = errorFactory;
			}

			public Maybe<TError> Execute()
				=> _parent.Execute()
					.SelectOrElse(
						error => error.ToMaybe(),
						() => _predicate()
							? _errorFactory().ToMaybe()
							: Maybe<TError>.Nothing
					);

			public void Record(TEvent @event)
				=> _parent.Record(@event);
		}

		internal class AggregateCommandRecord<TEvent, TError> : IValidatedAggregateCommand<TEvent, TError>
		{
			private readonly IAggregateCommand<TEvent, TError> _parent;
			private readonly Func<TEvent> _eventFactory;
			
			public AggregateCommandRecord(IAggregateCommand<TEvent, TError> parent, Func<TEvent> eventFactory)
			{
				_parent = parent;
				_eventFactory = eventFactory;
			}

			public Maybe<TError> Execute() 
				=> _parent.Execute()
					.SelectOrElse(
						error => error.ToMaybe(),
						Succeed
					);
			
			public void Record(TEvent @event)
				=> _parent.Record(@event);

			private Maybe<TError> Succeed()
			{
				Record(_eventFactory());
				return Maybe<TError>.Nothing;
			}
		}

		internal class AggregateCommandRecordIf<TEvent, TError> : IValidatedAggregateCommand<TEvent, TError>
		{
			private readonly IAggregateCommand<TEvent, TError> _parent;
			private readonly Func<bool> _predicate;
			private readonly Func<TEvent> _eventFactory;
			
			public AggregateCommandRecordIf(IAggregateCommand<TEvent, TError> parent, Func<bool> predicate, Func<TEvent> eventFactory)
			{
				_parent = parent;
				_predicate = predicate;
				_eventFactory = eventFactory;
			}

			public Maybe<TError> Execute()
				=> _parent.Execute()
					.SelectOrElse(
						error => error.ToMaybe(),
						Succeed
					);
			
			public void Record(TEvent @event)
				=> _parent.Record(@event);

			private Maybe<TError> Succeed()
			{
				if (_predicate())
					Record(_eventFactory());

				return Maybe<TError>.Nothing;
			}
		}
		
		public static IValidatingAggregateCommand<TEvent, TError> BuildCommand<TEvent, TError>(this IRecordEvent<TEvent> recorder)
			=> new AggregateCommandRoot<TEvent, TError>(recorder);
		
		public static IValidatingAggregateCommand<TEvent, TError> FailIf<TEvent, TError>(this IValidatingAggregateCommand<TEvent, TError> command, Func<bool> predicate, Func<TError> errorFactory)
			=> new AggregateCommandFailIf<TEvent, TError>(command, predicate, errorFactory);
		
		public static IValidatedAggregateCommand<TEvent, TError> Record<TEvent, TError>(this IAggregateCommand<TEvent, TError> command, Func<TEvent> eventFactory)
			=> new AggregateCommandRecord<TEvent, TError>(command, eventFactory);
		
		public static IValidatedAggregateCommand<TEvent, TError> RecordIf<TEvent, TError>(this IAggregateCommand<TEvent, TError> command, Func<bool> predicate, Func<TEvent> eventFactory)
			=> new AggregateCommandRecordIf<TEvent, TError>(command, predicate, eventFactory);
	}
}
