using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NamedThingBuilding : NamedThing
{
    public override string ThingName()
    {
        return GetComponent<CityThingView>().thing.Name;
    }
}
