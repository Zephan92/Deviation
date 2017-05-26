using Assets.Scripts.Interface;
using Assets.Scripts.Interface.DTO;
using Assets.Scripts.Interface.Exchange;
using Assets.Scripts.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Library.Action.ModuleActions
{
	public class ScavengeActions : MonoBehaviour
	{
		public static readonly Dictionary<string, System.Action<IBattlefieldController, IAttack, IPlayer>> ActionMethodLibraryTable = new Dictionary<string, System.Action<IBattlefieldController, IAttack, IPlayer>>
		{
			{"StunField", //this method spawns 5 stun traps around the opponent, each trap will stun and damage the opponent if touched 
				delegate (IBattlefieldController bc, IAttack attack, IPlayer player)
				{
					IPlayer enemy = player.Enemies[0];
					attack.Attacker = player;
					attack.Defender = enemy;
					attack.InitiateRecoil();
					Vector3 origin = ActionUtilities.FindOrigin(enemy);
					float originX = origin.x;
					float originZ = origin.z;

					int numStuns = 5;
					int[,] stunLocations = new int[numStuns, 2];
					stunLocations = ActionUtilities.InitializeZones(stunLocations, numStuns);
					GameObject[] stunTriggers = new GameObject[5];
					for (int i = 0; i < numStuns; i++)
					{
						stunLocations = ActionUtilities.PickZone(stunLocations, i);
						GameObject stunTrigger = (GameObject)Instantiate(Resources.Load("StunTrigger"), new Vector3(originX + stunLocations[i,0], 0, originZ + stunLocations[i,1]), player.Transform.rotation);
						stunTriggers[i] = stunTrigger;
					}
					bc.DeleteAfterTimeout(4f, stunTriggers);
				}
			},
		};
	}
}
