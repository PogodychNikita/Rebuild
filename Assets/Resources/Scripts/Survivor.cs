using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Survivor : MonoBehaviour
{
    public Game Main;
    public Text SurvStatus;
    public Text Hint;
    public Text NameText;
    public Image ButtonImage;
    public Button ButtonScript;
    public EventTrigger TriggerScript;
    public SpecialisationBonus SpecBonus;
    public RectTransform RectTransf;
    public int Leader;
    public int Scavanger;
    public int Soldier;
    public int Builder;
    public int Scientist;
    public string Specialisation;
    public Vector3 AncoredPos
    {
        get { return RectTransf.anchoredPosition3D; }
        set { RectTransf.anchoredPosition3D = value; }
    }

    public string Status
    {
        get { return SurvStatus.text; }
        set { SurvStatus.text = value; }
    }

    public string Name
    {
        get { return NameText.text; }
        set { NameText.text = value; }
    }

    public bool Selected;
    public bool Alive;
    public int Id;

    public void Initialize(int id, Game main)
    {   
        Main = main;
        Status = "Idle";
        Alive = true;
        Id = id;
        AddButtonEvent();
        RectTransf = GetComponent<RectTransform>();
        SpecBonus = new SpecialisationBonus(Leader, Soldier, Builder, Scavanger, Scientist);
    }

    public void Die ()
    {
        Lock();
        Alive = false;
    }

    public void AddMissionListener()
    {
        ButtonScript.onClick.RemoveAllListeners();
        ButtonScript.onClick.AddListener(() => Main.SelectSurvivor(Id));
    }

    public void AddDefendListener()
    {
        ButtonScript.onClick.RemoveAllListeners();
        ButtonScript.onClick.AddListener(() => Main.SelectDefender(Id));
    }

    public void AddButtonEvent()
    {
        EventTrigger.Entry entryEnter = new EventTrigger.Entry();
        EventTrigger.Entry entryExit = new EventTrigger.Entry();

        entryEnter.eventID = EventTriggerType.PointerEnter;
        entryEnter.callback.AddListener((eventData) => Main.EnterSelectedButton(Id));
        entryExit.eventID = EventTriggerType.PointerExit;
        entryExit.callback.AddListener((eventData) => Main.ExitSelectedButton(Id));

        TriggerScript.triggers.Clear();
        TriggerScript.triggers.Add(entryEnter);
        TriggerScript.triggers.Add(entryExit);
    }

    public void Lock()
    {
        Selected = true;
        ButtonScript.enabled = false;
        ButtonImage.color = new Color(0.3f, 0.3f, 0.3f);
    }

    public void Unlock()
    {
        Status = "Idle";
        Selected = false;
        ButtonScript.enabled = true;
        ButtonImage.color = new Color(1, 1, 1);
    }

    public void Select()
    {
        Selected = true;
        ButtonImage.color = new Color(0.7f, 0.5f, 0.5f);   
    }

    public void Deselect()
    {
        Status = "Idle";
        Selected = false;
        ButtonImage.color = new Color(1, 1, 1);
    }

    public void Train(SpecialisationBonus specBonus)
    {
        if (Specialisation == "Untrained")
        {
            SpecBonus = specBonus + SpecBonus;
            if (SpecBonus.LeaderBonus >= 1)
                ChangeSpec("Leader");
            else if (SpecBonus.ScavangerBonus >= 1)
                ChangeSpec("Scavanger");
            else if (SpecBonus.SoldierBonus >= 1)
                ChangeSpec("Soldier");
            else if (SpecBonus.BuilderBonus >= 1)
                ChangeSpec("Builder");
            else if (SpecBonus.ScientistBonus >= 1)
                ChangeSpec("Scientist");
        }
    }

    public void ChangeSpec(string newSpec)
    {
        GameObject newGameObj = GameObject.Instantiate(Resources.Load("Prefabs/Survivors/" + newSpec)) as GameObject;
        newGameObj.transform.SetParent(Main.SurvButtonsCont);
        newGameObj.transform.SetSiblingIndex(Id);
        newGameObj.GetComponent<Survivor>().Initialize(Id, Main);
        newGameObj.GetComponent<Survivor>().AncoredPos = AncoredPos;

        Main.Survivors.Insert(Id, newGameObj.GetComponent<Survivor>());
        Main.Survivors.Remove(this);
        Destroy(gameObject);
    }
}
