using Assets.Scripts.Enum;
using UnityEngine;

namespace Assets.Scripts.DTO.Exchange
{
	public class MovingDetails
	{
		public Vector3 Destination;
		public Direction MovingDirection;
		public bool Moving = false;

		public MovingDetails(Vector3 dest, Direction dir)
		{
			Destination = dest;
			MovingDirection = dir;
			Moving = true;
		}
	}
}
