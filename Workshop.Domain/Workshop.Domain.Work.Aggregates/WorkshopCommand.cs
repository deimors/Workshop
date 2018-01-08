using OneOf;

namespace Workshop.Domain.Work.Aggregates
{
	public class WorkshopCommand 
		: OneOfBase<
			WorkshopCommand.AddWorker, 
			WorkshopCommand.AddJob, 
			WorkshopCommand.AssignJob, 
			WorkshopCommand.UnassignWorker,
			WorkshopCommand.UpdateJobStatus
		>
	{
		public class AddWorker : WorkshopCommand
		{
			public Worker Worker { get; }

			public AddWorker(Worker worker)
			{
				Worker = worker;
			}
		}

		public class AddJob : WorkshopCommand
		{
			public Job Job { get; }

			public AddJob(Job job)
			{
				Job = job;
			}
		}

		public class AssignJob : WorkshopCommand
		{
			public JobIdentifier JobId { get; }
			public WorkerIdentifier WorkerId { get; }

			public AssignJob(JobIdentifier jobId, WorkerIdentifier workerId)
			{
				JobId = jobId;
				WorkerId = workerId;
			}
		}

		public class UnassignWorker : WorkshopCommand
		{
			public WorkerIdentifier WorkerId { get; }

			public UnassignWorker(WorkerIdentifier workerId)
			{
				WorkerId = workerId;
			}
		}

		public class UpdateJobStatus : WorkshopCommand
		{
			public JobIdentifier JobId { get; }
			public JobStatus Status { get; }

			public UpdateJobStatus(JobIdentifier jobId, JobStatus status)
			{
				JobId = jobId;
				Status = status;
			}
		}
	}
}
