using FluentAssertions;
using FluentAssertions.OneOf;
using Functional.Maybe;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Xunit2;
using Workshop.Domain.Work.Aggregates.Tests.Customizations;
using Xunit;

namespace Workshop.Domain.Work.Aggregates.Tests
{
	public class WorkshopTestFixture
	{
		private readonly Workshop _sut;

		protected static readonly IFixture StaticFixture = new Fixture().Customize(new WorkDomainCustomization());

		public WorkshopTestFixture()
		{
			AssertionOptions.AssertEquivalencyUsing(EquivalencyOptions.OneOf);

			_sut = new Workshop();
		}

		protected void Arrange_EventHistory(params WorkshopEvent[] events)
			=> _sut.LoadFromHistory(events);

		protected Maybe<WorkshopError> Act_AddWorker(WorkerIdentifier workerId)
			=> _sut.AddWorker(workerId);
		
		protected void ActAssert_AddWorkerSucceeds(WorkerIdentifier workerId)
			=> Act_AddWorker(workerId).Should().Be(Maybe<WorkshopError>.Nothing);

		protected void ActAssert_AddWorkerFailsWith(WorkerIdentifier workerId, WorkshopError expected)
			=> Act_AddWorker(workerId).Should().Be(expected.ToMaybe());

		protected Maybe<WorkshopError> Act_AddJob(Job job)
			=> _sut.AddJob(job);

		protected void ActAssert_AddJobSucceeds(Job job)
			=> Act_AddJob(job).Should().Be(Maybe<WorkshopError>.Nothing);

		protected void ActAssert_AddJobFailsWith(Job job, WorkshopError expected)
			=> Act_AddJob(job).Should().Be(expected.ToMaybe());

		protected Maybe<WorkshopError> Act_AssignJob(WorkerIdentifier workerId, JobIdentifier jobId)
			=> _sut.AssignJob(workerId, jobId);

		protected void ActAssert_AssignJobSucceeds(WorkerIdentifier workerId, JobIdentifier jobId)
			=> Act_AssignJob(workerId, jobId).Should().Be(Maybe<WorkshopError>.Nothing);

		protected void ActAssert_AssignJobFailsWith(WorkerIdentifier workerId, JobIdentifier jobId, WorkshopError expected)
			=> Act_AssignJob(workerId, jobId).Should().Be(expected.ToMaybe());

		protected void Assert_UncommittedContainsEvent(WorkshopEvent expected)
			=> _sut.UncommittedEvents.Should().Contain(expected);

		protected void Assert_UncommittedEventsContains(params WorkshopEvent[] expected)
			=> _sut.UncommittedEvents.Should().Contain(expected);
	}

	public class WhenNoWorkers : WorkshopTestFixture
	{
		[Theory, AutoData]
		public void AddWorker_Succeeds(WorkerIdentifier workerId)
		{
			ActAssert_AddWorkerSucceeds(workerId);
		}

		[Theory, AutoData]
		public void AddWorker_UncommittedContainsWorkerAddedEvent(WorkerIdentifier workerId)
		{
			Act_AddWorker(workerId);

			Assert_UncommittedContainsEvent(new WorkshopEvent.WorkerAdded(workerId));
		}

		[Theory, WorkAutoData]
		public void AddJob_Succeeds(Job job)
		{
			ActAssert_AddJobSucceeds(job);
		}

		[Theory, WorkAutoData]
		public void AddJob_UncommittedContainsJobAddedEvent(Job job)
		{
			Act_AddJob(job);

			Assert_UncommittedContainsEvent(new WorkshopEvent.JobAdded(job));
		}

		[Theory, WorkAutoData]
		public void AssignSomeWorkerToSomeJob_FailsWithUnknownWorker(WorkerIdentifier someWorker, Job someJob)
		{
			ActAssert_AssignJobFailsWith(someWorker, someJob.Id, WorkshopError.UnknownWorker);
		}
	}

	public class AfterWorkerAdded : WorkshopTestFixture
	{
		private readonly WorkerIdentifier _addedWorker = new WorkerIdentifier();

		public AfterWorkerAdded()
		{
			Arrange_EventHistory(
				new WorkshopEvent.WorkerAdded(_addedWorker)
			);
		}

		[Fact]
		public void AddAddedWorker_FailsWithWorkerAlreadyAdded()
		{
			ActAssert_AddWorkerFailsWith(_addedWorker, WorkshopError.WorkerAlreadyAdded);
		}

		[Theory, AutoData]
		public void AddAnotherWorker_Succeeds(WorkerIdentifier anotherWorker)
		{
			ActAssert_AddWorkerSucceeds(anotherWorker);
		}

		[Theory, AutoData]
		public void AddAnotherWorker_UncommittedContainsAddAnotherWorkerEvent(WorkerIdentifier anotherWorker)
		{
			Act_AddWorker(anotherWorker);

			Assert_UncommittedEventsContains(new WorkshopEvent.WorkerAdded(anotherWorker));
		}

