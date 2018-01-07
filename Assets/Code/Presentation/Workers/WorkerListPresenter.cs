using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Workshop.Domain.Work;
using Workshop.Domain.Work.Aggregates;
using Workshop.Models;
using Workshop.Presentation.Workers.Panel;
using Zenject;
using System;

namespace Workshop.Presentation.Workers
{
	public class WorkerListPresenter : MonoBehaviour
	{
		private WorkerPanel.Factory _workerPanelFactory;

		private readonly IDictionary<WorkerIdentifier, WorkerPanel> _workerPanels = new Dictionary<WorkerIdentifier, WorkerPanel>();

		[Inject]
		public void Initialize(IObservable<WorkshopEvent> workshopEvents, WorkerPanel.Factory workerPanelFactory)
		{
			_workerPanelFactory = workerPanelFactory;

			workshopEvents
				.OfType<WorkshopEvent, WorkshopEvent.WorkerAdded>()
				.Subscribe(workerAdded => OnWorkerAdded(workerAdded.Worker.Id));
		}

		private void OnWorkerAdded(WorkerIdentifier workerId)
			=> _workerPanels[workerId] = _workerPanelFactory.Create(workerId);
	}
}
