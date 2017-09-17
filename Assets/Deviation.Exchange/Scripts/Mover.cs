using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using Assets.Scripts.Enum;
using Assets.Scripts.DTO.Exchange;
using Assets.Scripts.Utilities;
using Assets.Scripts.Interface;

public class Mover : NetworkBehaviour
{
	[SyncVar]
	private bool _rooted;
	[SyncVar]
	public int CurrentRow;
	[SyncVar]
	public int CurrentColumn;
	[SyncVar]
	public float MovementSpeed;

	private MovingDetails _movingDetails;
	private MovingDetails _movingDetailsNext;

	private ICoroutineManager cm;
	private ExchangeBattlefieldController bc;

	private BattlefieldZone _zone;
	private IEnumerator _movingCoroutine;

	public void Awake()
	{
		bc = FindObjectOfType<ExchangeBattlefieldController>();
		cm = FindObjectOfType<CoroutineManager>();	
	}

	public void FixedUpdate()
	{
		if (_movingDetails == null && _movingDetailsNext != null)
		{
			_movingDetails = _movingDetailsNext;
			_movingDetailsNext = null;
			cm.StartCoroutineThread(MovingAction, ref _movingCoroutine);
		}
	}

	public void Init(BattlefieldZone zone, int row, int column, float movementSpeed)
	{
		CurrentRow = row;
		CurrentColumn = column;
		MovementSpeed = movementSpeed;
		_zone = zone;
		_rooted = false;
	}

	public void SetRoot(bool root)
	{
		_rooted = root;
	}

	public void Move(Direction direction, int distance, bool force = false)
	{
		if (_rooted)
		{
			return;
		}

		Vector3 currentPosition = transform.position;
		bool createMoveCoroutine = false;

		Vector3 destination = Vector3.zero;
		float destPoint = 0f;

		if (_movingDetails != null)
		{
			if (_movingDetails.GetDistanceTraveledPercentage() >= 0.5f)
			{
				currentPosition = _movingDetails.Destination;
			}
			else
			{
				return;
			}
		}

		if (_zone == BattlefieldZone.Right)
		{
			switch (direction)
			{
				case Direction.Up:
					direction = Direction.Down;
					break;

				case Direction.Down:
					direction = Direction.Up;
					break;

				case Direction.Left:
					direction = Direction.Right;
					break;

				case Direction.Right:
					direction = Direction.Left;
					break;
			}
		}

		switch (direction)
		{
			case Direction.Up:
				if (force || !bc.GetBattlefieldState(CurrentRow, CurrentColumn + distance, _zone))
				{
					destPoint = currentPosition.x + distance;
					destination = new Vector3(destPoint, 0, currentPosition.z);
					createMoveCoroutine = true;
				}
				break;

			case Direction.Down:
				if (force || !bc.GetBattlefieldState(CurrentRow, CurrentColumn - distance, _zone))
				{
					destPoint = currentPosition.x - distance;
					destination = new Vector3(destPoint, 0, currentPosition.z);
					createMoveCoroutine = true;
				}
				break;

			case Direction.Left:
				if (force || !bc.GetBattlefieldState(CurrentRow + distance, CurrentColumn, _zone))
				{
					destPoint = currentPosition.z + distance;
					destination = new Vector3(currentPosition.x, 0, destPoint);
					createMoveCoroutine = true;
				}
				break;

			case Direction.Right:
				if (force || !bc.GetBattlefieldState(CurrentRow - distance, CurrentColumn, _zone))
				{
					destPoint = currentPosition.z - distance;
					destination = new Vector3(currentPosition.x, 0, destPoint);
					createMoveCoroutine = true;
				}
				break;
		}

		if (createMoveCoroutine && !IsAtBoundary(direction, destPoint))
		{
			if (_movingDetails == null)
			{
				_movingDetails = new MovingDetails(destination, direction, distance);
				cm.StartFixedCoroutineThread(MovingAction, ref _movingCoroutine);
			}
			else
			{
				_movingDetailsNext = new MovingDetails(destination, direction, distance);
			}
			
		}
	}

	public void MovingAction()
	{
		if (_movingDetails == null)
		{
			cm.StopCoroutineThread(ref _movingCoroutine);
			return;
		}

		bool isAtDestination = false;
		Vector3 directionVector;

		switch (_movingDetails.MovingDirection)
		{
			case Direction.Up:
				if (transform.position.x >= _movingDetails.Destination.x)
				{
					isAtDestination = true;
				}

				directionVector = Vector3.right;
				break;

			case Direction.Down:
				if (transform.position.x <= _movingDetails.Destination.x)
				{
					isAtDestination = true;
				}

				directionVector = Vector3.left;
				break;

			case Direction.Left:
				if (transform.position.z >= _movingDetails.Destination.z)
				{
					isAtDestination = true;
				}

				directionVector = Vector3.forward;
				break;

			case Direction.Right:
				if (transform.position.z <= _movingDetails.Destination.z)
				{
					isAtDestination = true;
				}

				directionVector = Vector3.back;
				break;

			default:
				directionVector = Vector3.zero;
				isAtDestination = true;
				break;
		}

		if (isAtDestination)
		{
			bc.SetBattlefieldState(CurrentRow, CurrentColumn, false, _zone);
			var gridLocation = bc.GetGridCoordinates(_movingDetails.Destination, _zone);
			CurrentRow = (int) gridLocation.y;
			CurrentColumn = (int) gridLocation.x;
			bc.SetBattlefieldState(CurrentRow, CurrentColumn, true, _zone);
			transform.position = _movingDetails.Destination;
			cm.StopCoroutineThread(ref _movingCoroutine);
			_movingDetails = null;
		}
		else
		{
			_movingDetails.AddDistanceTraveled(MovementSpeed);
			transform.Translate(directionVector * MovementSpeed);
		}
	}

	public bool IsAtBoundary(Direction dir, float destination)
	{
		Rect boundaries = bc.GetBattlefieldBoundaries(_zone);
		float targetBoundary = 0;

		switch (dir)
		{
			case Direction.Up:
				targetBoundary = boundaries.xMax;
				break;

			case Direction.Down:
				targetBoundary = boundaries.xMin;
				break;

			case Direction.Left:
				targetBoundary = boundaries.yMax;
				break;

			case Direction.Right:
				targetBoundary = boundaries.yMin;
				break;

			default:
				return true;
		}

		switch (dir)
		{
			case Direction.Up:
			case Direction.Left:
				return destination >= targetBoundary;

			case Direction.Down:
			case Direction.Right:
				return destination < targetBoundary;

			default:
				return true;
		}
	}
}
