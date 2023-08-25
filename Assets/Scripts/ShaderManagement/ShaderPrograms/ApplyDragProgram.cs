using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ApplyDragProgram : StandardShaderProgram
{
    [SerializeField]
    private float drag = 40;

    private void OnValidate()
    {
        init();
    }

    public override void init()
    {
        loadShader("ApplyDrag");
        shader.SetFloat("drag", drag);
    }
}
