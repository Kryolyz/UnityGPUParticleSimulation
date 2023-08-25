using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ApplyGravityProgram : StandardShaderProgram
{
    [SerializeField]
    private Vector2 gravity = new Vector2(0, -30);

    private void OnValidate()
    {
        init();
    }

    public override void init()
    {
        base.loadShader("ApplyGravity");
    }

    private void FixedUpdate()
    {
        shader.SetVector("g", gravity);
    }
}
