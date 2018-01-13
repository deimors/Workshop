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
using Workshop.UseCases.Work;
using Zenject;

namespace Workshop.Presentation.Workers.Panel
{
	public class JobListDropdownPresenter : MonoBehaviour, IDisplaySelectedJob, IAddJobListDropdownOption
	{
		[SerializeField]
		private Dropdown _jobListDropdown;

		[Inject]
		public WorkerIdentifier WorkerIdentifier { get; }

		private bool updateLock;

		public Maybe<JobIdentifier> SelectedJob
		{
			set
			{
				updateLock = true;
				_jobListDropdown.value = FindJobOption(value);
				updateLock = false;
			}
		}
		
		[Inject]
		public void Initialize(IObservable<WorkshopEvent> workshopEvents, IQueueWorkshopCommands queueWorkshopCommands)
		{
			AddJobOption(JobListDropdownOption.None);
			
			_jobListDropdown.onValueChanged
				.AsObservable()
				.Where(_ => !updateLock)
				.Select(selectedIndex => _jobListDropdown.options[selectedIndex])
				.OfType<Dropdown.OptionData, JobListDropdownOption>()
				.Select(option => option.JobId)
				.Select(BuildJobSelectionCommand)
				.Do(_ => SelectedJob = Maybe<JobIdentifier>.Nothing)
				.Subscribe(queueWorkshopCommands.QueueCommand);
		}
		
		private int FindJobOption(Maybe<JobIdentifier> maybeJobId)
			=> _jobListDropdown.options
				.OfType<JobListDropdownOption>()
				.Select((option, index) => new { option, index })
				.Single(pair => pair.option.JobId == maybeJobId)
				.index;
		
		public void AddJobOption(JobIdentifier jobId)
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
