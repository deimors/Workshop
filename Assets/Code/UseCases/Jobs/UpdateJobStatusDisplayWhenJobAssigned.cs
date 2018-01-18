using System;
using UniRx;
using Workshop.Domain.Work;
using Workshop.Domain.Work.Aggregates;

namespace Workshop.UseCases.Work
{
	public class UpdateJobStatusDisplayWhenJobAssigned
	{
		public UpdateJobStatusDisplayWhenJobAssigned(JobIdentifier jobId, IObservable<WorkshopEvent> workshopEvents, IDisplayJobStatus displayAssignedWork)
		{
			workshopEvents
				.OfType<WorkshopEvent, WorkshopEvent.JobAssigned>()
				.Where(jobAssigned => jobAssigned.JobId == jobId)
				.Select(jobAssigned => $"Assigned to {jobAssigned.WorkerId.ToString()}")
				.Subscribe(assignmentText => displayAssignedWork.Status = assignmentText);
		}
	}
}
