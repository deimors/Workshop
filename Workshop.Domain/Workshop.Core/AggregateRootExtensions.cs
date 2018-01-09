﻿using Functional.Maybe;
using System;

namespace Workshop.Core
{
	public static class AggregateRootExtensions
	{
		public class AggregateCommand<TEvent, TError>
		{
			public IRecordEvent<TEvent> Recorder;

			public AggregateCommand(IRecordEvent<TEvent> recorder)
			{
				Recorder = recorder ?? throw new ArgumentNullException(nameof(recorder));
			}

			public virtual Maybe<TError> Execute() 
				=> Maybe<TError>.Nothing;
		}

		public class AggregateCommandRecord<TEvent, TError> : AggregateCommand<TEvent, TError>
		{
			private readonly AggregateCommand<TEvent, TError> _parent;
			private readonly Func<TEvent> _eventFactory;

			public AggregateCommandRecord(AggregateCommand<TEvent, TError> parent, Func<TEvent> eventFactory) : base(parent.Recorder)
			{
				_parent = parent;
				_eventFactory = eventFactory;
			}

			public override Maybe<TError> Execute() 
				=> _parent.Execute()
					.SelectOrElse(
						error => error.ToMaybe(),
						() =>
						{
							Recorder.Record(_eventFactory());
							return Maybe<TError>.Nothing;
						}
					);
		}

		public class AggregateCommandRecordIf<TEvent, TError> : AggregateCommand<TEvent, TError>
		{
			private readonly AggregateCommand<TEvent, TError> _parent;
			private readonly Func<bool> _predicate;
			private readonly Func<TEvent> _eventFactory;

			public AggregateCommandRecordIf(AggregateCommand<TEvent, TError> parent, Func<bool> predicate, Func<TEvent> eventFactory) : base(parent.Recorder)
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
								Recorder.Record(_eventFactory());

							return Maybe<TError>.Nothing;
						}
					);
		}

		public class AggregateCommandFailIf<TEvent, TError> : AggregateCommand<TEvent, TError>
		{
			private readonly AggregateCommand<TEvent, TError> _parent;
			private readonly Func<bool> _predicate;
			private readonly Func<TError> _errorFactory;

			public AggregateCommandFailIf(AggregateCommand<TEvent, TError> parent, Func<bool> predicate, Func<TError> errorFactory) : base(parent.Recorder)
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

		public static AggregateCommand<TEvent, TError> BuildCommand<TEvent, TError>(this IRecordEvent<TEvent> recorder)
			=> new AggregateCommand<TEvent, TError>(recorder);
		
		public static AggregateCommand<TEvent, TError> FailIf<TEvent, TError>(this AggregateCommand<TEvent, TError> command, Func<bool> predicate, Func<TError> errorFactory)
			=> new AggregateCommandFailIf<TEvent, TError>(command, predicate, errorFactory);
		
		public static AggregateCommand<TEvent, TError> Record<TEvent, TError>(this AggregateCommand<TEvent, TError> command, Func<TEvent> eventFactory)
			=> new AggregateCommandRecord<TEvent, TError>(command, eventFactory);
		
		public static AggregateCommand<TEvent, TError> RecordIf<TEvent, TError>(this AggregateCommand<TEvent, TError> command, Func<bool> predicate, Func<TEvent> eventFactory)
			=> new AggregateCommandRecordIf<TEvent, TError>(command, predicate, eventFactory);


	}
}
