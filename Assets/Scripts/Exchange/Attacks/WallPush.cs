
using System;
using Assets.Scripts.Controllers;
using Assets.Scripts.Enum;
using Assets.Scripts.Interface;
using UnityEngine;
using Assets.Scripts.Interface.DTO;
using Assets.Scripts.DTO.Exchange;

namespace Assets.Scripts.Exchange
{
	public class WallPush : MonoBehaviour, IExchangeAttack
	{
		private IExchangeController ec;
		private IBattlefieldController bc;
		private MovingDetails _movingDetails;
		public int CurrentColumn = 0;
		public int CurrentRow = 0;

		private IAttack Attack;

		public void Awake()
		{
			//start moving wall toward center
			_movingDetails = new MovingDetails(new Vector3(transform.position.x + 2, 0, transform.position.z),Direction.Right);
			bc = FindObjectOfType<BattlefieldController>();
			ec = FindObjectOfType<ExchangeController>();

			CurrentColumn = (int) transform.localPosition.x;
			CurrentRow = (int) transform.localPosition.z;
		}

		public void Update()
		{
			if (_movingDetails != null)
			{
				if (transform.localPosition.x >= _movingDetails.Destination.x)
				{
					CurrentColumn += 2;
					UpdateTransform(CurrentRow, CurrentColumn);
					_movingDetails = null;
				}
				else
				{
					UpdateTransform(transform.localPosition.z, transform.localPosition.x + 0.1f);
				}
			}
		}

		private void UpdateTransform(float row, float column)
		{
			transform.localPosition = new Vector3(column, 0, row);
		}

		public void OnTriggerEnter(Collider other)
		{
			//if this hits a player, push them and deal damage
			if(other.name.Equals("Player"))
			{
				Player player = other.gameObject.GetComponent<Player>();
				Attack.SetDefender(player);
				Attack.InitiateDrain();
				ec.UpdateExchangeControlsDisplay();
				if (player.CurrentColumn == -2)
				{
					player.MoveObject(Direction.Right, 2, true);
				}
				else
				{
					player.MoveObject(Direction.Right, 1, true);
				}
				bc.SetBattlefieldState(player.Battlefield, ConvertToArrayNumber(player.CurrentRow), ConvertToArrayNumber(player.CurrentColumn), true);
			}
		}

		//should put these in a utilities class
		private int ConvertToArrayNumber(int input)
		{
			return input + 2;
		}

		private int ConvertFromArrayNumber(int input)
		{
			return input - 2;
		}

		public void SetAttack(IAttack attack)
		{
			Attack = attack;
		}
	}
}
