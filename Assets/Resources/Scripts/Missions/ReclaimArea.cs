using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public class ReclaimArea : Mission
{
    public ReclaimArea()
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

    public override void Calculate(List<Survivor> sel)
    {
        SelSurv.Clear();
        int newRequareTime = MaxRequareTime;
        int newDanger = MaxDanger;

        for (int i = 0; i < sel.Count; i++)
        {
            SelSurv.Add(sel[i]);
            SelSurv[i].Status = "Reclaiming the area";
            if (newRequareTime > MinRequareTime)
                newRequareTime -= (int)SelSurv[i].SpecBonus.BuilderBonus;
            if (newDanger > MinDanger)
                newDanger -= (int)SelSurv[i].SpecBonus.SoldierBonus*10;
        }

        RequareTime = newRequareTime;
        Danger = newDanger;
    }

    public override void Execute()
    {
        if (RequareTime > 1)
        {
            RequareTime--;
            _clock.text = RequareTime.ToString();
        }
        else
        {
            CurLoc.Reclaim();
            _clock.transform.parent.gameObject.SetActive(false);

            for (int i = 0; i < SelSurv.Count; i++)
            {
                SelSurv[i].Status = "Idle";
                SelSurv[i].Unlock();
            }

            if (Danger != 0)
            {
                int chance = Random.Range(0, 100);

                if (chance > 0 && chance < Danger)
                {
                    int tempRange = Random.Range(0, SelSurv.Count);
                    SelSurv[tempRange].Status = "Die at Reclaim Area mission";
                    SelSurv[tempRange].Die();
                }
            }

            foreach (Survivor surv in SelSurv)
                if (surv.Alive)
                    surv.Train(new SpecialisationBonus(0, 0, 1, 0, 0));

            MainScript.ActiveMissions.Remove(this);
        }
    }

    public override bool CheckConditions()
    {
        if (CurLoc.ZombiesAmount > 0)
        {
            Status = "Ther are to many zombies at this location";
            return false;
        }
        else if (CurLoc.ZombiesAmount == 0 && !CurLoc.CheckNeighbours())
        {
            Status = "This location is to far from fort";
            return false;
        }
        else return true;
    }
}