using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ResetGridProgram : StandardShaderProgram
{
    private void OnValidate()
    {
        init();
    }

    public override void init()
    {
        loadShader("ResetGrid");
    }
}
