using Workshop.Domain.Work;

namespace Workshop.Models
{
	public interface IWriteJobList
	{
		void Add(Job job);

		void Remove(JobIdentifier jobIdentifier);

		IWriteJob this[JobIdentifier job] { get; }
	}
}
