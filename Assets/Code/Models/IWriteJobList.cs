using Workshop.Domain.Work;

namespace Workshop.Models
{
	public interface IWriteJobList
	{
		void Add(JobStatus job);

		void Remove(JobIdentifier jobIdentifier);

		IWriteJob this[JobIdentifier job] { get; }
	}
}
