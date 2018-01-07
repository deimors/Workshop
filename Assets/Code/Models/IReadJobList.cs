﻿using System.Collections.Generic;
using UniRx;
using Workshop.Domain.Work;

namespace Workshop.Models
{
	public interface IReadJobList
	{
		IReadJob this[JobIdentifier identifier] { get; }

		IEnumerable<JobIdentifier> Keys { get; }
		IEnumerable<IReadJob> Values { get; }
	}
}