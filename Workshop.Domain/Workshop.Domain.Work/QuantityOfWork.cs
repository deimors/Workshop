using System;

namespace Workshop.Domain.Work
{
	public class QuantityOfWork : IEquatable<QuantityOfWork>
	{
		public static QuantityOfWork Unit = new QuantityOfWork(1);

		public static QuantityOfWork None = new QuantityOfWork(0);

		private readonly float _value;

		private QuantityOfWork(float value)
		{
			_value = value;
		}

		public override bool Equals(object obj) 
			=> Equals(obj as QuantityOfWork);

		public bool Equals(QuantityOfWork other) 
			=> other != null 
			&& _value == other._value;

		public override int GetHashCode() 
			=> -1939223833 + _value.GetHashCode();

		public override string ToString()
			=> $"<W: {_value}>";

		public static QuantityOfWork operator *(float multiplier, QuantityOfWork work)
			=> new QuantityOfWork(multiplier * work._value);
		
		public static QuantityOfWork operator +(QuantityOfWork left, QuantityOfWork right)
			=> new QuantityOfWork(left._value + right._value);

		public static QuantityOfWork operator -(QuantityOfWork left, QuantityOfWork right)
			=> new QuantityOfWork(left._value - right._value);

		public static QuantityOfWork operator %(QuantityOfWork left, QuantityOfWork right)
			=> new QuantityOfWork(left._value % right._value);

		public static float operator /(QuantityOfWork completed, QuantityOfWork total)
			=> completed._value / total._value;
		
		public static bool operator <(QuantityOfWork left, QuantityOfWork right)
			=> left._value < right._value;

		public static bool operator >(QuantityOfWork left, QuantityOfWork right)
			=> left._value > right._value;
	}

	public static class QuantityOfWorkExtensions
	{
		public static QuantityOfWork Clamp(this QuantityOfWork work, QuantityOfWork first, QuantityOfWork second)
			=> first < second ? work.ClampOrdered(first, second) : work.ClampOrdered(second, first);

		public static QuantityOfWork ClampOrdered(this QuantityOfWork work, QuantityOfWork lower, QuantityOfWork upper)
			=> work < lower ? lower : work > upper ? upper : work;
	}
}
