using Assets.Code.UseCases.Work;
using System;
using Workshop.Domain.Work;
using Workshop.UseCases.Work;
using Zenject;

namespace Workshop.Presentation.Workers.Panel
{
	public class PerformWorkFactory : Factory<IWriteJob, IReadJob, IPerformWork> { }

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

			Container.BindIFactory<IPerformWork>()
				.To<CompleteWorkAfterDelay>()
				.WithArguments(TimeSpan.FromSeconds(1));
		}
	}
}
