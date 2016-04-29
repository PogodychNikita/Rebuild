using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GuardingCanvas : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler 
{
    public Text DefenseNum;
    public CameraMove currentCamera;

    public void OnPointerEnter(PointerEventData eventData)
    {
        currentCamera.EnterUi = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        currentCamera.EnterUi = false;
    }

    public void Activate()
    {
        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        currentCamera.EnterUi = false;
        gameObject.SetActive(false);
    }
}
