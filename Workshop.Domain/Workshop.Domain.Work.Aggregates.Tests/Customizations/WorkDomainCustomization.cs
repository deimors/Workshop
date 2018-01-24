using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Xunit2;
using System.Linq;

namespace Workshop.Domain.Work.Aggregates.Tests.Customizations
{
	public class WorkDomainCustomization : ICustomization
	{
		public void Customize(IFixture fixture)
		{
			fixture.Register(() => CreateQuantityOfWork(fixture));

			fixture.Register(() => CreateJobStatus(fixture));

			fixture.Register(() => CreateWorkerStatus(fixture));

			fixture.Register(() => CreateFloatRange(fixture));
		}

		private static QuantityOfWork CreateQuantityOfWork(IFixture fixture) 
			=> fixture.Create<float>() * QuantityOfWork.Unit;

		private JobStatus CreateJobStatus(IFixture fixture)
		{
			var quantity1 = fixture.Create<QuantityOfWork>();
			var quantity2 = fixture.Create<QuantityOfWork>();

			return quantity1 < quantity2
				? new JobStatus(quantity2, quantity1, false)
				: new JobStatus(quantity1, quantity2, false);
		}

		private WorkerStatus CreateWorkerStatus(IFixture fixture)
			=> new WorkerStatus(false);

		private FloatRange CreateFloatRange(IFixture fixture)
		{
			var floats = fixture.CreateMany<float>(2).OrderBy(x => x).ToArray();

			return new FloatRange(floats[0], floats[1]);
		}
	}

	public class WorkAutoDataAttribute : AutoDataAttribute
	{
		public WorkAutoDataAttribute() : base(
			new Fixture()
				.Customize(new WorkDomainCustomization())
		) { }
	}
}
