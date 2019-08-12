using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using UnityEngine.UI;
using Newtonsoft.Json;
using UnityEngine.Networking;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
[Serializable]
public class Creature
{
    public string model_id { get; set; }
    public string model_name { get; set; }
    public string model_image { get; set; }
    public string model_file { get; set; }
}

[Serializable]
public class ModelList
{
    public Creature[] model_list { get; set; }
}


public class JsonLoaderScript : MonoBehaviour
{

    public string url;
    public ScriptableData scriptableData;
    ModelList listofModels;
    int downloadCount;
    public static JsonLoaderScript instance;
    string trimPath = "http://dm-dev.southeastasia.cloudapp.azure.com:8080/get_image/";
    public void Awake()
    {
        if(instance== null)
        {
            instance = this;
        }

        try
        {
            if (Application.internetReachability != NetworkReachability.NotReachable)
            {
                StartCoroutine(GetJsonValues(url));
            }
        }
        catch(Exception ex)
        {
            UIManager.instance.myResultBox.text = ex.Message+ " : No internet connected";

        }
    }

    private void Start()
    {
        UIManager.instance.myResultBox.text = "";

        CheckFolderPaths();
    }

    void CheckFolderPaths()
    {
        if (!Directory.Exists(Application.dataPath + "/Resources/Textures/"))
        {
            CreateFolders(Application.dataPath + "/Resources/Textures/");
        }
        if (!Directory.Exists(Application.dataPath + "/Resources/Models/"))
        {
            CreateFolders(Application.dataPath + "/Resources/Models/");
        }
    }
    void CreateFolders(string path)
    {
        var folder = Directory.CreateDirectory(path);
        AssetDatabase.Refresh();
    }
    public IEnumerator GetJsonValues(string _url)
    {
        float timeSent = Time.time;
        WWW www = new WWW(_url);
        yield return www;

        while (!www.isDone)
        {
            UIManager.instance.myResultBox.text = "Waiting ...";
            yield return null;
        }

       // print(www.progress);
        float duration = Time.time - timeSent;
        if (www.error != null && www.error.Length > 0)
        {
            UIManager.instance.myResultBox.text = "Error: " + www.error + " (" + duration + " secs)";
            yield break;
        }
        //print("Success (" + duration + " secs)");
        //print("Result: " + www.text);
        if(www.isDone)
          Handle(www.text);
    }
    void Handle(string jsonString)
    {
        if (jsonString != null)
        {
            listofModels = new ModelList();
            JsonConvert.PopulateObject(jsonString, listofModels);
            UIManager.instance.slider.maxValue = listofModels.model_list.Length;
            if (scriptableData.ClonedObjects.Length == 0 && 
                scriptableData.ClonedNames.Length == 0 )
            {
                scriptableData.ClonedObjects = new GameObject[listofModels.model_list.Length];
                scriptableData.ClonedNames = new string[listofModels.model_list.Length];
            }
            foreach (var data in listofModels.model_list)
            {
                StartCoroutine(GetTextures(data.model_image,data));
                StartCoroutine(GetModeles(data.model_file, data));
            } 
        }
    }
    public void ViewModels()
    {
        StartCoroutine(AssignRefOfGameObject(1f));
    }
    IEnumerator AssignRefOfGameObject(float delay)
    {
        yield return new WaitForSeconds(delay);
        for (int i = 0; i < scriptableData.ClonedNames.Length ; i++)
        {
            if(GetGameObject("Models/" + scriptableData.ClonedNames[i]))
                scriptableData.ClonedObjects[i] = GetGameObject("Models/" + scriptableData.ClonedNames[i])  ;
            CreateButtons(i);
        }
        UIManager.instance.PanelDownloadModels.SetActive(false);
        UIManager.instance.PanelModelBtns.SetActive(true);
        
    }  
    IEnumerator GetModeles(string _url,Creature data)
    {
        string write_path =Path.Combine(Application.dataPath+ "/Resources/Models/",  GetExetention(data.model_file));
        if(!File.Exists(write_path))
        {
            UnityWebRequest www = new UnityWebRequest(_url, UnityWebRequest.kHttpVerbGET);
            www.downloadHandler = new DownloadHandlerFile(write_path,false);
            yield return www.SendWebRequest();
           if(www.downloadHandler.isDone)
            {
                scriptableData.ClonedNames[downloadCount] = GetName(GetExetention(data.model_file));
                if (downloadCount == 3)
                {
                    AssetDatabase.Refresh();
                    AssetDatabase.ImportAsset(write_path);
                }
                downloadCount++;
                 // print("downloadCount " + downloadCount);
                DownloadCompleted();
                UIManager.instance.myResultBox.text += "Download Completed :" +GetName(GetExetention(data.model_file))+ "Model"+ "\n";
            }
            if (www.error != null)
            {
                Debug.Log(www.error);
            }
        }
        else
        {
            UIManager.instance.AfterDownloadModels();
            print("isCompleted");
        }
    }
    IEnumerator GetTextures(string _url, Creature data)
    {
        string write_path = Path.Combine(Application.dataPath + "/Resources/Textures/", GetExetention(data.model_image));
        if (!File.Exists(write_path))
        {
            UnityWebRequest www = new UnityWebRequest(_url, UnityWebRequest.kHttpVerbGET);
            if (GetSpriteName(GetExetention(data.model_image)).Contains("01.jpg"))
            {
                write_path = Path.Combine(Application.dataPath + "/Resources/Textures/", "house.jpg");
                www.downloadHandler = new DownloadHandlerFile(write_path, false);
            }
            else
            {
                www.downloadHandler = new DownloadHandlerFile(write_path, false);
            }
           
            yield return www.SendWebRequest();
            if (www.downloadHandler.isDone)
            {
                AssetDatabase.Refresh();
                AssetDatabase.ImportAsset(write_path);
                UIManager.instance.myResultBox.text += "Download Completed :" + GetSpriteName(GetExetention(data.model_image)) + "Texture" + "\n";
            }
            if (www.error != null)
            {
                Debug.Log(www.error);
            }
            else if (www.isDone)
            {
                print(write_path);
            }
        }
    }
    public void SetMaterial(GameObject obj)
    {
        obj.name = obj.name.Replace("(Clone)", "");
        Material mat = (Material)Resources.Load(Tags.MaterialPath, typeof(Material));
        obj.transform.GetComponentInChildren<Renderer>().material.enableInstancing = true;
        if (obj.name.StartsWith(Tags.chotta))
        {
            Texture tex = (Texture)Resources.Load(Tags.TexturesPath + Tags.chotta, typeof(Texture));
            obj.transform.GetComponentInChildren<Renderer>().material.mainTexture = tex;
        }
      else  if (obj.name.StartsWith(Tags.house))
        {
            Texture tex = (Texture)Resources.Load(Tags.TexturesPath + Tags.house, typeof(Texture));
            obj.transform.GetComponentInChildren<Renderer>().material.mainTexture = tex;
        }
        else if (obj.name.StartsWith(Tags.Wolf_Body))
        {
            Texture tex = (Texture)Resources.Load(Tags.TexturesPath + Tags.Wolf_Body, typeof(Texture));
            obj.transform.GetComponentInChildren<Renderer>().material.mainTexture = tex;
        }
        else if (obj.name.StartsWith(Tags.iron))
        {
            Texture tex = (Texture)Resources.Load(Tags.TexturesPath + Tags.iron, typeof(Texture));
            obj.transform.GetComponentInChildren<Renderer>().material.mainTexture = tex;
        }
    }

