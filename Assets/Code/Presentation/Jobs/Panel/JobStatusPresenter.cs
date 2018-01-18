using UnityEngine;
using UnityEngine.UI;
using Workshop.Domain.Work;
using Workshop.UseCases.Work;
using Zenject;

namespace Workshop.Presentation.Jobs.Panel
{
	public class JobStatusPresenter : MonoBehaviour, IDisplayJobStatus
	{
		[SerializeField]
		private Text _jobStatusText;

		[Inject]
		public JobIdentifier Identifier { get; }

		public string Status
		{
			set
			{
				_jobStatusText.text = value;
			}
		}
	}
}
