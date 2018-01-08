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

		private readonly IDictionary<Dropdown.OptionData, JobDropdownOption> _jobOptions = new Dictionary<Dropdown.OptionData, JobDropdownOption>();

		private bool updateLock;

		[Inject]
		public void NewSetup(IObservable<WorkshopEvent> workshopEvents, IQueueWorkshopCommands queueWorkshopCommands)
		{
			AddJobOption(JobDropdownOption.None);

			UpdateDropdownOptions();

			workshopEvents
				.OfType<WorkshopEvent, WorkshopEvent.JobAdded>()
				.Do(jobAdded => AddJobOption(new JobDropdownOption(jobAdded.Job.Id)))
				.Subscribe(_ => UpdateDropdownOptions());

			workshopEvents
				.OfType<WorkshopEvent, WorkshopEvent.JobUnassigned>()
				.Where(jobUnassigned => jobUnassigned.WorkerId == WorkerIdentifier)
				.Subscribe(_ => SelectNoneOption());

			_jobListDropdown.onValueChanged
				.AsObservable()
				.Where(_ => !updateLock)
				.Select(selectedIndex => _jobListDropdown.options[selectedIndex])
				.Select(GetJobIdentifierFromOptionData)
				.Select(BuildSelectionCommand)
				.Subscribe(queueWorkshopCommands.QueueCommand);
		}

		private void SelectNoneOption()
		{
			updateLock = true;
			_jobListDropdown.value = 0;
			updateLock = false;
		}
		
		private void AddJobOption(JobDropdownOption jobOption)
			=> _jobOptions[new Dropdown.OptionData(jobOption.ToString())] = jobOption;

		private void UpdateDropdownOptions()
		{
			_jobListDropdown.ClearOptions();
			_jobListDropdown.AddOptions(_jobOptions.Keys.ToList());
		}
		
		private WorkshopCommand CreateAssignJobCommand(JobIdentifier jobIdentifier)
			=> new WorkshopCommand.AssignJob(jobIdentifier, WorkerIdentifier);

		private Maybe<JobIdentifier> GetJobIdentifierFromOptionData(Dropdown.OptionData option) 
			=> _jobOptions.Lookup(option).Select(jobOption => jobOption.Job);

		private WorkshopCommand BuildSelectionCommand(Maybe<JobIdentifier> maybeJobId)
			=> maybeJobId.SelectOrElse<JobIdentifier, WorkshopCommand>(
				jobId => new WorkshopCommand.AssignJob(jobId, WorkerIdentifier),
				() => new WorkshopCommand.UnassignWorker(WorkerIdentifier)
			);
	}
}
