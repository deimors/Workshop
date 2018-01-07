using FluentAssertions;
using FluentAssertions.OneOf;
using Functional.Maybe;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Xunit2;
using Workshop.Domain.Work.Aggregates.Tests.Customizations;
using Xunit;

namespace Workshop.Domain.Work.Aggregates.Tests
{
	public class WorkshopAggregateTestFixture
	{
		private readonly WorkshopAggregate _sut;

		protected static readonly IFixture StaticFixture = new Fixture().Customize(new WorkDomainCustomization());

		public WorkshopAggregateTestFixture()
		{
			AssertionOptions.AssertEquivalencyUsing(EquivalencyOptions.OneOf);

			_sut = new WorkshopAggregate();
		}

		protected void Arrange_EventHistory(params WorkshopEvent[] events)
			=> _sut.LoadFromHistory(events);

		protected Maybe<WorkshopError> Act_AddWorker(Worker worker)
			=> _sut.AddWorker(worker);

		protected Maybe<WorkshopError> Act_AddJob(Job job)
			=> _sut.AddJob(job);

		protected Maybe<WorkshopError> Act_AssignJob(WorkerIdentifier workerId, JobIdentifier jobId)
			=> _sut.AssignJob(workerId, jobId);
		
		protected void Assert_UncommittedEventsContains(params WorkshopEvent[] expected)
			=> _sut.UncommittedEvents.Should().ContainInOrder(expected);
	}

	public static class WorkshopAggregateTestAssertionExtensions
	{
		public static void Assert_Succeeds(this Maybe<WorkshopError> result)
			=> result.Should().Be(Maybe<WorkshopError>.Nothing);

		public static void Assert_FailsWith(this Maybe<WorkshopError> result, WorkshopError error)
			=> result.Should().Be(error.ToMaybe());
	}

	public class WhenNoWorkers : WorkshopAggregateTestFixture
	{
		[Theory, AutoData]
		public void AddWorker_Succeeds(Worker worker)
		{
			Act_AddWorker(worker)
				.Assert_Succeeds();
		}

		[Theory, AutoData]
		public void AddWorker_UncommittedContainsWorkerAddedEvent(Worker worker)
		{
			Act_AddWorker(worker);

			Assert_UncommittedEventsContains(new WorkshopEvent.WorkerAdded(worker));
		}

		[Theory, WorkAutoData]
		public void AddJob_Succeeds(Job job)
		{
			Act_AddJob(job)
				.Assert_Succeeds();
		}

		[Theory, WorkAutoData]
		public void AddJob_UncommittedContainsJobAddedEvent(Job job)
		{
			Act_AddJob(job);

			Assert_UncommittedEventsContains(new WorkshopEvent.JobAdded(job));
		}

		[Theory, WorkAutoData]
		public void AssignSomeWorkerToSomeJob_FailsWithUnknownWorker(WorkerIdentifier someWorker, Job someJob)
		{
			Act_AssignJob(someWorker, someJob.Id)
				.Assert_FailsWith(WorkshopError.UnknownWorker);
		}
	}

	public class AfterWorkerAdded : WorkshopAggregateTestFixture
	{
		private readonly Worker _addedWorker = StaticFixture.Create<Worker>();

		public AfterWorkerAdded()
		{
			Arrange_EventHistory(
				new WorkshopEvent.WorkerAdded(_addedWorker)
			);
		}

		[Fact]
		public void AddAddedWorker_FailsWithWorkerAlreadyAdded()
		{
			Act_AddWorker(_addedWorker)
				.Assert_FailsWith(WorkshopError.WorkerAlreadyAdded);
		}

		[Theory, AutoData]
		public void AddAnotherWorker_Succeeds(Worker anotherWorker)
		{
			Act_AddWorker(anotherWorker)
				.Assert_Succeeds();
		}

		[Theory, AutoData]
		public void AddAnotherWorker_UncommittedContainsAddAnotherWorkerEvent(Worker anotherWorker)
		{
			Act_AddWorker(anotherWorker);

			Assert_UncommittedEventsContains(new WorkshopEvent.WorkerAdded(anotherWorker));
		}

