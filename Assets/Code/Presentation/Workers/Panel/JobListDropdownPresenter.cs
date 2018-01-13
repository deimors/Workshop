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
		
		private bool updateLock;

		private IObservable<Maybe<JobIdentifier>> _jobSelectionObservable;

		public Maybe<JobIdentifier> SelectedJob
		{
			set
			{
				updateLock = true;
				_jobListDropdown.value = FindJobOption(value);
				updateLock = false;
			}
		}

		public void AddJobOption(JobIdentifier jobId)
			=> AddJobOption(new DropdownOption(jobId));

		public IDisposable Subscribe(IObserver<Maybe<JobIdentifier>> observer)
			=> _jobSelectionObservable.Subscribe(observer);
		
		[Inject]
		public void Initialize()
		{
			AddJobOption(DropdownOption.None);

			_jobSelectionObservable = _jobListDropdown.onValueChanged
				.AsObservable()
				.Where(_ => !updateLock)
				.Select(selectedIndex => _jobListDropdown.options[selectedIndex])
				.OfType<Dropdown.OptionData, DropdownOption>()
				.Do(_ => SelectedJob = Maybe<JobIdentifier>.Nothing)
				.Select(option => option.JobId);
		}
		
		private int FindJobOption(Maybe<JobIdentifier> maybeJobId)
			=> _jobListDropdown.options
				.OfType<DropdownOption>()
				.Select((option, index) => new { option, index })
				.Single(pair => pair.option.JobId == maybeJobId)
				.index;
		
		private void AddJobOption(DropdownOption option)
			=> _jobListDropdown.AddOptions(new List<Dropdown.OptionData> { option });	
	}
}
