using OneOf;

namespace Workshop.Domain.Work.Aggregates
{
	public class WorkshopCommand 
		: OneOfBase<
			WorkshopCommand.AddWorker, 
			WorkshopCommand.AddJob, 
			WorkshopCommand.AssignJob, 
			WorkshopCommand.UnassignWorker,
			WorkshopCommand.StartWork,
			WorkshopCommand.CompleteWork
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

		public class StartWork : WorkshopCommand
		{
			public JobIdentifier JobId { get; }

			public StartWork(JobIdentifier jobId)
			{
				JobId = jobId;
			}
		}

		public class CompleteWork : WorkshopCommand
		{
			public JobIdentifier JobId { get; }
			public QuantityOfWork Quantity { get; }

			public CompleteWork(JobIdentifier jobId, QuantityOfWork quantity)
			{
				JobId = jobId;
				Quantity = quantity;
			}
		}
	}
}
