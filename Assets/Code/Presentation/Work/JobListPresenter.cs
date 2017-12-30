using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Workshop.Domain.Work;
using Workshop.UseCases.Work;
using Zenject;

namespace Workshop.Presentation.Work
{
	public class JobListPresenter : MonoBehaviour
	{
		private JobPanel.Factory _jobPanelFactory;

		private readonly IDictionary<JobIdentifier, JobPanel> _jobPanels = new Dictionary<JobIdentifier, JobPanel>();

		[Inject]
		public void Initialize(IObserveJobList jobList, JobPanel.Factory jobPanelFactory)
		{
			_jobPanelFactory = jobPanelFactory;

			jobList.ObserveAdd.Subscribe(OnJobAdded);
		}

		private void OnJobAdded(JobIdentifier jobId) 
			=> _jobPanels[jobId] = _jobPanelFactory.Create(jobId);
	}
}
