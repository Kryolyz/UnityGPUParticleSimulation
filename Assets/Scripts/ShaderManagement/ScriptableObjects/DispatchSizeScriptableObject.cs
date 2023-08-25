using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ShaderDispatchSize", order = 1)]
public class DispatchSizeScriptableObject : ScriptableObject
{
    [SerializeField]
    public Vector3Int dispatchSize;
}
