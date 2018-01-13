using UnityEngine;
using UnityEngine.UI;
using Workshop.Domain.Work;
using Workshop.UseCases.Work;
using Zenject;

namespace Workshop.Presentation.Jobs.Panel
{
	public class AssignedWorkTextPresenter : MonoBehaviour, IDisplayAssignedWork
	{
		[SerializeField]
		private Text _assignedWorkText;

		[Inject]
		public JobIdentifier Identifier { get; }

		public string AssignedWork
		{
			set
			{
				_assignedWorkText.text = value;
			}
		}
	}
}
