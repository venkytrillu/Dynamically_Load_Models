using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName ="ModelNames",menuName = "ModelsData")]
public class ScriptableData : ScriptableObject
{
    public string[] ClonedNames;
    public GameObject[] ClonedObjects;

}
