﻿using Assets.Code.UseCases.Work;
using Workshop.Domain.Work;
using Workshop.UseCases.Work;
using Zenject;

namespace Workshop.Presentation.Work
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

			Container.Bind<IReadWorker>().FromResolveGetter<IReadWorkerList>(jobList => jobList[Identifier]);
		}
	}
}
