using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Workshop.Domain.Work;
using Workshop.Domain.Work.Aggregates;
using Workshop.UseCases.Work;
using Zenject;

namespace Workshop.Presentation.Workers.Panel
{
	public class PerformWorkButtonPresenter : MonoBehaviour
	{
		[SerializeField]
		private Button _performWorkButton;
		
		[Inject]
		public void Initialize(WorkerIdentifier workerId, IPerformAssignedWork performAssignedWork, IObservable<WorkshopEvent> workshopEvents)
		{
			_performWorkButton
				.onClick
				.AsObservable()
				.Subscribe(_ => performAssignedWork.Perform(workerId));

			workshopEvents
				.OfType<WorkshopEvent, WorkshopEvent.WorkerStatusUpdated>()
				.Where(workerStatusUpdated => workerStatusUpdated.WorkerId == workerId)
				.Select(workerStatusUpdated => workerStatusUpdated.NewStatus.Busy)
				.Subscribe(busyStatus => _performWorkButton.interactable = !busyStatus);
		}
	}	
}