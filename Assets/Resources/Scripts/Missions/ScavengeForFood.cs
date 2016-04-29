using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ScavengeForFood : Mission
{
    public ScavengeForFood()
    {

    }

    public override void Initialize(Location loc)
    {
        CurLoc = loc;

        MainScript = CurLoc.transform.parent.GetComponent<Game>();
        SelSurv = new List<Survivor>();

        RequareSurv = 1;
        RequareTime = 0;
        MinRequareTime = 0;
        MaxRequareTime = 0;
        MaxDanger = 0;
        MinDanger = 0;

        _clock = CurLoc.Clock.transform.GetChild(0).GetComponent<Text>();
    }

    public override bool CheckConditions()
    {
        if (CurLoc.FoodAmount == 0)
        {
            Status = "Ther is no food at this location";
            return false;
        }
        else return true;
    }
}