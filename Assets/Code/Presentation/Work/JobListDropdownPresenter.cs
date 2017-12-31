using Functional.Maybe;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Workshop.Domain.Work;
using Workshop.UseCases.Work;
using Zenject;

namespace Workshop.Presentation.Work
{
	public class JobListDropdownPresenter : MonoBehaviour
	{
		[SerializeField]
		private Dropdown _jobListDropdown;

		private readonly IDictionary<JobIdentifier, Dropdown.OptionData> _jobOptions = new Dictionary<JobIdentifier, Dropdown.OptionData>();

		private readonly WorkerIdentifier workerIdentifier = new WorkerIdentifier();

		private IWriteWorkerJobAssignment _writeAssigments;

		private readonly Dropdown.OptionData[] _noneOptionData = new[] { new Dropdown.OptionData("None") };

		[Inject]
		public void Setup(IReadJobList readJobs, IObserveJobList observeJobs, IWriteWorkerJobAssignment writeAssignments)
		{
			_writeAssigments = writeAssignments;

			readJobs.Keys
				.ToObservable()
				.Subscribe(AddJobOption);

			UpdateDropdownOptions();

			observeJobs.ObserveAdd
				.Do(AddJobOption)
				.Subscribe(_ => UpdateDropdownOptions());

			_jobListDropdown.onValueChanged
				.AsObservable()
				.Select(selectedIndex => _jobListDropdown.options[selectedIndex])
				.Subscribe(OnOptionSelected);
		}

		private void AddJobOption(JobIdentifier job)
		{
			_jobOptions[job] = new Dropdown.OptionData(job.ToString());
		}

		private void UpdateDropdownOptions()
		{
			_jobListDropdown.ClearOptions();
			_jobListDropdown.AddOptions(_noneOptionData.Concat(_jobOptions.Values).ToList());
		}

		private void OnOptionSelected(Dropdown.OptionData option)
			=> GetJobByOption(option)
				.Match(AssignToJob, RemoveAssignment);

		private Maybe<JobIdentifier> GetJobByOption(Dropdown.OptionData option) 
			=> _jobOptions
				.SingleMaybe(pair => pair.Value == option)
				.Select(pair => pair.Key);

		private void AssignToJob(JobIdentifier job)
			=> _writeAssigments[workerIdentifier] = job.ToMaybe();

		private void RemoveAssignment() 
			=> _writeAssigments[workerIdentifier] = Maybe<JobIdentifier>.Nothing;
	}
}
