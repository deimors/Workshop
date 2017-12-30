using FluentAssertions;
using Xunit;

namespace Workshop.Domain.Work.Tests
{
	public class QuantityOfWorkExtensionsTests
	{
		[Theory]
		[InlineData(1, 0, 2, 1)]
		[InlineData(1, 0, 0, 0)]
		[InlineData(0, 1, 2, 1)]
		[InlineData(3, 1, 2, 2)]
		[InlineData(0, 2, 1, 1)]
		[InlineData(3, 2, 1, 2)]
		public void Clamp(float work, float first, float second, float expected)
		{
			var sut = work * QuantityOfWork.Unit;

			var result = sut.Clamp(first * QuantityOfWork.Unit, second * QuantityOfWork.Unit);

			result.Should().Be(expected * QuantityOfWork.Unit);
		}
	}
}
