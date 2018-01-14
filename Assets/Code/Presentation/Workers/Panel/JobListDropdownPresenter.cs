using Functional.Maybe;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Workshop.Domain.Work;
using Workshop.UseCases.Work;
using Zenject;

namespace Workshop.Presentation.Workers.Panel
{
	public class JobListDropdownPresenter : MonoBehaviour, IDisplaySelectedJob, IAddJobListDropdownOption, IJobSelectedObservable
	{
		private class DropdownOption : Dropdown.OptionData
		{
			public static readonly DropdownOption None = new DropdownOption();

			public Maybe<JobIdentifier> JobId { get; }

			private DropdownOption() : base("None")
			{
				JobId = Maybe<JobIdentifier>.Nothing;
			}

			public DropdownOption(JobIdentifier jobId) : base(jobId.ToString())
			{
				JobId = jobId.ToMaybe();
			}
		}

		[SerializeField]
		private Dropdown _jobListDropdown;

		private IObservable<Maybe<JobIdentifier>> _jobSelectionObservable;

		private Maybe<JobIdentifier> _selectedJob = Maybe<JobIdentifier>.Nothing;

		[Inject]
		public void Initialize()
		{
			AddJobOption(DropdownOption.None);

			_jobSelectionObservable = BuildJobSelectionObservable();
		}

		public Maybe<JobIdentifier> SelectedJob
		{
			set
			{
				FindJobOption(value).Match(
					index => 
					{
						_selectedJob = value;
						_jobListDropdown.value = index;
					},
					() => Debug.LogError($"Could not find Job Option for {value}")
				);
			}
		}
		
		public void AddJobOption(JobIdentifier jobId)
			=> AddJobOption(new DropdownOption(jobId));

		public IDisposable Subscribe(IObserver<Maybe<JobIdentifier>> observer)
			=> _jobSelectionObservable.Subscribe(observer);
		
		private IObservable<Maybe<JobIdentifier>> BuildJobSelectionObservable() 
			=> _jobListDropdown.onValueChanged
				.AsObservable()
				.Select(selectedIndex => _jobListDropdown.options[selectedIndex])
				.OfType<Dropdown.OptionData, DropdownOption>()
				.Where(option => option.JobId != _selectedJob)
				.Do(_ => SelectedJob = _selectedJob)
				.Select(option => option.JobId);

		private Maybe<int> FindJobOption(Maybe<JobIdentifier> maybeJobId)
			=> _jobListDropdown.options
				.OfType<DropdownOption>()
				.Select((option, index) => new { option, index })
				.SingleMaybe(pair => pair.option.JobId == maybeJobId)
				.Select(pair => pair.index);
		
		private void AddJobOption(DropdownOption option)
			=> _jobListDropdown.AddOptions(new List<Dropdown.OptionData> { option });	
	}
}
