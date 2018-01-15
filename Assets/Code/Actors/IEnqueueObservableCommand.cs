using System;

namespace Workshop.Actors
{
	public interface IEnqueueObservableCommand<TCommand, TError>
	{
		IObservable<CommandResult<TError>> ObserveResult(TCommand command);
	}
}
