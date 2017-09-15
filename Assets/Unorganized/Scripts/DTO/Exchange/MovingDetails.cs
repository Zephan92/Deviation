using Assets.Scripts.Enum;
using UnityEngine;

namespace Assets.Scripts.DTO.Exchange
{
	public class MovingDetails
	{
		public Vector3 Destination;
		public Direction MovingDirection;
		private float _distanceTraveled = 0;
		private float _distanceToDestination = 0;

		public MovingDetails(Vector3 dest, Direction dir)
		{
			Destination = dest;
			MovingDirection = dir;
		}

		public MovingDetails(Vector3 dest, Direction dir, float distanceToDestination)
		{
			Destination = dest;
			MovingDirection = dir;
			_distanceToDestination = distanceToDestination;
		}

		public void AddDistanceTraveled(float distance)
		{
			_distanceTraveled += Mathf.Abs(distance);
		}

		public float GetDistanceTraveledPercentage()
		{
			return _distanceTraveled / _distanceToDestination;
		}
	}
}
