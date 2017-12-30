using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Workshop.UseCases.Work;
using Zenject;

namespace Workshop.Presentation.Work
{
	public class PerformWorkButtonPresenter : MonoBehaviour
	{
		[SerializeField]
		private Button StartButton;

		[Inject]
		public void Initialize(IPerformWork performWork, IReadJob jobState)
		{
			StartButton.onClick.AsObservable().Subscribe(_ => performWork.Perform());

			jobState.Busy.Subscribe(busy => StartButton.interactable = !busy);
		}
	}
}