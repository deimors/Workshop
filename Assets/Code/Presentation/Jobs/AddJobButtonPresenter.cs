using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Workshop.Domain.Work;
using Workshop.UseCases.Work;
using Zenject;

namespace Workshop.Presentation.Jobs
{
	public class AddJobButtonPresenter : MonoBehaviour
	{
		[SerializeField]
		private Button _addButton;

		[SerializeField]
		private float _quantityOfWork = 5;

		[Inject]
		public void Initialize(IWriteJobList jobList)
		{
			_addButton.onClick
				.AsObservable()
				.Subscribe(_ => jobList.Add(JobStatus.Create(new JobIdentifier(), _quantityOfWork * QuantityOfWork.Unit)));
		}
	}
}
