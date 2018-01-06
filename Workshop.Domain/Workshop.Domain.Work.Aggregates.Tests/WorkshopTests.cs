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

		protected void Assert_UncommittedContainsEvent(WorkshopEvent expected)
			=> _sut.UncommittedEvents.Should().Contain(expected);

		protected void Assert_UncommittedContainsEvents(params WorkshopEvent[] expected)
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
	}

	public class AfterWorkerAdded : WorkshopTestFixture
	{
		private readonly WorkerIdentifier _addedWorker = new WorkerIdentifier();

		public AfterWorkerAdded()
		{
			Act_AddWorker(_addedWorker);
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
		public void AddAnotherWorker_UncommittedContainsBothEvents(WorkerIdentifier anotherWorker)
		{
			Act_AddWorker(anotherWorker);

			Assert_UncommittedContainsEvents(
				new WorkshopEvent.WorkerAdded(_addedWorker),
				new WorkshopEvent.WorkerAdded(anotherWorker)
			);
		}
	}

	public class AfterJobAdded : WorkshopTestFixture
	{
		private readonly Job _addedJob = StaticFixture.Create<Job>();

		public AfterJobAdded()
		{
			Act_AddJob(_addedJob);
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
		public void AddAnotherJob_UncommittedEventsContainsBothJobAddedEvents(Job anotherJob)
		{
			Act_AddJob(anotherJob);

			Assert_UncommittedContainsEvents(
				new WorkshopEvent.JobAdded(_addedJob),
				new WorkshopEvent.JobAdded(anotherJob)
			);
		}
	}
}
