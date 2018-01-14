using Functional.Maybe;
using UniRx;
using Workshop.Actors;
using Workshop.Domain.Work;
using Workshop.Domain.Work.Aggregates;

namespace Workshop.UseCases.Work
{
	public class AssignOrUnassignJobToWorkerWhenJobSelected
	{
		public AssignOrUnassignJobToWorkerWhenJobSelected(WorkerIdentifier workerId, IJobSelectedObservable jobSelectedObservable, IEnqueueCommand<WorkshopCommand> workshopCommands)
		{
			jobSelectedObservable
				.Select(maybeJobId => BuildJobSelectionCommand(maybeJobId, workerId))
				.Subscribe(workshopCommands.Enqueue);
		}

		private WorkshopCommand BuildJobSelectionCommand(Maybe<JobIdentifier> maybeJobId, WorkerIdentifier workerId)
			=> maybeJobId.SelectOrElse<JobIdentifier, WorkshopCommand>(
				jobId => new WorkshopCommand.AssignJob(jobId, workerId),
				() => new WorkshopCommand.UnassignWorker(workerId)
			);
	}

}
