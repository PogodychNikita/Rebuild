using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StatusBar : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler 
{
    public Text SurvivorsNum;
    public Text LivingSpaceNum;
    public Text FoodNum;
    public Text HappinessNum;
    public Text CityName;
    public Text DefenseNum;
    public Text DefenseFromBuildingNum;
    public Text DangerNum;
    public Text Days;
    public CameraMove currentCamera;

    public void OnPointerEnter(PointerEventData eventData)
    {
        currentCamera.EnterUi = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        currentCamera.EnterUi = false;
    }

    public void ShowFortStats()
    {
        DefenseNum.gameObject.SetActive(true);
        DefenseFromBuildingNum.gameObject.SetActive(true);
        DangerNum.gameObject.SetActive(true);
    }

    public void ShowSurvStats()
    {
        LivingSpaceNum.gameObject.SetActive(true);
    }

    public void HideFortStats()
    {
        DefenseNum.gameObject.SetActive(false);
        DefenseFromBuildingNum.gameObject.SetActive(false);
        DangerNum.gameObject.SetActive(false);
    }

    public void HideSurvStats()
    {
        LivingSpaceNum.gameObject.SetActive(false);
    }

    public void UpdateStats(int days, int defense, int defFromBuild, int livingSpace, int happiness)
    {
        Days.text = days.ToString();
        DefenseNum.text = defense.ToString();
        DefenseFromBuildingNum.text = defFromBuild.ToString();
        LivingSpaceNum.text = livingSpace.ToString();
        HappinessNum.text = happiness.ToString();
    }
}
