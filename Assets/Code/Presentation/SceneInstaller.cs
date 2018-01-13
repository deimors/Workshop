using System;
using UniRx;
using Workshop.Actors;
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
			Container
				.BindInterfacesTo<WorkshopActor>()
				.AsSingle()
				.WithArguments(Observable.EveryUpdate().AsUnitObservable());
		}
	}
}
