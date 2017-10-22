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

	public int CurrentBattlefieldRow { get { return CurrentRow; } }
	public int CurrentBattlefieldColumn { get { return _zone == BattlefieldZone.Left ? CurrentColumn : CurrentColumn + 5; } }

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

		int zoneModifier = _zone == BattlefieldZone.Left ? 1 : -1;

		switch (direction)
		{
			case Direction.Up:
				if (force || !bc.GetBattlefieldState(CurrentRow, CurrentColumn + distance * zoneModifier, _zone))
				{
					destPoint = currentPosition.x + distance * zoneModifier;
					destination = new Vector3(destPoint, 0, currentPosition.z);
					createMoveCoroutine = true;
				}
				break;

			case Direction.Down:
				if (force || !bc.GetBattlefieldState(CurrentRow, CurrentColumn - distance * zoneModifier, _zone))
				{
					destPoint = currentPosition.x - distance * zoneModifier;
					destination = new Vector3(destPoint, 0, currentPosition.z);
					createMoveCoroutine = true;
				}
				break;

			case Direction.Left:
				if (force || !bc.GetBattlefieldState(CurrentRow + distance * zoneModifier, CurrentColumn, _zone))
				{
					destPoint = currentPosition.z + distance * zoneModifier;
					destination = new Vector3(currentPosition.x, 0, destPoint);
					createMoveCoroutine = true;
				}
				break;

			case Direction.Right:
				if (force || !bc.GetBattlefieldState(CurrentRow - distance * zoneModifier, CurrentColumn, _zone))
				{
					destPoint = currentPosition.z - distance * zoneModifier;
					destination = new Vector3(currentPosition.x, 0, destPoint);
					createMoveCoroutine = true;
				}
				break;
		}

		if (createMoveCoroutine && bc.GetBattlefieldBoundaries(_zone).Contains(new Vector2(destination.x, destination.z)))
		{
			if (_movingDetails == null)
			{
				_movingDetails = new MovingDetails(destination, direction, distance);
				Move();
			}
			else
			{
				_movingDetailsNext = new MovingDetails(destination, direction, distance);
			}
			
		}
	}

	public void Move()
	{
		Vector3 directionVector;

		switch (_movingDetails.MovingDirection)
		{
			case Direction.Up:
				directionVector = Vector3.forward;
				break;

			case Direction.Down:
				directionVector = Vector3.back;
				break;

			case Direction.Left:
				directionVector = Vector3.left;
				break;

			case Direction.Right:
				directionVector = Vector3.right;
				break;

			default:
				directionVector = Vector3.zero;
				break;
		}

		_movingDetails.AddDistanceTraveled(MovementSpeed);
		transform.Translate(directionVector * MovementSpeed);
		if (!bc.GetGridSpaceBroken(CurrentRow, CurrentColumn, _zone))
		{
			bc.SetBattlefieldState(CurrentRow, CurrentColumn, false, _zone);
			bc.ResetGridSpaceColor(CurrentRow, CurrentColumn, _zone);
		}
		var gridLocation = bc.GetGridCoordinates(_movingDetails.Destination, _zone);
		CurrentRow = (int)gridLocation.y;
		CurrentColumn = (int)gridLocation.x;
		bc.SetGridSpaceColor(CurrentRow, CurrentColumn, Color.gray, _zone);

		bc.SetBattlefieldState(CurrentRow, CurrentColumn, true, _zone);
		transform.position = _movingDetails.Destination;
		_movingDetails = null;
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

				directionVector = Vector3.forward;
				break;

			case Direction.Down:
				if (transform.position.x <= _movingDetails.Destination.x)
				{
					isAtDestination = true;
				}

				directionVector = Vector3.back;
				break;

			case Direction.Left:
				if (transform.position.z >= _movingDetails.Destination.z)
				{
					isAtDestination = true;
				}

				directionVector = Vector3.left;
				break;

			case Direction.Right:
				if (transform.position.z <= _movingDetails.Destination.z)
				{
					isAtDestination = true;
				}

				directionVector = Vector3.right;
				break;

			default:
				directionVector = Vector3.zero;
				isAtDestination = true;
				break;
		}

		if (isAtDestination)
		{
			if (!bc.GetGridSpaceBroken(CurrentRow, CurrentColumn, _zone))
			{
				bc.SetBattlefieldState(CurrentRow, CurrentColumn, false, _zone);
				//bc.ResetGridSpaceColor(CurrentRow, CurrentColumn, _zone);
			}
			var gridLocation = bc.GetGridCoordinates(_movingDetails.Destination, _zone);
			CurrentRow = (int) gridLocation.y;
			CurrentColumn = (int) gridLocation.x;
			//bc.SetGridSpaceColor(CurrentRow, CurrentColumn, Color.gray, _zone);

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
}
