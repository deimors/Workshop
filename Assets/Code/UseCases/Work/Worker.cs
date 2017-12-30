using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.Domain.Work;

namespace Assets.Code.UseCases.Work
{
	public interface IReadWorker
	{
		IObservable<JobIdentifier> CurrentJob { get; }
	}

	public class Worker
	{
	}
}
