using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Workshop.Domain.Work;
using Workshop.UseCases.Work;
using Zenject;

namespace Workshop.Presentation.Workers.Panel
{
	public class PerformWorkButtonPresenter : MonoBehaviour
	{
		[SerializeField]
		private Button StartButton;
		
		[Inject]
		public void Initialize(WorkerIdentifier worker, IPerformAssignedWork performAssignedWork)
		{
			StartButton.onClick.AsObservable().Subscribe(_ => performAssignedWork.Perform(worker));
		}
	}
}