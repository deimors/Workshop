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
	public class WorkButtonPresenter : MonoBehaviour, IWorkButtonClickedObservable
	{
		[SerializeField]
		private Button _workButton;
		
		[Inject]
		public void Initialize(WorkerIdentifier workerId, IObservable<WorkshopEvent> workshopEvents)
		{
			workshopEvents
				.OfType<WorkshopEvent, WorkshopEvent.WorkerStatusUpdated>()
				.Where(workerStatusUpdated => workerStatusUpdated.WorkerId == workerId)
				.Select(workerStatusUpdated => workerStatusUpdated.NewStatus.Busy)
				.Subscribe(busy => _workButton.interactable = !busy);
		}

		public IDisposable Subscribe(IObserver<Unit> observer)
			=> _workButton.onClick.AsObservable().Subscribe(observer);
	}	
}