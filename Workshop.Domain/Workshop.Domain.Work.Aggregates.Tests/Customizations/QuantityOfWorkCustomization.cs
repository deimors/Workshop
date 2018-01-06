using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Xunit2;

namespace Workshop.Domain.Work.Aggregates.Tests.Customizations
{
	public class WorkDomainCustomization : ICustomization
	{
		public void Customize(IFixture fixture)
		{
			fixture.Register(() => fixture.Create<float>() * QuantityOfWork.Unit);

			fixture.Register(() => CreateJobStatus(fixture));
		}

		private JobStatus CreateJobStatus(IFixture fixture)
		{
			var quantity1 = fixture.Create<QuantityOfWork>();
			var quantity2 = fixture.Create<QuantityOfWork>();

			return quantity1 < quantity2
				? new JobStatus(quantity2, quantity1)
				: new JobStatus(quantity1, quantity2);
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
