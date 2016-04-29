using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public class KillZombies : Mission
{
    public KillZombies()
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
            MaxDanger = 10;
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
            SelSurv[i].Status = "Killing zombies";
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
                SelSurv[i].Status = "Idle";
                SelSurv[i].Unlock();
            }

            for (int i = 0; i < SelSurv.Count; i++)
            {
                CurLoc.ZombiesAmount -= (int)SelSurv[i].SpecBonus.SoldierBonus*2 + 1;

                if (CurLoc.ZombiesAmount <= 0)
                {
                    CurLoc.ZombiesAmount = 0;
                    break;
                }
            }

            if (Danger != 0)
            {
                int chance = Random.Range(0, 100);

                if (chance > 0 && chance < Danger)
                {
                    int tempRange = Random.Range(0, SelSurv.Count);
                    SelSurv[tempRange].Status = "Die at Killing Zombies mission";
                    SelSurv[tempRange].Die();
                }
            }

            CurLoc.ChangeInfestedStatus();

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
        List<Survivor> tempSortedSurv = new List<Survivor>();
        List<Survivor> endList = new List<Survivor>();

        for (int i = 0; i < survivors.Count; i++)
            tempSortedSurv.Add(survivors[i]);

        for (int i = 0; i < survivors.Count; i++)
        {
            if (survivors[i].Selected)
                endList.Add(survivors[i]);

            tempButtonPos.Add(new Vector3(-100, -(i * 40)-5, 0));
        }

        for (int i = 0; i < survivors.Count; i++)
        {
            for (int j = 0; j < endList.Count; j++)
            {
                if (survivors[i].Id == endList[j].Id)
                {
                    tempSortedSurv.Remove(survivors[i]);
                    break;
                }
            }
        }

        tempSortedSurv = survivors.OrderBy(s => s.Specialisation).ToList<Survivor>();

        for (int i = 0; i < survivors.Count; i++)
        {
            tempSortedSurv[i].AncoredPos = tempButtonPos[i];
            for (int j = 0; j < survivors.Count; j++)
            {
                if (tempSortedSurv[i].Id == survivors[j].Id)
                {
                    survivors[j].AncoredPos = tempSortedSurv[i].AncoredPos;
                    break;
                }
            }   
        }
    }

    public override bool CheckConditions()
    {
        if (CurLoc.ZombiesAmount == 0)
        {
            Status = "Ther is no zombies at this location";
            return false;
        }
        else return true;
    }
}
