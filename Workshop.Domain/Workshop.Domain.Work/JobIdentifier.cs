﻿namespace Workshop.Domain.Work
{
	public class JobIdentifier
	{
		public override string ToString()
			=> $"Job {GetHashCode().ToString("X")}";
	}
}
