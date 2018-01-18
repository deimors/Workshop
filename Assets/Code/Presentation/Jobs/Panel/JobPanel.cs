using Workshop.Domain.Work;
using Workshop.UseCases.Work;
using Zenject;

namespace Workshop.Presentation.Jobs.Panel
{
	public interface IJobPanel { }

	public class JobPanel : MonoInstaller, IJobPanel
	{
		public class Factory : Factory<JobIdentifier, IJobPanel> { }

		[Inject]
		public JobIdentifier Identifier { get; }
		
		public override void InstallBindings()
		{
			Container.Bind<IJobPanel>().FromInstance(this);

			Container.BindInstance(Identifier);

			Container.Bind<UpdateJobStatusDisplayWhenJobAssigned>().AsSingle().NonLazy();
			Container.Bind<UpdateJobStatusDisplayWhenJobAddedOrUnassigned>().AsSingle().NonLazy();
			Container.Bind<UpdateJobCompletionWhenJobAddedOrStatusUpdated>().AsSingle().NonLazy();
		}
	}
}
