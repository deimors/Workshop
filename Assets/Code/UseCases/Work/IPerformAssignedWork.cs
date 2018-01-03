using Functional.Maybe;
using Workshop.Domain.Work;
using Workshop.Models;
using Zenject;

namespace Workshop.UseCases.Work
{
	public class PerformWorkFactory : Factory<IWriteJob, IReadJob, IPerformWork> { }

	public interface IPerformAssignedWork
	{
		void Perform(WorkerIdentifier worker);
	}

	public class PerformAssignedWork : IPerformAssignedWork
	{
		private readonly IReadWorkerJobAssignment _readAssignments;
		private readonly IReadJobList _readJobs;
		private readonly IWriteJobList _writeJobs;
		private readonly PerformWorkFactory _performWorkFactory;

		public PerformAssignedWork(IReadWorkerJobAssignment readAssignments, IReadJobList readJobs, IWriteJobList writeJobs, PerformWorkFactory performWorkFactory)
		{
			_readAssignments = readAssignments;
			_readJobs = readJobs;
			_writeJobs = writeJobs;
			_performWorkFactory = performWorkFactory;
		}

		public void Perform(WorkerIdentifier worker) 
			=> _readAssignments[worker].Match(PerformJob, () => { });

		private void PerformJob(JobIdentifier job) 
			=> _performWorkFactory.Create(_writeJobs[job], _readJobs[job]).Perform();
	}
}
