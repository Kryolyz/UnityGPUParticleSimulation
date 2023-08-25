using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/UpdatableGlobalVariable", order = 2)]
public class UpdatableGlobalVariable : ScriptableObject
{
    [SerializeField]
    public float floatVar;
    [SerializeField]
    public Vector2 vector2Var;
    [SerializeField]
    public int intVar;

    public UpdatableGlobalVariable (float var)
    {
        this.floatVar = var;
    }

    public UpdatableGlobalVariable(Vector2 var)
    {
        this.vector2Var = var;
    }

    public UpdatableGlobalVariable(int var)
    {
        this.intVar = var;
    }
}
