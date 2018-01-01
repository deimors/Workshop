using UnityEngine;
using Workshop.Domain.Work;
using Workshop.Presentation.Jobs.Panel;
using Zenject;

namespace Workshop.Presentation.Jobs
{
	public class JobListInstaller : MonoInstaller
	{
		[SerializeField]
		private GameObject _jobPanelPrefab;

		[SerializeField]
		private GameObject _jobPanelContainer;

		public override void InstallBindings()
		{
			Container.BindFactory<JobIdentifier, IJobPanel, JobPanel.Factory>()
				.FromSubContainerResolve()
				.ByNewPrefab<JobPanel>(_jobPanelPrefab)
				.UnderTransform(_jobPanelContainer.transform);
		}
	}
}
