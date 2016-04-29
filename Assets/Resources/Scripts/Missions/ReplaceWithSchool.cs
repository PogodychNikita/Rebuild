using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ReplaceWithSchool : Mission
{
    public ReplaceWithSchool()
    {

    }

    public override void Initialize(Location loc)
    {
        CurLoc = loc;

        MainScript = CurLoc.transform.parent.GetComponent<Game>();
        SelSurv = new List<Survivor>();

        RequareSurv = 0;
        RequareTime = 0;
        MinRequareTime = 0;
        MaxRequareTime = 0;
        MaxDanger = 0;
        MinDanger = 0;

        _clock = CurLoc.Clock.transform.GetChild(0).GetComponent<Text>();
    }
}