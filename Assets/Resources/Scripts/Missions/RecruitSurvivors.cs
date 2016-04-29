using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public class RecruitSurvivors : Mission
{
    public RecruitSurvivors()
    { 
        
    }

    public override void Initialize(Location loc)
    {
        CurLoc = loc;

        MainScript = CurLoc.transform.parent.GetComponent<Game>();
        _clock = CurLoc.Clock.transform.GetChild(0).GetComponent<Text>();
        SelSurv = new List<Survivor>();

        RequareSurv = 1;
        RequareTime = 2;
        MinRequareTime = 2;
        MaxRequareTime = 2;

        if (CurLoc.Scouted)
        {
            MaxDanger = 10;
            MinDanger = 0;
        }
        else
        {
            MaxDanger = 30;
            MinDanger = 10;
        }

        Danger = MaxDanger;     
    }

    public override void Calculate(List<Survivor> sel)
    {
        if (CurLoc.Scouted)
        {
            if (CurLoc.ZombiesAmount > 20)
                MaxDanger = 10 + CurLoc.ZombiesAmount - 20;
            else MaxDanger = 10;

            MinDanger = 0;
        }
        else
        {
            MaxDanger = 30;
            MinDanger = 10;
        }

        Danger = MaxDanger;

        SelSurv.Clear();
        RequareTime = MaxRequareTime;
        int newDanger = MaxDanger;

        for (int i = 0; i < sel.Count; i++)
        {
            SelSurv.Add(sel[i]);
            SelSurv[i].Status = "Recruting survivors";
            newDanger -= (int)(SelSurv[i].SpecBonus.SoldierBonus * 3f);

            if (newDanger < MinDanger)
            {
                newDanger = 0;
                break;
            }
        }

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
            foreach (Survivor selSurv in SelSurv)
            {
                if (selSurv.SpecBonus.LeaderBonus >= 1)
                {
                    for (int i = 0; i < CurLoc.SpotSurv.Count; i++)
                    {
                        MainScript.AddSurvivor(CurLoc.SpotSurv[i]);
                    }

                    break;
                }
            }

            CurLoc.SpotSurv.Clear();

            for (int i = 0; i < SelSurv.Count; i++)
                SelSurv[i].Unlock();

            if (Danger != 0)
            {
                int chance = Random.Range(0, 100);

                if (chance > 0 && chance < Danger)
                {
                    int tempRange = Random.Range(0, SelSurv.Count);
                    SelSurv[tempRange].Status = "Die at Reacruting survivors mission";
                    SelSurv[tempRange].Die();
                }
            }

            foreach (Survivor surv in SelSurv)
                if (surv.Alive)
                    surv.Train(new SpecialisationBonus(1, 0, 0, 0, 0));

            _clock.transform.parent.gameObject.SetActive(false);
            MainScript.ActiveMissions.Remove(this);
        }
    }

    public override bool CheckConditions()
    {
        if (!CurLoc.Scouted)
        {
            Status = "This location has not been scouted";
            return false;
        }
        else if (CurLoc.Scouted && CurLoc.SpotSurv.Count == 0)
        {
            Status = "No survivors at this location";
            return false;
        }
        else if (!CurLoc.Scouted && CurLoc.SpotSurv.Count == 0 && MainScript.LivingSpace == 0)
        {
            Status = "They'd have nowhere to live, we need more houses first";
            return false;
        }
        else return true;
    }
}