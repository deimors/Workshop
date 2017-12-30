using System;

namespace Workshop.Domain.Work.Concrete
{
    public class ApplyConstantWorkToJob : IWorkOnJob
    {
        private readonly QuantityOfWork _workIncrement;

        public ApplyConstantWorkToJob(QuantityOfWork workIncrement)
        {
            _workIncrement = workIncrement ?? throw new ArgumentNullException(nameof(workIncrement));
        }

        public JobStatus ApplyEffort(JobStatus job, QuantityOfEffort effort)
            => job.With(completed: prev => IncrementCompleted(job, prev));

        private QuantityOfWork IncrementCompleted(JobStatus job, QuantityOfWork prev) 
            => (prev + _workIncrement).Clamp(QuantityOfWork.None, job.Total);
    }
}
