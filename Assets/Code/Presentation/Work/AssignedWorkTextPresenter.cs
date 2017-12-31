﻿using Functional.Maybe;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Workshop.Domain.Work;
using Workshop.UseCases.Work;
using Zenject;

namespace Workshop.Presentation.Work
{
	public class AssignedWorkTextPresenter : MonoBehaviour
	{
		[SerializeField]
		private Text _assignedWorkText;

		[Inject]
		public JobIdentifier Identifier { get; }

		[Inject]
		public void Setup(JobIdentifier job, IObserveWorkerJobAssignment observeAssignment)
		{
			observeAssignment.Assignments
				.SubscribeToText(_assignedWorkText, GetAssignedWorkString);
		}

		private string GetAssignedWorkString(JobWorkerAssignmentMap assigments)
			=> assigments[Identifier].SelectOrElse(
				worker => $"Assigned to {worker}",
				() => "Unassigned"
			);
	}
}