		[Theory, WorkAutoData]
		public void AssignAddedWorkerToSomeJob_FailsWithUnknownJob(Job someJob)
		{
			ActAssert_AssignJobFailsWith(_addedWorker, someJob.Id, WorkshopError.UnknownJob);
		}
	}

	public class AfterJobAdded : WorkshopTestFixture
	{
		private readonly Job _addedJob = StaticFixture.Create<Job>();

		public AfterJobAdded()
		{
			Arrange_EventHistory(
				new WorkshopEvent.JobAdded(_addedJob)
			);
		}

		[Fact]
		public void AddAddedJob_FailsWithJobAlreadyAdded()
		{
			ActAssert_AddJobFailsWith(_addedJob, WorkshopError.JobAlreadyAdded);
		}

		[Theory, WorkAutoData]
		public void AddAnotherJob_Succeeds(Job anotherJob)
		{
			ActAssert_AddJobSucceeds(anotherJob);
		}

		[Theory, WorkAutoData]
		public void AddAnotherJob_UncommittedEventsContainsAddAnotherJobEvent(Job anotherJob)
		{
			Act_AddJob(anotherJob);

			Assert_UncommittedEventsContains(new WorkshopEvent.JobAdded(anotherJob));
		}

		[Theory, AutoData]
		public void AssignSomeWorkerToAddedJob_FailsWithUnknownWorker(WorkerIdentifier someWorker)
		{
			ActAssert_AssignJobFailsWith(someWorker, _addedJob.Id, WorkshopError.UnknownWorker);
		}
	}

	public class AfterJobAndWorkerAdded : WorkshopTestFixture
	{
		private readonly WorkerIdentifier _addedWorker = new WorkerIdentifier();
		private readonly Job _addedJob = StaticFixture.Create<Job>();

		public AfterJobAndWorkerAdded()
		{
			Arrange_EventHistory(
				new WorkshopEvent.JobAdded(_addedJob),
				new WorkshopEvent.WorkerAdded(_addedWorker)
			);
		}

		[Fact]
		public void AssignAddedJobToAddedWorker_Succeeds()
		{
			ActAssert_AssignJobSucceeds(_addedWorker, _addedJob.Id);
		}

		[Fact]
		public void AssignAddedJobToAddedWorker_UncommittedEventsContainsJobAssigned()
		{
			Act_AssignJob(_addedWorker, _addedJob.Id);

			Assert_UncommittedEventsContains(
				new WorkshopEvent.JobAssigned(_addedWorker, _addedJob.Id)
			);
		}
	}

	public class AfterJobAssignedToWorker : WorkshopTestFixture
	{
		private readonly WorkerIdentifier _addedWorker = new WorkerIdentifier();
		private readonly Job _addedJob = StaticFixture.Create<Job>();

		public AfterJobAssignedToWorker()
		{
			Arrange_EventHistory(
				new WorkshopEvent.JobAdded(_addedJob),
				new WorkshopEvent.WorkerAdded(_addedWorker),
				new WorkshopEvent.JobAssigned(_addedWorker, _addedJob.Id)
			);
		}

		[Theory, AutoData]
		public void AssignAddedJobToAnotherWorker_Succeeds(WorkerIdentifier anotherWorker)
		{
			Arrange_EventHistory(
				new WorkshopEvent.WorkerAdded(anotherWorker)
			);

			ActAssert_AssignJobSucceeds(anotherWorker, _addedJob.Id);
		}

		[Theory, AutoData]
		public void AssignAddedJobToAnotherWorker_UncommittedEventsContainsAddedWorkerUnassignedAnotherWorkerAssigned(WorkerIdentifier anotherWorker)
		{
			Arrange_EventHistory(
				new WorkshopEvent.WorkerAdded(anotherWorker)
			);

			Act_AssignJob(anotherWorker, _addedJob.Id);

			Assert_UncommittedEventsContains(
				new WorkshopEvent.JobUnassigned(_addedWorker, _addedJob.Id),
				new WorkshopEvent.JobAssigned(anotherWorker, _addedJob.Id)
			);
		}

		[Theory, WorkAutoData]
		public void AssignAnotherJobToAddedWorker_Succeeds(Job anotherJob)
		{
			Arrange_EventHistory(
				new WorkshopEvent.JobAdded(anotherJob)
			);

			ActAssert_AssignJobSucceeds(_addedWorker, anotherJob.Id);
		}

		[Theory, WorkAutoData]
		public void AssignAnotherJobToAddedWorker_UncommittedEventsContainsAddedWorkerUnassignedAnotherJobAssigned(Job anotherJob)
		{
			Arrange_EventHistory(
				new WorkshopEvent.JobAdded(anotherJob)
			);

			Act_AssignJob(_addedWorker, anotherJob.Id);

			Assert_UncommittedEventsContains(
				new WorkshopEvent.JobUnassigned(_addedWorker, _addedJob.Id),
				new WorkshopEvent.JobAssigned(_addedWorker, anotherJob.Id)
			);
		}
	}
}
