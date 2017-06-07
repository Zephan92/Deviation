using Assets.Scripts.Interface.Exchange;
using UnityEngine;

namespace Assets.Scripts.Utilities
{
	public static class ActionUtilities
	{
		//this helper method finds the local (0,0) of the specified player's battlefield
		public static Vector3 FindOrigin(IPlayer player)
		{
			Vector3 locPos = player.Transform.localPosition;
			Vector3 pos = player.Transform.position;
			return new Vector3(pos.x - locPos.x, pos.y - locPos.y, pos.z - locPos.z);
		}

		//this helper method initializes a zone array for each battlefield
		public static int[,] InitializeZones(int[,] zones, int zonesLength)
		{
			for (int i = 0; i < zonesLength; i++)
			{
				zones[i, 0] = -100;
				zones[i, 1] = -100;
			}
			return zones;
		}

		//this helper method picks a random empty zone for the specified array 
		public static int[,] PickZone(int[,] zones, int currentZone)
		{
			int column = 0;
			int row = 0;
			bool searchingForFreeZone = true;
			while (searchingForFreeZone)
			{
				//find a random column/row pair
				column = Random.Range(-2, 3);
				row = Random.Range(-2, 3);

				//check to see if the spot is empty
				for (int j = 0; j < currentZone + 1; j++)
				{
					if (zones[j, 0] == column && zones[j, 1] == row)
					{
						//if zone taken start the loop over
						break;
					}
					else if (j == currentZone)
					{
						//else if this is the last zone to look at and it is empty, stop searching
						searchingForFreeZone = false;
					}
					else
					{
						//else continue checking zones
						continue;
					}
				}
			}

			//made it out of the loop use that space
			zones[currentZone, 0] = column;
			zones[currentZone, 1] = row;
			return zones;
		}
	}
}
