﻿using UnityEngine;
using Workshop.Domain.Work;
using Workshop.UseCases.Work;
using Zenject;

namespace Workshop.Presentation.Work
{
	public class JobPanel : MonoInstaller
	{
		public class Factory : Factory<JobIdentifier, JobPanel> { }

		[Inject]
		public JobIdentifier Identifier { get; }
		
		public override void InstallBindings()
		{
			Container.Bind<JobPanel>().FromInstance(this);

			Container.Bind<IReadJob>().FromResolveGetter<IReadJobList>(jobList => jobList[Identifier]);
		}
	}
}
