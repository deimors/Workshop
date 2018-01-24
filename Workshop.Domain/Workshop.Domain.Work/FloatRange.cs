using System;

namespace Workshop.Domain.Work
{
	public struct FloatRange : IEquatable<FloatRange>
	{
		public static readonly FloatRange ZeroOne = new FloatRange(0, 1);

		public float Min;
		public float Max;

		public FloatRange(float min, float max)
		{
			if (max < min) throw new ArgumentOutOfRangeException(nameof(max), $"< {nameof(min)}");

			Min = min;
			Max = max;
		}

		public override bool Equals(object obj)
			=> obj is FloatRange
			&& Equals((FloatRange)obj);

		public bool Equals(FloatRange other)
			=> Min == other.Min
			&& Max == other.Max;

		public override int GetHashCode()
		{
			var hashCode = 1537547080;
			hashCode = hashCode * -1521134295 + base.GetHashCode();
			hashCode = hashCode * -1521134295 + Min.GetHashCode();
			hashCode = hashCode * -1521134295 + Max.GetHashCode();
			return hashCode;
		}

		public static bool operator ==(FloatRange range1, FloatRange range2) 
			=> range1.Equals(range2);

		public static bool operator !=(FloatRange range1, FloatRange range2) 
			=> !(range1 == range2);
	}
}
