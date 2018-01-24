﻿using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Workshop.Actors;
using Workshop.Domain.Work;
using Workshop.Domain.Work.Aggregates;
using Zenject;

namespace Workshop.Presentation.Workers
{
	public class AddWorkerButtonPresenter : MonoBehaviour
	{
		[SerializeField]
		private Button _addButton;
		
		[Inject]
		public void Initialize(IEnqueueCommand<WorkshopCommand> workshopCommands)
		{
			_addButton.onClick
				.AsObservable()
				.Select(_ => CreateAddWorkerCommand())
				.Subscribe(workshopCommands.Enqueue);
		}

		private WorkshopCommand CreateAddWorkerCommand()
			=> new WorkshopCommand.AddWorker(CreateNewWorker());

		private Worker CreateNewWorker()
			=> Worker.NewDefault().With(attributes: _ => RandomAttributes());

		private WorkerAttributes RandomAttributes()
			=> new WorkerAttributes(RandomRange(), RandomRange());

		private FloatRange RandomRange()
			=> new FloatRange(Random.Range(0, 0.5f), Random.Range(0.5f, 1));
	}
}
