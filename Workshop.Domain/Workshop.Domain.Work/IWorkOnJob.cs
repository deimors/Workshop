namespace Workshop.Domain.Work
{
    public interface IWorkOnJob
    {
        JobStatus ApplyEffort(JobStatus job, QuantityOfEffort effort);
    }
}
