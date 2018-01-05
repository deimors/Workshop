using System;
using UniRx;
using UnityEngine;
using Workshop.Domain.Work;
using Workshop.Models;

namespace Workshop.UseCases.Work
{

	public class CompleteWorkAfterDelay : IPerformWork
	{
		private readonly TimeSpan _delay;
		private readonly IWorkOnJob _workOnJob;
		private readonly IReadJob _readModel;
		private readonly IWriteJob _writeModel;

		public CompleteWorkAfterDelay(TimeSpan delay, IWorkOnJob workOnJob, IWriteJob writeModel, IReadJob readModel)
		{
			_delay = delay;
			_workOnJob = workOnJob;
			_writeModel = writeModel;
			_readModel = readModel;
		}

		public void Perform() 
			=> _readModel.Busy
				.First()
				.Where(busy => !busy)
				.Subscribe(_ => WorkOnJob());

		private void WorkOnJob()
		{
			Debug.Log("WorkOnJob");

			_writeModel.Busy = true;

			Observable.Timer(_delay)
				.WithLatestFrom(_readModel.Value, (_, currentJob) => currentJob)
				.Select(currentJob => new { Job = currentJob, NewStatus = _workOnJob.ApplyEffort(currentJob.Status, new QuantityOfEffort()) })
				.Do(_ => _writeModel.Busy = false)
				.Subscribe(pair => _writeModel.Value = pair.Job.With(status: s => pair.NewStatus));
		}
	}
}
