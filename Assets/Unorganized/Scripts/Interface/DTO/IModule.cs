using Assets.Scripts.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Interface.DTO
{
	public interface IModule
	{
		string Name { get; set; }
		ModuleType Type { get; set; }
		Texture2D ModuleTexture { get; set; }
		IExchangeAction[] Actions { get;  set;}
		string[] ActionNames { get; set; }
		IKit ParentKit { get; set; }
	}
}
