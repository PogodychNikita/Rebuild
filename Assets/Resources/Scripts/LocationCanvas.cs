using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LocationCanvas : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Transform MissionButtonsCont;
    public GameObject AssignDefButton;
    public CameraMove currentCamera;
    public Image SurvImage;
    public Image FoodImage;
    public Image SafeImage;
    public Text LocationName;
    public Text ScoutedText;
    public Text SurvText;
    public Text FoodText;
    public Text SafeText;
    public Text LocationDescriptionText;

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

    public void ShowStatus()
    {
        ScoutedText.gameObject.SetActive(false);

        LocationName.gameObject.SetActive(true);
        SurvImage.gameObject.SetActive(true);
        FoodImage.gameObject.SetActive(true);
        SafeImage.gameObject.SetActive(true);
        SurvText.gameObject.SetActive(true);
        FoodText.gameObject.SetActive(true);
        SafeText.gameObject.SetActive(true);
        LocationDescriptionText.gameObject.SetActive(false);
    }

    public void ShowRecStatus()
    {
        ScoutedText.gameObject.SetActive(false);

        LocationName.gameObject.SetActive(true);
        SurvImage.gameObject.SetActive(false);
        FoodImage.gameObject.SetActive(false);
        SafeImage.gameObject.SetActive(false);
        SurvText.gameObject.SetActive(false);
        FoodText.gameObject.SetActive(false);
        SafeText.gameObject.SetActive(false);
        LocationDescriptionText.gameObject.SetActive(true);
    }

    public void HideStatus()
    {
        ScoutedText.gameObject.SetActive(true);

        LocationName.gameObject.SetActive(false);
        SurvImage.gameObject.SetActive(false);
        FoodImage.gameObject.SetActive(false);
        SafeImage.gameObject.SetActive(false);
        SurvText.gameObject.SetActive(false);
        FoodText.gameObject.SetActive(false);
        SafeText.gameObject.SetActive(false);
        LocationDescriptionText.gameObject.SetActive(false);
    }

    public void HideAll()
    {
        ScoutedText.gameObject.SetActive(false);

        LocationName.gameObject.SetActive(false);
        SurvImage.gameObject.SetActive(false);
        FoodImage.gameObject.SetActive(false);
        SafeImage.gameObject.SetActive(false);
        SurvText.gameObject.SetActive(false);
        FoodText.gameObject.SetActive(false);
        SafeText.gameObject.SetActive(false);
        LocationDescriptionText.gameObject.SetActive(false);
    }
}