    GameObject GetGameObject(string name)
    {
        GameObject spawnedPrefab =(GameObject)Resources.Load(name, typeof(GameObject));
         return spawnedPrefab; 
    }

    public GameObject SpawnGameobject(GameObject obj)
    {
      return  Instantiate(obj, Vector3.zero, Quaternion.identity);
    }

    string GetExetention(string data)
    {
        string exetension = data.Replace(trimPath,"");
        return exetension;
    }
    string GetName(string data)
    {
        if(data.Contains(".fbx"))
        {
            string exetension = data.Replace(".fbx", "");
            return exetension;
        }
        if (data.Contains(".obj"))
        {
            string exetension = data.Replace(".obj", "");
            return exetension;
        }
        return null;
    }
    string GetSpriteName(string data)
    {
        if (data.Contains(".png"))
        {
            string exetension = data.Replace(".png", "");
            return exetension;
        }
        if (data.Contains(".jpg"))
        {
            string exetension = data.Replace(".jpg", "");
            return exetension;
        }
        return null;
    }

    void DownloadCompleted()
    {
        UIManager.instance.SetSliderValue(downloadCount);
        if (downloadCount== listofModels.model_list.Length)
        {
            UIManager.instance.buttonView.SetActive(true);
        }
    }

    public void CreateButtons(int index)
    {
      GameObject button=  SpawnGameobject(UIManager.instance.buttonPrefab.gameObject);
        button.transform.parent = UIManager.instance.ButtonParent.transform;
        button.GetComponent<ButtonInteract>().refNum = index;
    }


 }// cllass

