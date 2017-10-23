using Assets.Scripts.Enum;
using Assets.Scripts.Exchange;
using Assets.Scripts.Exchange.Attacks;
using Assets.Scripts.Interface;
using Assets.Scripts.Interface.DTO;
using Assets.Scripts.Interface.Exchange;
using Assets.Scripts.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Library.Action.ModuleActions
{
	public class ElementalActionMethods : IActionMethodLibraryModule
	{
		public Dictionary<string, System.Action<IBattlefieldController, IAttack, IExchangePlayer, BattlefieldZone>> ActionMethods { get; set; }
		public ElementalActionMethods()
		{
			ActionMethods = new Dictionary<string, System.Action<IBattlefieldController, IAttack, IExchangePlayer, BattlefieldZone>>
			{
				{"Avalanche", //this method drops 3 rounds of 5 rocks on the opponent, each rock does damage
					delegate (IBattlefieldController bc, IAttack attack, IExchangePlayer player, BattlefieldZone zone)
					{
						//IPlayer enemy = player.Enemies[0];
						//attack.Attacker = player;
						//attack.Defender = enemy;
						//attack.InitiateRecoil();
						//Vector3 origin = ActionUtilities.FindOrigin(enemy);
						//float originX = origin.x;
						//float originZ = origin.z;
						//int numRocks = 5;
						//int numRounds = 3;
						//int rockCounter = 0;

						//System.Action<Collider, GameObject, IAttack> onTriggerEnterMethod = delegate(Collider other, GameObject projectile, IAttack atk)
						//{
						//	//if this objects hit a player attack it
						//	if (other.name.Equals("Player"))
						//	{
						//		atk.SetDefender(other.GetComponent<Player>());
						//		atk.InitiateDrain();
						//		GameObject.Destroy(projectile);
						//	}
						//};

						//for (int j = 0; j < numRounds; j++)
						//{
						//	int[,] zones = new int[numRocks, 2];
						//	zones = ActionUtilities.InitializeZones(zones, numRocks);
						//	for (int i = 0; i < numRocks; i++)
						//	{
						//		zones = ActionUtilities.PickZone(zones, i);

						//		bc.SpawnProjectileAfterTimeout(rockCounter / 4f,2f,"Rock", new Vector3(originX + zones[i, 0], 3f, originZ + zones[i, 1]), enemy.Transform.rotation, attack, onTriggerEnterMethod);
						//		rockCounter++;
						//	}
						//}
					}
				},
				{"FireBlast", //Shoots a burst of fire in a column
					delegate (IBattlefieldController bc, IAttack attack, IExchangePlayer player, BattlefieldZone zone)
					{
						////IPlayer enemy = player.Enemies[0];
						//attack.Attacker = player;
						//attack.InitiateRecoil();

						//ParticleSystem system = player.Transform.gameObject.GetComponent<ParticleSystem>();
						////system.GetCollisionEvents;
						//ParticleSystem.MainModule module = system.main;
						//module.startColor = Color.red;
						//system.Emit(10);

						//RaycastHit hit;

						//if (Physics.Raycast(player.Transform.position, Vector3.forward, out hit))
						//{
						//	attack.Defender = hit.transform.gameObject.GetComponent<Player>();
						//	//attack.InitiateDrain();


						//}

					}
				},
				{"Tremor", //this method breaks 5 tiles around the opponent
					delegate (IBattlefieldController bc, IAttack attack, IExchangePlayer player, BattlefieldZone zone)
					{
						var enemyZone = ActionUtilities.GetEnemyBattlefieldZone(zone);
						attack.InitiateAttack(new List<IExchangePlayer>{ player}, AttackAlignment.Allies );

						Vector3 origin = bc.GetBattlefieldCoordinates(enemyZone);
						float originX = origin.x;
						float originZ = origin.z;

						int numTiles = 5;
						int[,] stunLocations = new int[numTiles, 2];
						stunLocations = ActionUtilities.InitializeZones(stunLocations, numTiles);
						for (int i = 0; i < numTiles; i++)
						{
							stunLocations = ActionUtilities.PickZone(stunLocations, i);
							int x = (int) originX + stunLocations[i,0];
							int z = (int) originZ + stunLocations[i,1];

							System.Action onDelayStart = delegate()
							{
								bc.SetGridSpaceColor(z,x,Color.yellow);
							};

							System.Action onDelayEnd = delegate()
							{
								bc.ResetGridSpaceColor(z,x);
								bc.DamageTile(z,x);
							};

							bc.ActionWarning(0.5f, onDelayStart, onDelayEnd);
						}
					}
				},
				{"ShockWave", //this method breaks 3 tiles around the opponent
					delegate (IBattlefieldController bc, IAttack attack, IExchangePlayer player, BattlefieldZone zone)
					{
						attack.InitiateAttack(new List<IExchangePlayer>{ player}, AttackAlignment.Allies );

						System.Action<GameObject> onStartMethod = delegate(GameObject actionGO)
						{
							actionGO.GetComponent<ActionObject>().DisableRenderer();
							var mover = actionGO.GetComponent<ActionObjectMover>();
							mover.Init(player.Mover.CurrentRow, player.Mover.CurrentColumn, 15, true);
						};

						System.Action<GameObject> onTileEnterMethod = delegate(GameObject actionGO)
						{
							var mover = actionGO.GetComponent<ActionObjectMover>();

							if(bc.GetGridSpaceBroken(mover.CurrentRow, mover.CurrentColumn))
							{
								mover.StopObject();
							}

							if (!bc.IsInsideBattlefieldBoundaries(mover.CurrentRow, mover.CurrentColumn, zone))
							{
								bc.DamageTile(mover.CurrentRow, mover.CurrentColumn, breakable: true);
							}
						};

						System.Action<Collider, GameObject, IAttack> onTriggerEnterMethod = delegate(Collider other, GameObject actionGO, IAttack actionAttack)
						{
							if(other.tag == "Player")
							{
								IExchangePlayer otherPlayer = other.GetComponent<IExchangePlayer>();
								if(!otherPlayer.Equals(player))
								{
									actionAttack.InitiateAttack(new List<IExchangePlayer>{ otherPlayer}, AttackAlignment.Enemies );
								}
							}
						};

						bc.SpawnActionObject(0.0f, 10f, "Rocket", player.Position, attack,
							rotation: Quaternion.Euler(player.Rotation.eulerAngles),
							onStartAction: onStartMethod,
							onTriggerAction: onTriggerEnterMethod,
							onTileEnter: onTileEnterMethod);
					}
				},
			};
		}
	}
}
