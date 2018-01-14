using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Workshop.Domain.Work.Aggregates;
using Workshop.UseCases.Work;

namespace Workshop.Presentation.Workers.Panel
{
	public class JobListDropdownValidationPresenter : MonoBehaviour, IDisplayJobListDropdownValidation
	{
		[SerializeField]
		private Image _image;

		public void ShowError(WorkshopError error)
		{
			var original = _image.color;

			_image.color = Color.red;

			Observable.Timer(TimeSpan.FromSeconds(.5))
				.Subscribe(_ => _image.color = original);
		}
	}
}
