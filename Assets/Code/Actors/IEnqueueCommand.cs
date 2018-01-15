namespace Workshop.Actors
{
	public interface IEnqueueCommand<TCommand>
	{
		void Enqueue(TCommand command);
	}
}
