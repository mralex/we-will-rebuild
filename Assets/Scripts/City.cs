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
    public CityThingView view;
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
    public float Money = 200000;
    public Map map;

    public float damageReceived = 0;
    public float damageDealt = 0;

    public float totalDamageReceived;
    public float totalDamageDealt;
    public float totalMoney = 120000;
    public int totalBuildings;

    public int BuildingCount {
        get
        {
            int count = 0;

            foreach(MapCell c in map.Cells)
            {
                if (c.type == CellType.Building || c.type == CellType.Defense)
                {
                    count++;
                }
            }

            return count;
        }
    }

    public int DamagedBuildingCount
    {
        get
        {
            int count = 0;

            foreach (MapCell c in map.Cells)
            {
                if ((c.type == CellType.Building || c.type == CellType.Defense) && c.thing.Damage > 0)
                {
                    count++;
                }
            }

            return count;
        }
    }

    public bool PlaceBuilding(int x, int y, BuildingTypes type, bool free=false)
    {
        if (!map.CellAtPositionIsType(x, y, CellType.Empty) || (!free && Money - 2000 < 0))
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

        totalBuildings++;

        if (!free)
            Money -= 2000;

        return true;
    }

    public bool PlaceDefense(int x, int y, bool free = false)
    {
        if (!map.CellAtPositionIsType(x, y, CellType.Empty) || (!free && Money - 50000 < 0))
        {
            return false;
        }

        CityThing t = new CityThing();
        t.cell = map.CellAtPosition(x, y);
        t.type = CellType.Defense;
        t.Size = 0;
        t.Name = "A rocket launcher";
        t.state = ThingState.Normal;

        t.cell.thing = t;

        totalBuildings++;

        if (!free)
            Money -= 50000;

        return true;
    }

    public bool PlaceRoad(int x, int y)
    {
        if (!map.CellAtPositionIsType(x, y, CellType.Empty) || Money - 800 < 0)
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

        Money -= 800;

        return true;
    }


    public bool PlaceTree(int x, int y, bool free = false)
    {
        if (!map.CellAtPositionIsType(x, y, CellType.Empty) || (!free && Money - 400 < 0))
        {
            return false;
        }

        CityThing t = new CityThing();
        t.cell = map.CellAtPosition(x, y);
        t.type = CellType.Park;
        t.Size = 1;
        t.Name = "A tree";
        t.state = ThingState.Normal;

        t.cell.thing = t;

        if (!free)
            Money -= 400;

        return true;
    }

    public void DestroyAt(int x, int y)
    {
        if (map.CellAtPositionIsType(x, y, CellType.Empty))
        {
            return;
        }

        map.CellAtPosition(x, y).thing = null;
    }

    public void EngageLaunchers()
    {
        foreach (MapCell c in map.Cells)
        {
            if (c.type == CellType.Defense)
            {
                c.thing.view.gameObject.GetComponentInChildren<LauncherView>().IsLive = true;
            }
        }
    }

    public void DisengageLaunchers()
    {
        foreach (MapCell c in map.Cells)
        {
            if (c.type == CellType.Defense)
            {
                c.thing.view.gameObject.GetComponentInChildren<LauncherView>().IsLive = false;
            }
        }
    }
}
