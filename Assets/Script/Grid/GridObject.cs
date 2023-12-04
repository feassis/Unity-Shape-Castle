
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject
{
    private GridPosition gridPosition;
    private GridSystemHex<GridObject> gridSystem;
    private Tile tile;

    public GridPosition GetGridPosition() { return gridPosition; }

    public GridObject(GridSystemHex<GridObject> gridSystem, GridPosition gridPosition)
    {
        this.gridPosition = gridPosition;
        this.gridSystem = gridSystem;
    }

    public void SetTile(Tile desiredTile)
    {
        if (tile != null)
        {
            GameObject.Destroy(tile.gameObject);
        }

        tile = GameObject.Instantiate(desiredTile);
        tile.transform.position = gridSystem.GetWorldPosition(gridPosition);
    }

    public Tile GetTile() { return tile; }

    public override string ToString()
    {
        return $"{gridPosition.x}, {gridPosition.z} \n ";
    }
}
