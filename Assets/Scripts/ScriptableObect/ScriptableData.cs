using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName ="ModelNames",menuName = "ModelsData")]
public class ScriptableData : ScriptableObject
{
    public string[] ClonedNames;
    public GameObject[] ClonedObjects;
    public int numberOfFiles;

} // class
