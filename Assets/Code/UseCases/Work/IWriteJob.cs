using Workshop.Domain.Work;

namespace Workshop.UseCases.Work
{
	public interface IWriteJob
	{
		JobStatus Status { set; }

		bool Busy { set; }
	}
}