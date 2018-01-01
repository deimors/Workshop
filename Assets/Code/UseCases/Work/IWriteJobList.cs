using Workshop.Domain.Work;

namespace Workshop.UseCases.Work
{
	public interface IWriteJobList
	{
		void Add(JobStatus job);

		void Remove(JobIdentifier jobIdentifier);

		IWriteJob this[JobIdentifier job] { get; }
	}
}
