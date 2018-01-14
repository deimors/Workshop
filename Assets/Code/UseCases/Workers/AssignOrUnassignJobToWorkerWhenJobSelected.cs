using Functional.Maybe;
using UniRx;
using Workshop.Actors;
using Workshop.Domain.Work;
using Workshop.Domain.Work.Aggregates;

namespace Workshop.UseCases.Work
{
	public interface IDisplayJobListDropdownValidation
	{
		void ShowError(WorkshopError error);
	}

	public class AssignOrUnassignJobToWorkerWhenJobSelected
	{
		public AssignOrUnassignJobToWorkerWhenJobSelected(WorkerIdentifier workerId, IJobSelectedObservable jobSelectedObservable, IEnqueueCommand<WorkshopCommand, WorkshopError> workshopCommands, IDisplayJobListDropdownValidation displayValidation)
		{
			jobSelectedObservable
				.Select(maybeJobId => BuildJobSelectionCommand(maybeJobId, workerId))
				.SelectMany(command => workshopCommands.Enqueue(command))
				.Subscribe(result => result.Switch(success => { }, failure => displayValidation.ShowError(failure.Error)));
		}
		
		private WorkshopCommand BuildJobSelectionCommand(Maybe<JobIdentifier> maybeJobId, WorkerIdentifier workerId)
			=> maybeJobId.SelectOrElse<JobIdentifier, WorkshopCommand>(
				jobId => new WorkshopCommand.AssignJob(jobId, workerId),
				() => new WorkshopCommand.UnassignWorker(workerId)
			);
	}
}
