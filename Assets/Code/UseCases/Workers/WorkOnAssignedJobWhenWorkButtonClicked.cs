using Functional.Maybe;
using System;
using UniRx;
using UnityEngine;
using Workshop.Actors;
using Workshop.Domain.Work;
using Workshop.Domain.Work.Aggregates;

namespace Workshop.UseCases.Work
{
	public class WorkerAttributesReadModel
	{
		public WorkerAttributes Value { get; private set; }

		public WorkerAttributesReadModel(WorkerIdentifier workerId, IObservable<WorkshopEvent> workshopEvents)
		{
			workshopEvents
				.OfType<WorkshopEvent, WorkshopEvent.WorkerAdded>()
				.Where(workerAdded => workerAdded.Worker.Id == workerId)
				.Subscribe(workerAdded => Value = workerAdded.Worker.Attributes);
		}
	}

	public class WorkOnAssignedJobWhenWorkButtonClicked
	{
		private readonly AssignedJobReadModel _assignedJob;
		private readonly WorkerAttributesReadModel _attributes;
		private readonly IEnqueueCommand<WorkshopCommand> _workshopCommands;
		private readonly IDisplayProgress _displayProgress;

		public WorkOnAssignedJobWhenWorkButtonClicked(AssignedJobReadModel assignedJobModel, WorkerAttributesReadModel workerSpeedModel, IWorkButtonClickedObservable workButtonClicked, IEnqueueCommand<WorkshopCommand> workshopCommands, IDisplayProgress displayProgress)
		{
			_assignedJob = assignedJobModel;
			_attributes = workerSpeedModel;
			_workshopCommands = workshopCommands;
			_displayProgress = displayProgress;
			
			workButtonClicked.Subscribe(_ => MaybeWorkOnAssignedJob());
		}

		public void MaybeWorkOnAssignedJob()
			=> _assignedJob.Value.Match(WorkOnJob, () => { });

		private void WorkOnJob(JobIdentifier jobId)
		{
			StartWorkOnJob(jobId);
			CompleteWorkOnJobAfterDelay(jobId, 1 / GetRandom(_attributes.Value.Speed));
		}
		
		private void StartWorkOnJob(JobIdentifier jobId)
			=> _workshopCommands.Enqueue(new WorkshopCommand.StartWork(jobId));

		private void CompleteWorkOnJobAfterDelay(JobIdentifier jobId, float delaySeconds)
			=> CompleteWorkOnJobAfterDelay(jobId, Time.time, delaySeconds);

		private void CompleteWorkOnJobAfterDelay(JobIdentifier jobId, float startTime, float delaySeconds)
			=> Observable.EveryUpdate()
				.Take(TimeSpan.FromSeconds(delaySeconds))
				.Select(_ => (Time.time - startTime) / delaySeconds)
				.Select(progress => Mathf.Clamp01(progress) % 1f)
				.Subscribe(
					_displayProgress.ShowProgress,
					() => CompleteWorkOnJob(jobId)
				);

		private void CompleteWorkOnJob(JobIdentifier jobId)
			=> _workshopCommands.Enqueue(new WorkshopCommand.CompleteWork(jobId, GetRandom(_attributes.Value.Quality) * QuantityOfWork.Unit));

		private float GetRandom(FloatRange range)
			=> UnityEngine.Random.Range(range.Min, range.Max);
	}
}
