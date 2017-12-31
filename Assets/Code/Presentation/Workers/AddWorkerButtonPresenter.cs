using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Workshop.Domain.Work;
using Workshop.UseCases.Work;
using Zenject;

namespace Workshop.Presentation.Workers
{
	public class AddWorkerButtonPresenter : MonoBehaviour
	{
		[SerializeField]
		private Button _addButton;
		
		[Inject]
		public void Initialize(IWriteWorkerList workerList)
		{
			_addButton.onClick
				.AsObservable()
				.Subscribe(_ => workerList.Add(new WorkerIdentifier()));
		}
	}
}
