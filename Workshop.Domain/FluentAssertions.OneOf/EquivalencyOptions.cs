using FluentAssertions.Equivalency;

namespace FluentAssertions.OneOf
{
	public static class EquivalencyOptions
	{
		public static EquivalencyAssertionOptions OneOf(EquivalencyAssertionOptions options)
			=> options
				.RespectingRuntimeTypes()
				.Using(new OneOfEquivalencyStep());
	}
}
