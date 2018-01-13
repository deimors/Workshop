using UnityEngine;
using UnityEngine.UI;
using Workshop.UseCases.Work;

namespace Workshop.Presentation.Jobs.Panel
{
	public class WorkCompletedSliderPresenter : MonoBehaviour, IDisplayJobCompletion
	{
		[SerializeField]
		private Slider _completedSlider;

		public float PercentComplete
		{
			set
			{
				_completedSlider.value = value;
			}
		}
	}
}
