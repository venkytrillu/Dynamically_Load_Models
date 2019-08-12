using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonInteract : MonoBehaviour
{
    public static ButtonInteract instance;
    Animator anim;
    Button buttonField;
    Text childTextField;
    public int refNum;
    private void Awake()
    {
        if(instance=null)
        {
            instance = this;
        }
    }
    private void Start()
    {
        buttonField = transform.GetComponent<Button>();
        childTextField = gameObject.transform.GetChild(0).GetComponent<Text>();
        OnClickEvent();
        SetText(JsonLoaderScript.instance.scriptableData.ClonedObjects[refNum].name);
    }

    public void SetText(string _text)
    {
        childTextField.text = _text;
    }

    void MethodEvent()
    {
        if(UIManager.instance.ClonedGameObject==null)
        {
            GameObject obj = JsonLoaderScript.instance.SpawnGameobject(JsonLoaderScript.instance.scriptableData.ClonedObjects[refNum]);
            SetTransform(obj);
            UIManager.instance.CloseModelBtnView();
            UIManager.instance.ClonedGameObject = obj;
            JsonLoaderScript.instance.SetMaterial(obj);
            SetAnimation(obj);
            print(obj.name);
        }
    }
    public void OnClickEvent()
    {
        buttonField.onClick.AddListener(MethodEvent);
    }

    void SetTransform(GameObject obj)
    {
        for(int i=0;i<UIManager.instance.objTransform.Length;i++)
        {
            if(obj.name.Contains(UIManager.instance.objTransform[i].name))
            {
                obj.transform.position = UIManager.instance.objTransform[i].transform.position;
                obj.transform.rotation = UIManager.instance.objTransform[i].transform.rotation;
                obj.transform.localScale = UIManager.instance.objTransform[i].transform.localScale;
            }
        }
    }

    void SetAnimation(GameObject obj)
    {
        if(obj.name.StartsWith(Tags.chotta))
        {
            anim = obj.GetComponent<Animator>();
            if(anim==null)
            {
                print("Please Redownload the Models. Animator is nor Assign to This Model");
               // anim= obj.AddComponent<Animator>();
            }
            RuntimeAnimatorController ChotabeamController = (RuntimeAnimatorController)Resources.Load("ChotabeamController", typeof(RuntimeAnimatorController));
            anim.runtimeAnimatorController = ChotabeamController;
            anim.Play(Tags.AnimJump);
            //print(ChotabeamController.name + " " + anim.gameObject.name);
        }
    }

 
} // class
