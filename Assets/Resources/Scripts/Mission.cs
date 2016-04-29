using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public class Mission
{
    public Location CurLoc;
    public Game MainScript;
    public List<Survivor> SelSurv;
    public int RequareSurv;
    public int RequareTime;
    public int MinRequareTime;
    public int MaxRequareTime;
    public int Danger;
    public int MinDanger;
    public int MaxDanger;
    public string Status;

    protected Text _clock;

    public Mission()
    { 
        
    }

    public virtual void Initialize(Location loc)
    {

    }

    public virtual void Calculate(List<Survivor> sel)
    {

    }

    public virtual void Execute()
    {

    }

    public virtual void SortSurvButtons(List<Survivor> survivors)
    { 
        
    }

    public virtual bool CheckConditions()
    {
        return false;
    }
}