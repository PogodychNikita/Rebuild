using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ReplaceWithSuburb : Mission
{
    public ReplaceWithSuburb()
    { 
        
    }

    public override void Initialize(Location loc)
    {
        CurLoc = loc;

        MainScript = CurLoc.transform.parent.GetComponent<Game>();
        _clock = CurLoc.Clock.transform.GetChild(0).GetComponent<Text>();
        SelSurv = new List<Survivor>();

        RequareSurv = 3;
        RequareTime = 6;
        MinRequareTime = 3;
        MaxRequareTime = 6;
        MaxDanger = 0;
    }

    public override void Calculate(List<Survivor> sel)
    {
        SelSurv.Clear();
        int newRequareTime = MaxRequareTime;

        for (int i = 0; i < sel.Count; i++)
        {
            SelSurv.Add(sel[i]);
            SelSurv[i].Status = "Building Suburb";
            if (newRequareTime > MinRequareTime)
                newRequareTime -= (int)SelSurv[i].SpecBonus.BuilderBonus;
        }

        RequareTime = newRequareTime;
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
            Index tempId = CurLoc.GetComponent<Location>().Id;
            Vector3 tempPos = CurLoc.transform.localPosition;
            int tempSiblingIndex = CurLoc.transform.GetSiblingIndex();

            GameObject newGameObj = GameObject.Instantiate(Resources.Load("Prefabs/Locations/Suburb")) as GameObject;
            newGameObj.transform.SetParent(MainScript.transform);
            newGameObj.transform.SetSiblingIndex(tempSiblingIndex);
            newGameObj.transform.localPosition = tempPos;
            newGameObj.GetComponent<Location>().Initialize(tempId);
            newGameObj.GetComponent<Location>().Neighbours = CurLoc.Neighbours;
            GameObject.Destroy(MainScript.Map[tempId.x][tempId.y].gameObject);
            MainScript.Map[tempId.x][tempId.y] = newGameObj.GetComponent<Location>();

            foreach (Location loc in newGameObj.GetComponent<Location>().Neighbours)
            {
                loc.NeighboursAttached = false;
                loc.Neighbours.Clear();
                loc.UpdateNeighbours();
            }
            newGameObj.GetComponent<Location>().Reclaim();

            for (int i = 0; i < SelSurv.Count; i++)
                SelSurv[i].Unlock();

            foreach (Survivor surv in SelSurv)
                if (surv.Alive)
                    surv.Train(new SpecialisationBonus(0, 0, 1, 0, 0));

            MainScript.ActiveMissions.Remove(this);
        }
    }
}
