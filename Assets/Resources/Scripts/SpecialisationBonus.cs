using UnityEngine;
using System.Collections;

public class SpecialisationBonus 
{
    public float LeaderBonus;
    public float SoldierBonus;
    public float BuilderBonus;
    public float ScavangerBonus;
    public float ScientistBonus;

    public SpecialisationBonus()
    {
        LeaderBonus = 0;
        SoldierBonus = 0;
        BuilderBonus = 0;
        ScavangerBonus = 0;
        ScientistBonus = 0;
    }

    public SpecialisationBonus(float leaderBonus, float soldierBonus, float builderBonus, float scavangerBonus, float scientistBonus)
    {
        LeaderBonus = leaderBonus;
        SoldierBonus = soldierBonus;
        BuilderBonus = builderBonus;
        ScavangerBonus = scavangerBonus;
        ScientistBonus = scientistBonus;
    }

    public static SpecialisationBonus operator +(SpecialisationBonus specBonus1, SpecialisationBonus specBonus2)
    {
        SpecialisationBonus specBonus = new SpecialisationBonus();
        specBonus.LeaderBonus = specBonus1.LeaderBonus + specBonus2.LeaderBonus;
        specBonus.SoldierBonus = specBonus1.SoldierBonus + specBonus2.SoldierBonus;
        specBonus.BuilderBonus = specBonus1.BuilderBonus + specBonus2.BuilderBonus;
        specBonus.ScavangerBonus = specBonus1.ScavangerBonus + specBonus2.ScavangerBonus;
        specBonus.ScientistBonus = specBonus1.ScientistBonus + specBonus2.ScientistBonus;
        return specBonus;
    }
}
