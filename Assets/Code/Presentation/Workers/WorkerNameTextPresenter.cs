using UnityEngine;
using UnityEngine.UI;
using Workshop.Domain.Work;
using Zenject;

namespace Workshop.Presentation.Workers
{
	public class WorkerNameTextPresenter : MonoBehaviour
	{
		[SerializeField]
		private Text _workerNameText;

		[Inject]
		public void Setup(WorkerIdentifier worker)
			=> _workerNameText.text = worker.ToString();
	}
}
