using UnityEngine;
using Workshop.Domain.Work;
using Zenject;

namespace Workshop.Presentation.Work
{
	public class JobListInstaller : MonoInstaller
	{
		[SerializeField]
		private GameObject _jobPanelPrefab;

		[SerializeField]
		private GameObject _jobPanelContainer;

		public override void InstallBindings()
		{
			Container.BindFactory<JobIdentifier, JobPanel, JobPanel.Factory>()
				.FromSubContainerResolve()
				.ByNewPrefab<JobPanel>(_jobPanelPrefab)
				.UnderTransform(_jobPanelContainer.transform);
		}
	}
}
