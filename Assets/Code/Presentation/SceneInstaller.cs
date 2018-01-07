using System;
using UniRx;
using Workshop.Actors;
using Workshop.Domain.Work;
using Workshop.Domain.Work.Concrete;
using Workshop.Models;
using Workshop.UseCases.Work;
using Zenject;

namespace Workshop.Presentation
{
	public class SceneInstaller : MonoInstaller
	{
		public override void InstallBindings()
		{
			Container.Bind<IWorkOnJob>()
				.To<ApplyConstantWorkToJob>()
				.AsSingle()
				.WithArguments(QuantityOfWork.Unit);

			Container
				.BindInterfacesTo<PerformAssignedWork>()
				.AsSingle();

			Container.BindFactory<IWriteJob, IReadJob, IPerformWork, PerformWorkFactory>()
				.To<CompleteWorkAfterDelay>()
				.WithArguments(TimeSpan.FromSeconds(1));

			Container
				.BindInterfacesTo<JobList>()
				.AsSingle();

			Container
				.BindInterfacesTo<WorkerList>()
				.AsSingle();

			Container
				.BindInterfacesTo<AssignmentMap>()
				.AsSingle();

			Container
				.BindInterfacesTo<WorkshopActor>()
				.AsSingle()
				.WithArguments(Observable.EveryUpdate().AsUnitObservable());
		}
	}
}
