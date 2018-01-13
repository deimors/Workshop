﻿using Workshop.Domain.Work;
using Workshop.UseCases.Work;
using Zenject;

namespace Workshop.Presentation.Workers.Panel
{
	public class WorkerPanel : MonoInstaller
	{
		public class Factory : Factory<WorkerIdentifier, WorkerPanel> { }

		[Inject]
		public WorkerIdentifier Identifier { get; }

		public override void InstallBindings()
		{
			Container.Bind<WorkerPanel>().FromInstance(this);

			Container.BindInstance(Identifier);

			Container.Bind<AssignedJobReadModel>().AsSingle();
			Container.Bind<WorkOnAssignedJobWhenWorkButtonClicked>().AsSingle().NonLazy();
		}
	}
}
