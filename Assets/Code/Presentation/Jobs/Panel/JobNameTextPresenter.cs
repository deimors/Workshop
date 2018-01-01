using UnityEngine;
using UnityEngine.UI;
using Workshop.Domain.Work;
using Zenject;

namespace Workshop.Presentation.Jobs.Panel
{
	public class JobNameTextPresenter : MonoBehaviour
	{
		[SerializeField]
		private Text _jobNameText;

		[Inject]
		public void Setup(JobIdentifier job)
			=> _jobNameText.text = job.ToString();
	}
}
