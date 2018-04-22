using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CellType
{
    Empty = 0,
    Obscured,
    Road,
    Building,
    Park,
    Defense
}

[System.Serializable]
public class MapCell
{
    public Map map;
    public float x;
    public float y;

    public CellType type {
        get
        {
            if (thing == null)
                return CellType.Empty;

            return thing.type;
        }
    }

    public CityThing thing = null;

    public MapCell(int _x, int _y, Map _map)
    {
        x = _x;
        y = _y;
        map = _map;
    }
}

[System.Serializable]
public class Map {

    public MapCell[] Cells;

    public int Size = 10;

    public Map(int size)
    {
        Size = size;
        Cells = new MapCell[size * size];

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                Cells[y * size + x] = new MapCell(x, y, this);
            }
        }
    }

    public MapCell CellAtPosition(int x, int y)
    {
        if (x < 0 || x > Size || y < 0 || y > Size)
        {
            return null;
        }

        return Cells[y * Size + x];
    }

    public bool CellAtPositionIsType(int x, int y, CellType type)
    {
        MapCell c = CellAtPosition(x, y);

        if (c == null)
        {
            return false;
        }

        return c.type == type;
    }
}
