using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using Assets.Scripts.Enum;

public interface IGridManager
{
	void InitGridspace(GameObject go, GameObject parentGo, int row, int column);
	void ResetGrid();
	bool GetGridSpaceDamaged(GridCoordinate coordinate, BattlefieldZone zone = BattlefieldZone.All);
	bool GetGridSpaceBroken(GridCoordinate coordinate, BattlefieldZone zone = BattlefieldZone.All);
	void SetGridSpaceColor(GridCoordinate coordinate, Color color, BattlefieldZone zone = BattlefieldZone.All);
	void ResetGridSpaceColor(GridCoordinate coordinate, BattlefieldZone zone = BattlefieldZone.All);
	void BreakTile(GridCoordinate coordinate, BattlefieldZone zone = BattlefieldZone.All, bool force = false);
	void DamageTile(GridCoordinate coordinate, BattlefieldZone zone = BattlefieldZone.All, bool breakable = false);
	void SetGridspaceOccupied(GridCoordinate coordinate, bool state, BattlefieldZone zone = BattlefieldZone.All);
	bool GetGridspaceOccupied(GridCoordinate coordinate, BattlefieldZone zone = BattlefieldZone.All);
}

public class GridManager : NetworkBehaviour, IGridManager
{
	public GridSpace[,] Grid;

	public void Start()
	{
		Grid = new GridSpace[ExchangeConstants.BATTLEFIELD_COLUMN_COUNT, ExchangeConstants.BATTLEFIELD_ROW_COUNT];
	}

	public void InitGridspace(GameObject go, GameObject parentGo, int row, int column)
	{
		RpcGridSpaceInit(go, parentGo, row, column);
		GridSpaceInit(go, parentGo.transform, new GridCoordinate(row, column));
	}

	public void ResetGrid()
	{
		foreach (GridSpace gridspace in Grid)
		{
			gridspace.ReInit();
		}
	}

	public bool GetGridspaceOccupied(GridCoordinate coordinate, BattlefieldZone zone = BattlefieldZone.All)
	{
		GridSpace gridspace = GetGridSpace(coordinate, zone);
		if (gridspace != null)
		{
			return gridspace.Occupied;
		}
		else
		{
			return false;
		}
	}

	public void SetGridspaceOccupied(GridCoordinate coordinate, bool state, BattlefieldZone zone = BattlefieldZone.All)
	{
		//Debug.LogErrorFormat("Setting {0}: {1}", coordinate, state);
		GridSpace gridspace = GetGridSpace(coordinate, zone);
		if (gridspace != null)
		{
			gridspace.Occupied = state;
		}
		else
		{
			Debug.LogErrorFormat("Set Gridspace Occupied tile didn't target a correct gridspace. GridCoordinate: {0}", coordinate.ToString());
		}
	}

	public bool GetGridSpaceDamaged(GridCoordinate coordinate, BattlefieldZone zone = BattlefieldZone.All)
	{
		GridSpace gridspace = GetGridSpace(coordinate, zone);
		if (gridspace != null)
		{
			return gridspace.Damaged;
		}
		else
		{
			return false;
		}
	}

	public bool GetGridSpaceBroken(GridCoordinate coordinate, BattlefieldZone zone = BattlefieldZone.All)
	{
		GridSpace gridspace = GetGridSpace(coordinate, zone);
		if (gridspace != null)
		{
			return gridspace.Broken;
		}
		else
		{
			return false;
		}
	}

	public void ResetGridSpaceColor(GridCoordinate coordinate, BattlefieldZone zone = BattlefieldZone.All)
	{
		GridSpace gridspace = GetGridSpace(coordinate, zone);
		if (gridspace != null)
		{
			gridspace.ResetTexture();
		}
		else
		{
			Debug.LogErrorFormat("Reset Gridspace Color tile didn't target a correct gridspace. GridCoordinate: {0}", coordinate.ToString());
		}
	}

	public void SetGridSpaceColor(GridCoordinate coordinate, Color color, BattlefieldZone zone = BattlefieldZone.All)
	{
		GridSpace gridspace = GetGridSpace(coordinate, zone);
		if (gridspace != null)
		{
			gridspace.UpdateTexture(color);
		}
		else
		{
			Debug.LogErrorFormat("Set Gridspace Color tile didn't target a correct gridspace. GridCoordinate: {0}", coordinate.ToString());
		}
	}

	public void BreakTile(GridCoordinate coordinate, BattlefieldZone zone = BattlefieldZone.All, bool force = false)
	{
		GridSpace gridspace = GetGridSpace(coordinate, zone);
		if (gridspace != null)
		{
			gridspace.BreakTile(force);
		}
		else
		{
			Debug.LogErrorFormat("Break tile didn't target a correct gridspace. GridCoordinate: {0}", coordinate.ToString());
		}
	}

	public void DamageTile(GridCoordinate coordinate, BattlefieldZone zone = BattlefieldZone.All, bool breakable = false)
	{
		GridSpace gridspace = GetGridSpace(coordinate, zone);
		if (gridspace != null)
		{
			gridspace.DamageTile(breakable);
		}
		else
		{
			Debug.LogErrorFormat("Damage tile didn't target a correct gridspace. GridCoordinate: {0}", coordinate.ToString());
		}
	}

	private GridSpace GetGridSpace(GridCoordinate coordinate, BattlefieldZone zone = BattlefieldZone.All)
	{
		//Debug.LogErrorFormat("GetGridSpace {0}.", coordinate);
		if (coordinate.Valid(zone))
		{
			//Debug.LogErrorFormat("GetGridSpace {0}. Returning {1}", coordinate, Grid[coordinate.Column, coordinate.Row]);
			return Grid[coordinate.Column, coordinate.Row];
		}
		else
		{
			//Debug.LogErrorFormat("GetGridSpace {0}. Returning null", coordinate);
			return null;
		}
	}

	[ClientRpc]
	private void RpcGridSpaceInit(GameObject go, GameObject parentGO, int row, int column)
	{
		GridSpaceInit(go, parentGO.transform, new GridCoordinate(row, column));
	}

	private void GridSpaceInit(GameObject go, Transform parent, GridCoordinate coordinate)
	{
		go.transform.parent = parent;
		var gridspace = go.GetComponent<GridSpace>();
		Grid[coordinate.Column, coordinate.Row] = gridspace;
	}
}
