using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Workshop.Domain.Work;
using Workshop.UseCases.Work;
using Zenject;

namespace Workshop.Presentation.Workers
{
	public class WorkerListPresenter : MonoBehaviour
	{
		private WorkerPanel.Factory _workerPanelFactory;

		private readonly IDictionary<WorkerIdentifier, WorkerPanel> _workerPanels = new Dictionary<WorkerIdentifier, WorkerPanel>();

		[Inject]
		public void Initialize(IObserveWorkerList workerList, WorkerPanel.Factory workerPanelFactory)
		{
			_workerPanelFactory = workerPanelFactory;

			workerList.ObserveAdd.Subscribe(OnWorkerAdded);
		}

		private void OnWorkerAdded(WorkerIdentifier workerId)
			=> _workerPanels[workerId] = _workerPanelFactory.Create(workerId);
	}
}
