using System;
using UniRx;
using Workshop.Domain.Work;
using Workshop.Domain.Work.Aggregates;

namespace Workshop.UseCases.Work
{
	public class UpdateAssignedWorkDisplayWhenJobAddedOrUnassigned
	{
		public UpdateAssignedWorkDisplayWhenJobAddedOrUnassigned(JobIdentifier jobId, IObservable<WorkshopEvent> workshopEvents, IDisplayAssignedWork displayAssignedWork)
		{
			var addedJobIds = workshopEvents
				.OfType<WorkshopEvent, WorkshopEvent.JobAdded>()
				.Select(jobAdded => jobAdded.Job.Id);

			var unassignedJobIds = workshopEvents
				.OfType<WorkshopEvent, WorkshopEvent.JobUnassigned>()
				.Select(jobUnassigned => jobUnassigned.JobId);

			addedJobIds.Merge(unassignedJobIds)
				.Where(eventJobId => eventJobId == jobId)
				.Subscribe(_ => displayAssignedWork.AssignedWork = "Unassigned");
		}
	}
}
