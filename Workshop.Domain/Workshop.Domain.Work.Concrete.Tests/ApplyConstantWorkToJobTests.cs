using FluentAssertions;
using Xunit;

namespace Workshop.Domain.Work.Concrete.Tests
{
	public class ApplyConstantWorkToJobTestFixture
	{
		protected JobStatus Job = new JobStatus(QuantityOfWork.Unit, QuantityOfWork.None);

		protected QuantityOfEffort Effort = new QuantityOfEffort();

		protected QuantityOfWork Increment = QuantityOfWork.Unit;

		private ApplyConstantWorkToJob Sut => new ApplyConstantWorkToJob(Increment);

		protected JobStatus Act_ApplyEffort() => Sut.ApplyEffort(Job, Effort);
	}

	public class ApplyConstantWorkToJobTests : ApplyConstantWorkToJobTestFixture
	{
		[Fact]
		public void WhenUnitIncrementAndJobWithUnitTotalNoneCompleted_ApplyEffort_ReturnsJobWithUnitCompleted()
		{
			var result = Act_ApplyEffort();

			result.Completed.Should().Be(QuantityOfWork.Unit);
		}

		[Fact]
		public void WhenTwoUnitIncrementAndJobWithUnitTotalNoneCompleted_ApplyEffort_ReturnsJobWithUnitCompleted()
		{
			Increment = 2 * QuantityOfWork.Unit;

			var result = Act_ApplyEffort();

			result.Completed.Should().Be(QuantityOfWork.Unit);
		}
	}
}
