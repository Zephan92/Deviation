using Assets.Scripts.DTO.Exchange;
using Assets.Scripts.Enum;
using Assets.Scripts.Exchange;
using Assets.Scripts.Interface;
using Assets.Scripts.Interface.DTO;
using Assets.Scripts.Interface.Exchange;
using Assets.Scripts.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Library.Action.ModuleActions
{
	public class ManipulateActions : MonoBehaviour
	{
		public static readonly Dictionary<string, System.Action<IBattlefieldController, IAttack, IPlayer>> ActionMethodLibraryTable = new Dictionary<string, System.Action<IBattlefieldController, IAttack, IPlayer>>
		{
			{"ShortAttack", //this is a generic attack that does no battlefield affects
				delegate (IBattlefieldController bc, IAttack attack, IPlayer player)
				{
					IPlayer enemy = player.Enemies[0];
					attack.Attacker = player;
					attack.Defender = enemy;
					attack.InitiateAttack();
				}
			},
			{"MiddleAttack", //this is a generic attack that does no battlefield affects
				delegate (IBattlefieldController bc, IAttack attack, IPlayer player)
				{
					IPlayer enemy = player.Enemies[0];
					attack.Attacker = player;
					attack.Defender = enemy;
					attack.InitiateAttack();
				}
			},
			{"Steal",  //this is a generic attack that does no battlefield affects
				delegate (IBattlefieldController bc, IAttack attack, IPlayer player)
				{
					IPlayer enemy = player.Enemies[0];
					attack.Attacker = player;
					attack.Defender = enemy;
					attack.InitiateAttack();
				}
			},
			{"Disappear",  //this is a generic attack that does no battlefield affects
				delegate (IBattlefieldController bc, IAttack attack, IPlayer player)
				{
					IPlayer enemy = player.Enemies[0];
					attack.Attacker = player;
					attack.Defender = enemy;
					attack.InitiateRecoil();
				}
			},
			{"WallPush", // this method spawns a wall and pushes the opponent to the middle if it hits them dealing some damage
				delegate (IBattlefieldController bc, IAttack attack, IPlayer player)
				{
					IPlayer enemy = player.Enemies[0];
					attack.Attacker = player;
					attack.Defender = enemy;
					attack.InitiateRecoil();
					Vector3 origin = ActionUtilities.FindOrigin(enemy);
					float originX = origin.x;
					float originZ = origin.z;
					Battlefield field = enemy.Battlefield;
					float timeout = 0.5f;

					System.Action<Collider, GameObject, IAttack> onTriggerEnterMethod = delegate(Collider other, GameObject projectile, IAttack thing)
					{
						if(other.name.Equals("Player"))
						{
							Player otherPlayer = other.gameObject.GetComponent<Player>();
							attack.SetDefender(player);
							attack.InitiateDrain();
							if (player.CurrentColumn == -2)
							{
								player.MoveObject(Direction.Right, 2, true);
							}
							else
							{
								player.MoveObject(Direction.Right, 1, true);
							}
							bc.SetBattlefieldState(player.Battlefield, ExchangeUtilities.ConvertToArrayNumber(player.CurrentRow), ExchangeUtilities.ConvertToArrayNumber(player.CurrentColumn), true);
						}
					};

					System.Action<GameObject> onStartAction = delegate(GameObject projectile)
					{
					};

					bc.SpawnProjectile(timeout, "Wall", new Vector3(0,0,originZ), enemy.Transform.rotation, attack, onTriggerEnterMethod, onStartAction);

					for(int i = -2; i <= 2; i++)
					{
						bc.SetBattlefieldState(field, ExchangeUtilities.ConvertToArrayNumber(i), ExchangeUtilities.ConvertToArrayNumber(-1), true);
						bc.SetBattlefieldState(field, ExchangeUtilities.ConvertToArrayNumber(i), ExchangeUtilities.ConvertToArrayNumber(-2), true);
						bc.SetBattlefieldStateAfterTimout(timeout, field, ExchangeUtilities.ConvertToArrayNumber(i), ExchangeUtilities.ConvertToArrayNumber(-1), false);
						bc.SetBattlefieldStateAfterTimout(timeout, field, ExchangeUtilities.ConvertToArrayNumber(i), ExchangeUtilities.ConvertToArrayNumber(-2), false);
					}

				}
			},
		};
	}
}
