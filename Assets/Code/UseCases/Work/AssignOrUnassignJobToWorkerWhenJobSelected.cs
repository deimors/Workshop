using Functional.Maybe;
using UniRx;
using Workshop.Actors;
using Workshop.Domain.Work;
using Workshop.Domain.Work.Aggregates;

namespace Workshop.UseCases.Work
{
	public class AssignOrUnassignJobToWorkerWhenJobSelected
	{
		public AssignOrUnassignJobToWorkerWhenJobSelected(WorkerIdentifier workerId, IJobSelectedObservable jobSelectedObservable, IQueueWorkshopCommands queueWorkshopCommands)
		{
			jobSelectedObservable
				.Select(maybeJobId => BuildJobSelectionCommand(maybeJobId, workerId))
				.Subscribe(queueWorkshopCommands.QueueCommand);
		}

		private WorkshopCommand BuildJobSelectionCommand(Maybe<JobIdentifier> maybeJobId, WorkerIdentifier workerId)
			=> maybeJobId.SelectOrElse<JobIdentifier, WorkshopCommand>(
				jobId => new WorkshopCommand.AssignJob(jobId, workerId),
				() => new WorkshopCommand.UnassignWorker(workerId)
			);
	}

}
