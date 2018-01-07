using Functional.Maybe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.Core
{
	public static class AggregateRootExtensions
	{
		public class AggregateCommand<TEvent, TError>
		{
			public AggregateRoot<TEvent> Subject;

			public AggregateCommand(AggregateRoot<TEvent> subject)
			{
				Subject = subject ?? throw new ArgumentNullException(nameof(subject));
			}

			public virtual Maybe<TError> Execute() 
				=> Maybe<TError>.Nothing;
		}

		public class AggregateCommandRecord<TEvent, TError> : AggregateCommand<TEvent, TError>
		{
			private readonly AggregateCommand<TEvent, TError> _parent;
			private readonly TEvent _event;

			public AggregateCommandRecord(AggregateCommand<TEvent, TError> parent, TEvent @event) : base(parent.Subject)
			{
				_parent = parent;
				_event = @event;
			}

			public override Maybe<TError> Execute() 
				=> _parent.Execute()
					.SelectOrElse(
						error => error.ToMaybe(),
						() =>
						{
							Subject.Record(_event);
							return Maybe<TError>.Nothing;
						}
					);
		}

		public class AggregateCommandRecordIf<TEvent, TError> : AggregateCommand<TEvent, TError>
		{
			private readonly AggregateCommand<TEvent, TError> _parent;
			private readonly Func<bool> _predicate;
			private readonly Func<TEvent> _eventFactory;

			public AggregateCommandRecordIf(AggregateCommand<TEvent, TError> parent, Func<bool> predicate, Func<TEvent> eventFactory) : base(parent.Subject)
			{
				_parent = parent;
				_predicate = predicate;
				_eventFactory = eventFactory;
			}

			public override Maybe<TError> Execute()
				=> _parent.Execute()
					.SelectOrElse(
						error => error.ToMaybe(),
						() =>
						{
							if (_predicate())
								Subject.Record(_eventFactory());

							return Maybe<TError>.Nothing;
						}
					);
		}

		public class AggregateCommandFailIf<TEvent, TError> : AggregateCommand<TEvent, TError>
		{
			private readonly AggregateCommand<TEvent, TError> _parent;
			private readonly Func<bool> _predicate;
			private readonly Func<TError> _errorFactory;

			public AggregateCommandFailIf(AggregateCommand<TEvent, TError> parent, Func<bool> predicate, Func<TError> errorFactory) : base(parent.Subject)
			{
				_parent = parent;
				_predicate = predicate;
				_errorFactory = errorFactory;
			}

			public override Maybe<TError> Execute()
				=> _parent.Execute()
					.SelectOrElse(
						error => error.ToMaybe(),
						() => _predicate()
							? _errorFactory().ToMaybe()
							: Maybe<TError>.Nothing
					);
		}

		public static AggregateCommand<TEvent, TError> BuildCommand<TEvent, TError>(this AggregateRoot<TEvent> aggregateRoot)
			=> new AggregateCommand<TEvent, TError>(aggregateRoot);
		
		public static AggregateCommand<TEvent, TError> FailIf<TEvent, TError>(this AggregateCommand<TEvent, TError> command, Func<bool> predicate, Func<TError> errorFactory)
			=> new AggregateCommandFailIf<TEvent, TError>(command, predicate, errorFactory);
		
		public static AggregateCommand<TEvent, TError> Record<TEvent, TError>(this AggregateCommand<TEvent, TError> command, TEvent @event)
			=> new AggregateCommandRecord<TEvent, TError>(command, @event);
		
		public static AggregateCommand<TEvent, TError> RecordIf<TEvent, TError>(this AggregateCommand<TEvent, TError> command, Func<bool> predicate, Func<TEvent> eventFactory)
			=> new AggregateCommandRecordIf<TEvent, TError>(command, predicate, eventFactory);


	}
}
