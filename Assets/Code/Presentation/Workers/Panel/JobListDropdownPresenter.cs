using Functional.Maybe;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Workshop.Domain.Work;
using Workshop.Models;
using Workshop.UseCases.Work;
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
		
		private IWriteWorkerJobAssignment _writeAssigments;

		private readonly Dropdown.OptionData[] _noneOptionData = new[] { new Dropdown.OptionData("None") };

		[Inject]
		public void Setup(IReadJobList readJobs, IObserveJobList observeJobs, IWriteWorkerJobAssignment writeAssignments, IObserveWorkerJobAssignment observeAssignments, IGetJobDropdownOptions dropdownOptions)
		{
			_writeAssigments = writeAssignments;

			foreach (var option in dropdownOptions.Options)
				AddJobOption(option);

			UpdateDropdownOptions();

			observeJobs.ObserveAdd
				.Select(job => new JobDropdownOption(job))
				.Do(AddJobOption)
				.Subscribe(_ => UpdateDropdownOptions());

			_jobListDropdown.onValueChanged
				.AsObservable()
				.Select(selectedIndex => _jobListDropdown.options[selectedIndex])
				.Subscribe(OnOptionSelected);

			observeAssignments.Assignments
				.Where(assignments => assignments[WorkerIdentifier].IsNothing())
				.Subscribe(_ => _jobListDropdown.value = 0);
		}

		private void AddJobOption(JobDropdownOption jobOption) 
			=> _jobOptions[new Dropdown.OptionData(jobOption.ToString())] = jobOption;

		private void UpdateDropdownOptions()
		{
			_jobListDropdown.ClearOptions();
			_jobListDropdown.AddOptions(_jobOptions.Keys.ToList());
		}

		private void OnOptionSelected(Dropdown.OptionData option)
			=> _writeAssigments[WorkerIdentifier] = _jobOptions.Lookup(option).Select(jobOption => jobOption.Job);
	}
}
