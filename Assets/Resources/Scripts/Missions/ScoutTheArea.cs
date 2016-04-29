using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public class ScoutTheArea : Mission
{
    public ScoutTheArea()
    { 
        
    }

    public override void Initialize(Location loc)
    {
        CurLoc = loc;

        MainScript = CurLoc.transform.parent.GetComponent<Game>();
        _clock = CurLoc.Clock.transform.GetChild(0).GetComponent<Text>();
        SelSurv = new List<Survivor>();

        RequareSurv = 1;
        RequareTime = 1;
        MinRequareTime = 1;
        MaxRequareTime = 1;

        MaxDanger = 3;
        MinDanger = 0;

        Danger = MaxDanger;
    }

    public override void Calculate(List<Survivor> sel)
    {
        MaxDanger = 3;
        MinDanger = 0;

        Danger = MaxDanger;

        SelSurv.Clear();
        RequareTime = MaxRequareTime;
        int newDanger = MaxDanger;

        for (int i = 0; i < sel.Count; i++)
        {
            SelSurv.Add(sel[i]);
            SelSurv[i].Status = "Scouting area";
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
            for (int i = 0; i < SelSurv.Count; i++)
            {
                if (CurLoc.SurvPres.Count > 1 && SelSurv[i].SpecBonus.ScavangerBonus > 1)
                {
                    int spotAmount = Random.Range(1, CurLoc.SurvPres.Count);

                    for (int j = 0; j < spotAmount; j++)
                    {
                        CurLoc.SpotSurv.Add(CurLoc.SurvPres[j]);
                    }

                    break;
                }
                else if (CurLoc.SurvPres.Count != 0 && SelSurv[i].SpecBonus.ScavangerBonus >= 0)
                {
                    int spotAmount = Random.Range(1, CurLoc.SurvPres.Count);

                    for (int j = 0; j < spotAmount; j++)
                    {
                        CurLoc.SpotSurv.Add(CurLoc.SurvPres[j]);
                        if (j > 1) break;
                    }

                    break;
                }
                else if (CurLoc.SurvPres.Count == 0)
                {
                    break;
                }
            }

            CurLoc.Scouted = true;
            for (int i = 0; i < SelSurv.Count; i++)
                SelSurv[i].Unlock();

            if (Danger != 0)
            {
                int chance = Random.Range(0, 100);

                if (chance > 0 && chance < Danger)
                {
                    int tempRange = Random.Range(0, SelSurv.Count);
                    SelSurv[tempRange].Status = "Die at Scouting area mission";
                    SelSurv[tempRange].Die();
                }
            }

            foreach (Survivor surv in SelSurv)
                if (surv.Alive)
                    surv.Train(new SpecialisationBonus(0, 1, 0, 0, 0));

            _clock.transform.parent.gameObject.SetActive(false);
            MainScript.ActiveMissions.Remove(this);
        }
    }

    public override void SortSurvButtons(List<Survivor> survivors)
    {
        List<Vector3> tempButtonPos = new List<Vector3>();

        foreach (Survivor surv in survivors)
        {
            tempButtonPos.Add(surv.AncoredPos);
            //Debug.Log(surv.AncoredPos);
        }
    }

    public override bool CheckConditions()
    {
        if (CurLoc.Scouted)
        {
            Status = "This location has already been scouted";
            return false;
        }
        else return true;
    }
}
