using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Workshop.Actors;
using Workshop.Domain.Work;
using Zenject;

namespace Workshop.Presentation.Workers
{
	public class AddWorkerButtonPresenter : MonoBehaviour
	{
		[SerializeField]
		private Button _addButton;
		
		[Inject]
		public void Initialize(IQueueWorkshopCommands queueWorkshopCommands)
		{
			_addButton.onClick
				.AsObservable()
				.Select(_ => CreateAddWorkerCommand())
				.Subscribe(queueWorkshopCommands.QueueCommand);
		}

		private WorkshopCommand CreateAddWorkerCommand()
			=> new WorkshopCommand.AddWorker(CreateNewWorker());

		private Worker CreateNewWorker()
			=> new Worker(new WorkerIdentifier());
	}
}
