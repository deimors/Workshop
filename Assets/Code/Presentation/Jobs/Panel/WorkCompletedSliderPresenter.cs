using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Workshop.UseCases.Work;
using Zenject;

namespace Workshop.Presentation.Jobs.Panel
{
    public class WorkCompletedSliderPresenter : MonoBehaviour
    {
        [SerializeField]
        private Slider _completedSlider;

        [Inject]
        public void Initialize(IReadJob workModel)
        {
            workModel.Status
                .Select(job => job.Completed / job.Total)
                .Subscribe(percentage => _completedSlider.value = percentage);
        }
    }
}
