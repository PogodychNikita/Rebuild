using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class TrainSoldier : Mission
{
    public TrainSoldier()
    { 
    
    }

    public override void Initialize(Location loc)
    {
        CurLoc = loc;

        MainScript = CurLoc.transform.parent.GetComponent<Game>();
        _clock = CurLoc.Clock.transform.GetChild(0).GetComponent<Text>();
        SelSurv = new List<Survivor>();

        RequareSurv = 2;
        RequareTime = 4;
        MinRequareTime = 2;
        MaxRequareTime = 4;
        MaxDanger = 10;
        MinDanger = 0;

        Danger = MaxDanger;
    }
}
