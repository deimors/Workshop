using Workshop.Domain.Work;

namespace Workshop.Models
{
	public interface IWriteJob
	{
		JobStatus Status { set; }

		bool Busy { set; }
	}
}