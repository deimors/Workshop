using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Workshop.UseCases.Work;
using Zenject;

namespace Workshop.Presentation.Work
{
	public class WorkerPanel : MonoBehaviour
	{
		public class Factory : Factory<WorkerPanel> { }
	}
}
