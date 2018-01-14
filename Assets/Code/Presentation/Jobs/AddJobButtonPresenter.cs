using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Workshop.Actors;
using Workshop.Domain.Work;
using Workshop.Domain.Work.Aggregates;
using Zenject;

namespace Workshop.Presentation.Jobs
{
	public class AddJobButtonPresenter : MonoBehaviour
	{
		[SerializeField]
		private Button _addButton;

		[SerializeField]
		private float _quantityOfWork = 5;

		[Inject]
		public void Initialize(IEnqueueCommand<WorkshopCommand> workshopCommands) 
			=> _addButton.onClick
				.AsObservable()
				.Select(_ => CreateAddJobCommand())
				.Subscribe(workshopCommands.Enqueue);

		private WorkshopCommand CreateAddJobCommand()
			=> new WorkshopCommand.AddJob(CreateNewJob());

		private Job CreateNewJob() 
			=> new Job(new JobIdentifier(), JobStatus.Create(_quantityOfWork * QuantityOfWork.Unit));
	}
}