		[Theory, WorkAutoData]
		public void AssignAddedWorkerToSomeJob_FailsWithUnknownJob(Job someJob)
		{
			Act_AssignJob(_addedWorker.Id, someJob.Id)
				.Assert_FailsWith(WorkshopError.UnknownJob);
		}
	}

	public class AfterJobAdded : WorkshopAggregateTestFixture
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
			Act_AddJob(_addedJob)
				.Assert_FailsWith(WorkshopError.JobAlreadyAdded);
		}

		[Theory, WorkAutoData]
		public void AddAnotherJob_Succeeds(Job anotherJob)
		{
			Act_AddJob(anotherJob)
				.Assert_Succeeds();
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
			Act_AssignJob(someWorker, _addedJob.Id)
				.Assert_FailsWith(WorkshopError.UnknownWorker);
		}
	}

	public class AfterJobAndWorkerAdded : WorkshopAggregateTestFixture
	{
		private readonly Worker _addedWorker = StaticFixture.Create<Worker>();
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
			Act_AssignJob(_addedWorker.Id, _addedJob.Id)
				.Assert_Succeeds();
		}

		[Fact]
		public void AssignAddedJobToAddedWorker_UncommittedEventsContainsJobAssigned()
		{
			Act_AssignJob(_addedWorker.Id, _addedJob.Id);

			Assert_UncommittedEventsContains(
				new WorkshopEvent.JobAssigned(_addedWorker.Id, _addedJob.Id)
			);
		}
	}

	public class AfterJobAssignedToWorker : WorkshopAggregateTestFixture
	{
		private readonly Worker _addedWorker = StaticFixture.Create<Worker>();
		private readonly Job _addedJob = StaticFixture.Create<Job>();

		public AfterJobAssignedToWorker()
		{
			Arrange_EventHistory(
				new WorkshopEvent.JobAdded(_addedJob),
				new WorkshopEvent.WorkerAdded(_addedWorker),
				new WorkshopEvent.JobAssigned(_addedWorker.Id, _addedJob.Id)
			);
		}

		[Theory, AutoData]
		public void AssignAddedJobToAnotherWorker_Succeeds(Worker anotherWorker)
		{
			Arrange_EventHistory(
				new WorkshopEvent.WorkerAdded(anotherWorker)
			);

			Act_AssignJob(anotherWorker.Id, _addedJob.Id)
				.Assert_Succeeds();
		}

		[Theory, AutoData]
		public void AssignAddedJobToAnotherWorker_UncommittedEventsContainsAddedWorkerUnassignedAnotherWorkerAssigned(Worker anotherWorker)
		{
			Arrange_EventHistory(
				new WorkshopEvent.WorkerAdded(anotherWorker)
			);

			Act_AssignJob(anotherWorker.Id, _addedJob.Id);

			Assert_UncommittedEventsContains(
				new WorkshopEvent.JobUnassigned(_addedWorker.Id, _addedJob.Id),
				new WorkshopEvent.JobAssigned(anotherWorker.Id, _addedJob.Id)
			);
		}

		[Theory, WorkAutoData]
		public void AssignAnotherJobToAddedWorker_Succeeds(Job anotherJob)
		{
			Arrange_EventHistory(
				new WorkshopEvent.JobAdded(anotherJob)
			);

			Act_AssignJob(_addedWorker.Id, anotherJob.Id)
				.Assert_Succeeds();
		}

		[Theory, WorkAutoData]
		public void AssignAnotherJobToAddedWorker_UncommittedEventsContainsAddedWorkerUnassignedAnotherJobAssigned(Job anotherJob)
		{
			Arrange_EventHistory(
				new WorkshopEvent.JobAdded(anotherJob)
			);

			Act_AssignJob(_addedWorker.Id, anotherJob.Id);

			Assert_UncommittedEventsContains(
				new WorkshopEvent.JobUnassigned(_addedWorker.Id, _addedJob.Id),
				new WorkshopEvent.JobAssigned(_addedWorker.Id, anotherJob.Id)				
			);
		}
	}
}
