using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

		[Inject]
		public void Setup(IReadJobList readJobs, IObserveJobList observeJobs)
		{
			readJobs.Keys
				.ToObservable()
				.Subscribe(AddJobOption);

			UpdateDropdownOptions();

			observeJobs.ObserveAdd
				.Do(AddJobOption)
				.Subscribe(_ => UpdateDropdownOptions());
		}

		private void AddJobOption(JobIdentifier jobId)
		{
			_jobOptions[jobId] = new Dropdown.OptionData("Job");
		}

		private void UpdateDropdownOptions()
		{
			_jobListDropdown.ClearOptions();
			_jobListDropdown.AddOptions(_jobOptions.Values.ToList());
		}
	}
}
