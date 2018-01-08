using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Workshop.Domain.Work;
using Workshop.Domain.Work.Aggregates;
using Workshop.Presentation.Jobs.Panel;
using Zenject;

namespace Workshop.Presentation.Jobs
{
	public class JobListPresenter : MonoBehaviour
	{
		private JobPanel.Factory _jobPanelFactory;

		private readonly IDictionary<JobIdentifier, IJobPanel> _jobPanels = new Dictionary<JobIdentifier, IJobPanel>();

		[Inject]
		public void Initialize(IObservable<WorkshopEvent> workshopEvents, JobPanel.Factory jobPanelFactory)
		{
			_jobPanelFactory = jobPanelFactory;

			workshopEvents
				.OfType<WorkshopEvent, WorkshopEvent.JobAdded>()
				.Subscribe(jobAdded => OnJobAdded(jobAdded.Job.Id));
		}

		private void OnJobAdded(JobIdentifier jobId) 
			=> _jobPanels[jobId] = _jobPanelFactory.Create(jobId);
	}
}
