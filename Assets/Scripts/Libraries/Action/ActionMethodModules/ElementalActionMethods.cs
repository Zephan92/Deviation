using Assets.Scripts.Exchange;
using Assets.Scripts.Interface;
using Assets.Scripts.Interface.DTO;
using Assets.Scripts.Interface.Exchange;
using Assets.Scripts.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Library.Action.ModuleActions
{
	public class ElementalActionMethods : MonoBehaviour, IActionMethodLibraryModule
	{
		public Dictionary<string, System.Action<IBattlefieldController, IAttack, IPlayer>> ActionMethods { get; set; }
		public ElementalActionMethods()
		{
			ActionMethods = new Dictionary<string, System.Action<IBattlefieldController, IAttack, IPlayer>>
			{
				{"Avalanche", //this method drops 3 rounds of 5 rocks on the opponent, each rock does damage
					delegate (IBattlefieldController bc, IAttack attack, IPlayer player)
					{
						IPlayer enemy = player.Enemies[0];
						attack.Attacker = player;
						attack.Defender = enemy;
						attack.InitiateRecoil();
						Vector3 origin = ActionUtilities.FindOrigin(enemy);
						float originX = origin.x;
						float originZ = origin.z;
						int numRocks = 5;
						int numRounds = 3;
						int rockCounter = 0;

						System.Action<Collider, GameObject, IAttack> onTriggerEnterMethod = delegate(Collider other, GameObject projectile, IAttack atk)
						{
							//if this objects hit a player attack it
							if (other.name.Equals("Player"))
							{
								atk.SetDefender(other.GetComponent<Player>());
								atk.InitiateDrain();
								Destroy(projectile);
							}
						};

						for (int j = 0; j < numRounds; j++)
						{
							int[,] zones = new int[numRocks, 2];
							zones = ActionUtilities.InitializeZones(zones, numRocks);
							for (int i = 0; i < numRocks; i++)
							{
								zones = ActionUtilities.PickZone(zones, i);

								bc.SpawnProjectileAfterTimeout(rockCounter / 4f,2f,"Rock", new Vector3(originX + zones[i, 0], 3f, originZ + zones[i, 1]), enemy.Transform.rotation, attack, onTriggerEnterMethod);
								rockCounter++;
							}
						}
					}
				},
				{"FireBlast", //Shoots a burst of fire in a column
					delegate (IBattlefieldController bc, IAttack attack, IPlayer player)
					{
						IPlayer enemy = player.Enemies[0];
						attack.Attacker = player;
						attack.InitiateRecoil();

						ParticleSystem system = player.Transform.gameObject.GetComponent<ParticleSystem>();
						//system.GetCollisionEvents;
						ParticleSystem.MainModule module = system.main;
						module.startColor = Color.red;
						system.Emit(10);

						RaycastHit hit;

						if (Physics.Raycast(player.Transform.position, Vector3.forward, out hit))
						{
							attack.Defender = hit.transform.gameObject.GetComponent<Player>();
							//attack.InitiateDrain();


						}

					}
				},
			};
		}
	}
}
