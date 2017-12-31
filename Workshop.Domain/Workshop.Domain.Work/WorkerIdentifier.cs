namespace Workshop.Domain.Work
{
	public class WorkerIdentifier
	{
		public override string ToString()
			=> $"Worker {GetHashCode().ToString("X").Substring(0, 4)}";
	}
}
