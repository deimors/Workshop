using System;

namespace Workshop.Domain.Work
{
	public static class FloatRangeExtensions
	{
		public static bool IsInRange(this FloatRange range, float value)
			=> value > range.Min 
			&& value < range.Max;

		public static float Difference(this FloatRange range)
			=> range.Max - range.Min;

		public static float Project(this FloatRange range, float value)
			=> value * range.Difference() + range.Min;
	}
}
