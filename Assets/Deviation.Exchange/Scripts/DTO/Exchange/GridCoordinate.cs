using UnityEngine;
using Assets.Scripts.Enum;

public struct GridCoordinate
{
	public int Row;
	public int Column;
	public BattlefieldZone Zone;

	public int LocalColumn
	{ 
		get
		{
			if (Zone == BattlefieldZone.Right)
			{
				return Column - 5;
			}
			else
			{
				return Column;
			}
		}
	}

	public GridCoordinate(BattlefieldZone zone = BattlefieldZone.All, bool localCoordinates = false)
	{
		Row = 0;
		Column = 0;
		Zone = zone;

		if (localCoordinates && zone == BattlefieldZone.Right)
		{
			Column += 5;
		}
	}

	public GridCoordinate(int row, int column, BattlefieldZone zone = BattlefieldZone.All, bool localCoordinates = false)
	{
		Row = row;
		Column = column;
		Zone = zone;

		if(localCoordinates && zone == BattlefieldZone.Right)
		{
			Column += 5;
		}
	}

	public GridCoordinate(float row, float column, BattlefieldZone zone = BattlefieldZone.All, bool localCoordinates = false)
	{
		Row = Mathf.RoundToInt(row);
		Column = Mathf.RoundToInt(column);
		Zone = zone;

		if(localCoordinates && zone == BattlefieldZone.Right)
		{
			Column += 5;
		}
	}

	public GridCoordinate(Vector3 gridCoordinate, BattlefieldZone zone = BattlefieldZone.All)
	{
		Row = Mathf.RoundToInt(gridCoordinate.z);
		Column = Mathf.RoundToInt(gridCoordinate.x);
		Zone = zone;
	}

	public bool Valid(BattlefieldZone zone = BattlefieldZone.All)
	{
		int maxColumn = zone == BattlefieldZone.Left ? ExchangeConstants.BATTLEFIELD_LOCAL_COLUMN_COUNT : ExchangeConstants.BATTLEFIELD_COLUMN_COUNT;
		int minColumn = zone == BattlefieldZone.Right ? ExchangeConstants.BATTLEFIELD_LOCAL_COLUMN_COUNT : 0;
		//Debug.LogErrorFormat("Valid {0}. maxColumn: {1}. minColumn: {2}.", ToString(), maxColumn, minColumn);

		if (ExchangeConstants.BATTLEFIELD_ROW_COUNT > Row &&
			Row >= 0 &&
			maxColumn > Column &&
			Column >= minColumn)
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	public Vector2 Position_Vector2()
	{
		return new Vector2(Column, Row);
	}

	public Vector3 Position_Vector3()
	{
		return new Vector3(Column, 0, Row);
	}

	public override string ToString()
	{
		return string.Format("({0}, {1}): {2}", Row, Column, Zone);
	}
}
