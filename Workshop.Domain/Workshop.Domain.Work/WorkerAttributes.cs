using System;

namespace Workshop.Domain.Work
{
	public class WorkerAttributes : IEquatable<WorkerAttributes>
	{
		public static readonly WorkerAttributes Default = new WorkerAttributes(1, 1);

		public float Speed { get; }
		public float Quality { get; }

		public WorkerAttributes(float speed, float quality)
		{
			if (speed <= 0)
				throw new ArgumentOutOfRangeException($"{nameof(speed)} <= 0");

			if (quality <= 0)
				throw new ArgumentOutOfRangeException($"{nameof(quality)} <= 0");

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
