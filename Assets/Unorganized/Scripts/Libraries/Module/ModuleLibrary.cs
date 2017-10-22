using Assets.Scripts.Enum;
using Assets.Scripts.Interface.DTO;
using System.Collections.Generic;
using Assets.Scripts.DTO.Exchange;
using UnityEngine;

namespace Assets.Scripts.Library
{
	//This library holds all of the values for each Module, all definitions of a Module are here.
	public class ModuleLibrary
	{
		public static IModule GetModuleInstance(string moduleName)
		{
			IModule module = ModuleLibraryTable[moduleName];
			IModule moduleInstance = new Module(module.Name,module.ActionNames,module.Type,module.ModuleTexture);
			return moduleInstance;
		}

		public static readonly Dictionary<string, IModule> ModuleLibraryTable = new Dictionary<string, IModule>
		{
			{ "default", new Module(
				name: "Default",
				actionNames: new string []
				{
					"default",
				},
				type: ModuleType.Default,
				moduleTexture: Resources.Load("ActionTextures/White") as Texture2D

			)},
			{"Thief", new Module(
				name: "Thief",
				actionNames: new string []
				{
					"LargeProjectile",
					"MediumProjectile",
					"Tremor",
					"SmallProjectile",
				},
				type: ModuleType.Scavenge,
				moduleTexture: Resources.Load("ActionTextures/Black") as Texture2D
			)},
			{"Mage",new Module(
				name: "Mage",
				actionNames: new string []
				{
					"FireBlast",
					"MiddleAttack",
					"SmallProjectile",
					//"Teleport",
					"SmallProjectile",
				},
				type: ModuleType.Scavenge,
				moduleTexture: Resources.Load("ActionTextures/Purple") as Texture2D
			)},
		};
	}

}
