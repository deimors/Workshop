using System;
using UniRx;
using UnityEngine;
using Workshop.Domain.Work;

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
				.WithLatestFrom(_readModel.Status, (_, currentJob) => currentJob)
				.Select(currentJob => _workOnJob.ApplyEffort(currentJob, new QuantityOfEffort()))
				.Do(_ => _writeModel.Busy = false)
				.Subscribe(newJob => _writeModel.Status = newJob);
		}
	}
}
