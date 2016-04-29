using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MissionButton : MonoBehaviour
{
    public Game Main;
    public Text MissionName;
    public Text MissionStatus;
    public int Id;

    public void AddButtonEvent()
    {
        EventTrigger trigger = GetComponent<EventTrigger>();
        EventTrigger.Entry entryEnter = new EventTrigger.Entry();
        EventTrigger.Entry entryExit = new EventTrigger.Entry();

        entryEnter.eventID = EventTriggerType.PointerEnter;
        entryEnter.callback.AddListener((eventData) => Main.EnterMissinButton(Id));
        entryExit.eventID = EventTriggerType.PointerExit;
        entryExit.callback.AddListener((eventData) => Main.ExitMissinButton(Id));

        trigger.triggers.Add(entryEnter);
        trigger.triggers.Add(entryExit);
    }

    public void ClearButtonEvent()
    {
        EventTrigger trigger = GetComponent<EventTrigger>();
        trigger.triggers.Clear();
    }

    public void ShowStatus( string messege )
    {
        MissionStatus.text = messege;
        AddButtonEvent();
        GetComponent<Button>().enabled = false;
        GetComponent<Image>().color = new Color(0.3f, 0.3f, 0.3f);
    }
}
