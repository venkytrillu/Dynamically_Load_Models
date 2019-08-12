using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tags
{
    public static string house = "house";
    public static string chotta = "chotta";
    public static string iron = "iron";
    public static string Wolf_Body = "Wolf_Body";
    public static string Wolf = "Wolf";
    public static string MaterialPath = "Materials/Mat";
    public static string TexturesPath = "Textures/";
    public static string Tyre_texture = "tyre_texture";
    public static string Tyre= "tyre";
    public static string AnimJump = "Jump";
}
public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public Text myResultBox, waitingTxt;
    public GameObject PanelModelBtns, PanelDownloadModels, buttonView,ButtonParent, PanelCloseBtn;
    public Button buttonPrefab;
    public Slider slider;
    public Transform[] objTransform;

    public GameObject ClonedGameObject;
    private void Awake()
    {
        if(instance==null)
        {
            instance = this;
        }
    }

    void Start()
    {
        ClonedGameObject = null;
    }

    public void SetSliderValue(float progress)
    {
        slider.value = progress;
        DownloadCompleted();
    }

    public void DownloadCompleted()
    {
        if(slider.value==slider.maxValue)
        {
            waitingTxt.text = "downloaded..!";
            waitingTxt.color = Color.green;
        }
    }

    public void AfterDownloadModels()
    {
        myResultBox.gameObject.SetActive(false);
        buttonView.gameObject.SetActive(true);
        SetSliderValue(slider.maxValue);

    }

    public void CloseModelBtnView()
    {
        PanelCloseBtn.SetActive(true);
        PanelModelBtns.SetActive(false);
    }

    public void OpenModellView()
    {

        PanelModelBtns.SetActive(true);
        PanelCloseBtn.SetActive(false);
        if (ClonedGameObject)
            Destroy(ClonedGameObject);
    }

}// class
