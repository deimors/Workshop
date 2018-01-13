using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Workshop.UseCases.Work;

namespace Workshop.Presentation.Workers.Panel
{
	public class WorkButtonPresenter : MonoBehaviour, IWorkButtonClickedObservable, IDisplayWorkButtonInteractable
	{
		[SerializeField]
		private Button _workButton;

		public bool Interactable
		{
			set
			{
				_workButton.interactable = value;
			}
		}
		
		public IDisposable Subscribe(IObserver<Unit> observer)
			=> _workButton.onClick.AsObservable().Subscribe(observer);
	}	
}