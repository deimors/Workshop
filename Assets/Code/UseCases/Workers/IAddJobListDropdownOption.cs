using Workshop.Domain.Work;

namespace Workshop.UseCases.Work
{
	public interface IAddJobListDropdownOption
	{
		void AddJobOption(JobIdentifier jobId);
	}

}
