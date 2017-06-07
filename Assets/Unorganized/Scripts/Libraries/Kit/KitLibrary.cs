using Assets.Scripts.DTO.Exchange;
using Assets.Scripts.Interface.DTO;
using System.Collections.Generic;

namespace Assets.Scripts.Library
{
	public class KitLibrary
	{
		public static IKit GetKitInstance(string kitName)
		{
			IKit kit = KitLibraryTable[kitName];
			IKit kitInstance = new Kit(kit.Name, kit.ModuleNames, kit.MaxModules);
			return kitInstance;
		}

		//This library holds all of the values for each Kit, all definitions of a Kit are here.
		public static readonly Dictionary<string, IKit> KitLibraryTable = new Dictionary<string, IKit>
		{
			{"default", new Kit //this is the default kit
				(
					name: "Default",
					moduleNames: new string []
					{
						"default",
						"default",
					},
					maxModules: 2
				)
			},
			{"InitialKit", new Kit //this is the demo kit
				(
					name: "Eroe's Kit",
					moduleNames: new string []
					{
						"Thief",
						"Mage",
					},
					maxModules: 2
				)
			},
		};
	}
}
