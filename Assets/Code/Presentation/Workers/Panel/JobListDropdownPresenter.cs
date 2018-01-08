using Functional.Maybe;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Workshop.Actors;
using Workshop.Domain.Work;
using Workshop.Domain.Work.Aggregates;
using Zenject;

namespace Workshop.Presentation.Workers.Panel
{
	public class JobListDropdownPresenter : MonoBehaviour
	{
		[SerializeField]
		private Dropdown _jobListDropdown;

		[Inject]
		public WorkerIdentifier WorkerIdentifier { get; }
		
		private bool updateLock;

		[Inject]
		public void Initialize(IObservable<WorkshopEvent> workshopEvents, IQueueWorkshopCommands queueWorkshopCommands)
		{
			AddJobOption(JobListDropdownOption.None);
			
			workshopEvents
				.OfType<WorkshopEvent, WorkshopEvent.JobAdded>()
				.Select(jobAdded => jobAdded.Job.Id)
				.Subscribe(AddJobOption);

			workshopEvents
				.OfType<WorkshopEvent, WorkshopEvent.JobUnassigned>()
				.Where(jobUnassigned => jobUnassigned.WorkerId == WorkerIdentifier)
				.Subscribe(_ => SelectNoneOption());

			_jobListDropdown.onValueChanged
				.AsObservable()
				.Where(_ => !updateLock)
				.Select(selectedIndex => _jobListDropdown.options[selectedIndex])
				.OfType<Dropdown.OptionData, JobListDropdownOption>()
				.Select(option => option.JobId)
				.Select(BuildJobSelectionCommand)
				.Subscribe(queueWorkshopCommands.QueueCommand);
		}
		
		private void SelectNoneOption()
		{
			updateLock = true;
			_jobListDropdown.value = 0;
			updateLock = false;
		}

		private void AddJobOption(JobIdentifier jobId)
			=> AddJobOption(new JobListDropdownOption(jobId));


		private void AddJobOption(JobListDropdownOption option)
			=> _jobListDropdown.AddOptions(new List<Dropdown.OptionData> { option });
		
		private WorkshopCommand BuildJobSelectionCommand(Maybe<JobIdentifier> maybeJobId)
			=> maybeJobId.SelectOrElse<JobIdentifier, WorkshopCommand>(
				jobId => new WorkshopCommand.AssignJob(jobId, WorkerIdentifier),
				() => new WorkshopCommand.UnassignWorker(WorkerIdentifier)
			);
	}
}
