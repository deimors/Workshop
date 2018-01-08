using System;
using UniRx;
using Workshop.Domain.Work;

namespace Workshop.UseCases.Work
{
	public interface IPerformWork
	{
		void Perform();
	}
}