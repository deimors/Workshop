using Functional.Maybe;
using System;
using UniRx;
using Workshop.Domain.Work.Aggregates;

namespace Workshop.UseCases.Work
{
	public class AddJobListDropdownOptionWhenJobAdded
	{
		public AddJobListDropdownOptionWhenJobAdded(IObservable<WorkshopEvent> workshopEvents, IAddJobListDropdownOption addJobListDropdownOption)
		{
			workshopEvents
				.OfType<WorkshopEvent, WorkshopEvent.JobAdded>()
				.Select(jobAdded => jobAdded.Job.Id)
				.Subscribe(addJobListDropdownOption.AddJobOption);
		}
	}

}
