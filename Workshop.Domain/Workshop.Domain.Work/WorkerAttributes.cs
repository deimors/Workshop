using System;

namespace Workshop.Domain.Work
{
	public class WorkerAttributes : IEquatable<WorkerAttributes>
	{
		public static readonly WorkerAttributes Default = new WorkerAttributes(FloatRange.ZeroOne, FloatRange.ZeroOne);

		public FloatRange Speed { get; }
		public FloatRange Quality { get; }

		public WorkerAttributes(FloatRange speed, FloatRange quality)
		{
			Speed = speed;
			Quality = quality;
		}

		public override bool Equals(object obj) 
			=> Equals(obj as WorkerAttributes);

		public bool Equals(WorkerAttributes other) 
			=> !(other is null)
				&& Speed == other.Speed 
				&& Quality == other.Quality;

		public override int GetHashCode()
		{
			var hashCode = 1390535654;
			hashCode = hashCode * -1521134295 + Speed.GetHashCode();
			hashCode = hashCode * -1521134295 + Quality.GetHashCode();
			return hashCode;
		}
	}
}
