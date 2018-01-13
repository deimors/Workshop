using Functional.Maybe;
using Workshop.Domain.Work;

namespace Workshop.UseCases.Work
{
	public interface IDisplaySelectedJob
	{
		Maybe<JobIdentifier> SelectedJob { set; }
	}
}
