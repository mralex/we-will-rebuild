using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public enum BuildingTypes
{
    Small,
    Medium,
    MediumWide,
    Large
}

public enum ThingState
{
    None = 0,
    Construction,
    Normal,
    Damaged,
    Bulldoze
}

//[System.Serializable]
public class CityThing
{
    public MapCell cell;
    public CellType type = CellType.Empty;

    public ThingState state = ThingState.None;

    public int Size;
    public float Cost;
    public float Age = 0;
    public string Name;

    public float Damage = 0;
}

//[Serializable]
public class City
{
    public float Money = 100000;
    public Map map;

    public bool PlaceBuilding(int x, int y, BuildingTypes type)
    {
        if (!map.CellAtPositionIsType(x, y, CellType.Empty))
        {
            return false;
        }

        CityThing t = new CityThing();
        t.cell = map.CellAtPosition(x, y);
        t.type = CellType.Building;
        t.Size = (int)type;
        t.Name = "A building (" + type.ToString() + ")";
        t.state = ThingState.Normal;

        t.cell.thing = t;

        return true;
    }

    public bool PlaceRoad(int x, int y)
    {
        if (!map.CellAtPositionIsType(x, y, CellType.Empty))
        {
            return false;
        }

        CityThing t = new CityThing();
        t.cell = map.CellAtPosition(x, y);
        t.type = CellType.Building;
        t.Size = 1;
        t.Name = "A road";
        t.state = ThingState.Normal;

        t.cell.thing = t;

        return true;
    }
}
