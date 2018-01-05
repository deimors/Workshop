using Workshop.Domain.Work;

namespace Workshop.Models
{
	public interface IWriteJob
	{
		Job Value { set; }

		bool Busy { set; }
	}
}