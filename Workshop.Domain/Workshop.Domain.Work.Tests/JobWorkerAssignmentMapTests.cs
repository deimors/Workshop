using FluentAssertions;
using Functional.Maybe;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Xunit2;
using Xunit;

namespace Workshop.Domain.Work.Tests
{
	public class JobWorkerAssignmentMapTestFixture
	{
		protected static readonly IFixture StaticFixture = new Fixture();

	}

	public static class JobWorkerAssignmentMapTesting
	{
		public static JobWorkerAssignmentMap Act_AssignWorkerToJob(this JobWorkerAssignmentMap sut, JobIdentifier job, WorkerIdentifier worker)
			=> sut.WithAssignment(job, worker);

		public static JobWorkerAssignmentMap Act_WithoutJobAssignment(this JobWorkerAssignmentMap sut, JobIdentifier job)
			=> sut.WithoutAssignment(job);

		public static JobWorkerAssignmentMap Act_WithoutWorkerAssignment(this JobWorkerAssignmentMap sut, WorkerIdentifier worker)
			=> sut.WithoutAssignment(worker);

		public static void Assert_GetByJobReturnsNothing(this JobWorkerAssignmentMap sut, JobIdentifier job)
			=> sut[job].Should().Be(Maybe<WorkerIdentifier>.Nothing);

		public static void Assert_GetByJobReturnsWorker(this JobWorkerAssignmentMap sut, JobIdentifier job, WorkerIdentifier worker)
			=> sut[job].Should().Be(worker.ToMaybe());

		public static void Assert_GetByWorkerReturnsNothing(this JobWorkerAssignmentMap sut, WorkerIdentifier worker)
			=> sut[worker].Should().Be(Maybe<JobIdentifier>.Nothing);

		public static void Assert_GetByWorkerReturnsJob(this JobWorkerAssignmentMap sut, WorkerIdentifier worker, JobIdentifier job)
			=> sut[worker].Should().Be(job.ToMaybe());

		public static void Assert_Equals(this JobWorkerAssignmentMap sut, JobWorkerAssignmentMap other)
			=> sut.Should().Be(other);
	}

	public class WhenEmpty : JobWorkerAssignmentMapTestFixture
	{
		private readonly JobWorkerAssignmentMap _sut = JobWorkerAssignmentMap.Empty;

		[Theory, AutoData]
		public void GetByJobReturnsNothing(JobIdentifier job)
		{
			_sut.Assert_GetByJobReturnsNothing(job);
		}

		[Theory, AutoData]
		public void GetByWorkerReturnsNothing(WorkerIdentifier worker)
		{
			_sut.Assert_GetByWorkerReturnsNothing(worker);
		}

		[Theory, AutoData]
		public void WithoutJob_EqualsSut(JobIdentifier job)
		{
			_sut.Act_WithoutJobAssignment(job)
				.Assert_Equals(_sut);
		}

		[Theory, AutoData]
		public void WithoutWorker_EqualsSut(WorkerIdentifier worker)
		{
			_sut.Act_WithoutWorkerAssignment(worker)
				.Assert_Equals(_sut);
		}
	}

	public class WhenWorkerAssignedToJob : JobWorkerAssignmentMapTestFixture
	{
		private readonly JobIdentifier _assignedJob = StaticFixture.Create<JobIdentifier>();
		private readonly WorkerIdentifier _assignedWorker = StaticFixture.Create<WorkerIdentifier>();

		private readonly JobWorkerAssignmentMap _sut;

		public WhenWorkerAssignedToJob()
		{
			_sut = JobWorkerAssignmentMap.Empty
				.Act_AssignWorkerToJob(_assignedJob, _assignedWorker);
		}

		[Fact]
		public void AssignWorkerToJob_GetByAssignedJobReturnsAssignedWorker()
		{
			_sut.Assert_GetByJobReturnsWorker(_assignedJob, _assignedWorker);
		}

		[Fact]
		public void AssignWorkerToJob_GetByAssignedWorkerReturnsJob()
		{
			_sut.Assert_GetByWorkerReturnsJob(_assignedWorker, _assignedJob);
		}

		[Theory, AutoData]
		public void GetByOtherJobReturnsNothing(JobIdentifier otherJob)
		{
			_sut.Assert_GetByJobReturnsNothing(otherJob);
		}

		[Theory, AutoData]
		public void GetByOtherWorkerReturnsNothing(WorkerIdentifier otherWorker)
		{
			_sut.Assert_GetByWorkerReturnsNothing(otherWorker);
		}

		[Fact]
		public void WithoutAssignmentOfAssignedJob_GetByAssignedJobReturnsNothing()
		{
			_sut.Act_WithoutJobAssignment(_assignedJob)
				.Assert_GetByJobReturnsNothing(_assignedJob);
		}

		[Fact]
		public void WithoutAssignmentOfAssignedJob_GetByAssignedWorkerReturnsNothing()
		{
			_sut.Act_WithoutJobAssignment(_assignedJob)
				.Assert_GetByWorkerReturnsNothing(_assignedWorker);
		}

		[Fact]
		public void WithoutAssignmentOfAssignedWorker_GetByAssignedWorkerReturnsNothing()
		{
			_sut.Act_WithoutWorkerAssignment(_assignedWorker)
				.Assert_GetByWorkerReturnsNothing(_assignedWorker);
		}

		[Fact]
		public void WithoutAssignmentOfAssignedWorker_GetByAssignedJobReturnsNothing()
		{
			_sut.Act_WithoutWorkerAssignment(_assignedWorker)
				.Assert_GetByJobReturnsNothing(_assignedJob);
		}

		[Theory, AutoData]
		public void AssignOtherWorkerToJob_GetByAssignedWorkerReturnsNothing(WorkerIdentifier otherWorker)
		{
			_sut.Act_AssignWorkerToJob(_assignedJob, otherWorker)
				.Assert_GetByWorkerReturnsNothing(_assignedWorker);
		}

		[Theory, AutoData]
		public void AssignOtherWorkerToJob_GetByAssignedJobReturnsOtherWorker(WorkerIdentifier otherWorker)
		{
			_sut.Act_AssignWorkerToJob(_assignedJob, otherWorker)
				.Assert_GetByJobReturnsWorker(_assignedJob, otherWorker);
		}

		[Theory, AutoData]
		public void AssignOtherJobToWorker_GetByAssignedWorkerReturnsOtherJob(JobIdentifier otherJob)
		{
			_sut.Act_AssignWorkerToJob(otherJob, _assignedWorker)
				.Assert_GetByWorkerReturnsJob(_assignedWorker, otherJob);
		}

		[Theory, AutoData]
		public void AssignOtherJobToWorker_GetByAssignedJobReturnsNothing(JobIdentifier otherJob)
		{
			_sut.Act_AssignWorkerToJob(otherJob, _assignedWorker)
				.Assert_GetByJobReturnsNothing(_assignedJob);
		}
	}
}
