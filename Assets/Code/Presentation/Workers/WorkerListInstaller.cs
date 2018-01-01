using UnityEngine;
using Workshop.Domain.Work;
using Workshop.Presentation.Workers.Panel;
using Zenject;

namespace Workshop.Presentation.Workers
{
	public class WorkerListInstaller : MonoInstaller
	{
		[SerializeField]
		private GameObject _workerPanelPrefab;

		[SerializeField]
		private GameObject _workerPanelContainer;

		public override void InstallBindings()
		{
			Container.BindFactory<WorkerIdentifier, WorkerPanel, WorkerPanel.Factory>()
				.FromSubContainerResolve()
				.ByNewPrefab<WorkerPanel>(_workerPanelPrefab)
				.UnderTransform(_workerPanelContainer.transform);
		}
	}
}
