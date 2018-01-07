﻿using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using Workshop.Domain.Work;

namespace Workshop.Models
{
	public class WorkerList : IReadWorkerList, IWriteWorkerList, IObserveWorkerList
	{
		private IReactiveDictionary<WorkerIdentifier, Worker> _workers = new ReactiveDictionary<WorkerIdentifier, Worker>();

		public IReadWorker this[WorkerIdentifier identifier]
		{
			get
			{
				Worker worker;

				if (((IReadOnlyReactiveDictionary<WorkerIdentifier, Worker>)_workers).TryGetValue(identifier, out worker))
					return worker;
				else
					throw new KeyNotFoundException($"{identifier} not found");
			}
		}

		public IEnumerable<WorkerIdentifier> Keys => _workers.Keys;

		public IEnumerable<IReadWorker> Values => _workers.Values;

		public IObservable<WorkerIdentifier> ObserveAdd
			=> _workers.ObserveAdd().Select(pair => pair.Key);

		public IObservable<WorkerIdentifier> ObserveRemove
			=> _workers.ObserveRemove().Select(pair => pair.Key);

		public void Add(WorkerIdentifier worker)
			=> _workers.Add(worker, new Worker());

		public void Remove(WorkerIdentifier worker)
			=> _workers.Remove(worker);
	}
}