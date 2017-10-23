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
			Move();
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
				if (force || !bc.GetGridspaceOccupied(CurrentRow, CurrentColumn + distance * zoneModifier, _zone))
				{
					destPoint = currentPosition.x + distance * zoneModifier;
					destination = new Vector3(destPoint, 0, currentPosition.z);
					createMoveCoroutine = true;
				}
				break;

			case Direction.Down:
				if (force || !bc.GetGridspaceOccupied(CurrentRow, CurrentColumn - distance * zoneModifier, _zone))
				{
					destPoint = currentPosition.x - distance * zoneModifier;
					destination = new Vector3(destPoint, 0, currentPosition.z);
					createMoveCoroutine = true;
				}
				break;

			case Direction.Left:
				if (force || !bc.GetGridspaceOccupied(CurrentRow + distance * zoneModifier, CurrentColumn, _zone))
				{
					destPoint = currentPosition.z + distance * zoneModifier;
					destination = new Vector3(currentPosition.x, 0, destPoint);
					createMoveCoroutine = true;
				}
				break;

			case Direction.Right:
				if (force || !bc.GetGridspaceOccupied(CurrentRow - distance * zoneModifier, CurrentColumn, _zone))
				{
					destPoint = currentPosition.z - distance * zoneModifier;
					destination = new Vector3(currentPosition.x, 0, destPoint);
					createMoveCoroutine = true;
				}
				break;
		}

		if (createMoveCoroutine && bc.IsInsideBattlefieldBoundaries(destination, _zone))
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
		int oldRow = CurrentRow;
		int oldColumn = CurrentColumn;
		var gridLocation = bc.GetGridCoordinates(_movingDetails.Destination, _zone);
		CurrentRow = (int)gridLocation.y;
		CurrentColumn = (int)gridLocation.x;//make a gridCoordinate Struct already geeze
		transform.position = _movingDetails.Destination;
		_movingDetails = null;
		CmdUpdateBattlefield(oldRow, oldColumn, CurrentRow, CurrentColumn, _zone);
	}

	[Command]
	private void CmdUpdateBattlefield(int oldRow, int oldColumn,int newRow, int newColumn, BattlefieldZone zone)
	{
		if (!bc.GetGridSpaceDamaged(oldRow, oldColumn, zone))
		{
			bc.SetGridspaceOccupied(oldRow, oldColumn, false, zone);
			bc.ResetGridSpaceColor(oldRow, oldColumn, zone);
		}

		bc.SetGridSpaceColor(newRow, newColumn, Color.gray, zone);
		bc.SetGridspaceOccupied(newRow, newColumn, true, zone);
	}
}
