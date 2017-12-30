using System;
using Workshop.Domain.Work;
using Workshop.Domain.Work.Concrete;
using Workshop.UseCases.Work;
using Zenject;

namespace Workshop.Presentation
{
	public class SceneInstaller : MonoInstaller
	{
		public override void InstallBindings()
		{
			Container.Bind<IPerformWork>()
				.To<CompleteWorkAfterDelay>()
				.AsSingle()
				.WithArguments(TimeSpan.FromSeconds(1));

			Container
				.BindInterfacesTo<Job>()
				.AsSingle()
				.WithArguments(new JobStatus(new JobIdentifier(), 5 * QuantityOfWork.Unit, QuantityOfWork.None));

			Container.Bind<IWorkOnJob>()
				.To<ApplyConstantWorkToJob>()
				.AsSingle()
				.WithArguments(QuantityOfWork.Unit);

			Container
				.BindInterfacesTo<JobList>()
				.AsSingle();
		}
	}
}
