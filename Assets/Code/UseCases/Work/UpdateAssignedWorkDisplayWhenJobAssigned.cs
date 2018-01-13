using System;
using UniRx;
using Workshop.Domain.Work;
using Workshop.Domain.Work.Aggregates;

namespace Workshop.UseCases.Work
{
	public class UpdateAssignedWorkDisplayWhenJobAssigned
	{
		public UpdateAssignedWorkDisplayWhenJobAssigned(JobIdentifier jobId, IObservable<WorkshopEvent> workshopEvents, IDisplayAssignedWork displayAssignedWork)
		{
			workshopEvents
				.OfType<WorkshopEvent, WorkshopEvent.JobAssigned>()
				.Where(jobAssigned => jobAssigned.JobId == jobId)
				.Select(jobAssigned => $"Assigned to {jobAssigned.WorkerId.ToString()}")
				.Subscribe(assignmentText => displayAssignedWork.AssignedWork = assignmentText);
		}
	}
}
