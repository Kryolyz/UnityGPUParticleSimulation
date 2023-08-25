using Unity.VisualScripting;
using UnityEngine;

public class ComputeCollisionsProgram: StandardShaderProgram
{
    private void OnValidate()
    {
        init();
    }

    public override void init()
    {
        base.loadShader("ComputeCollisions");
        shader.DisableKeyword("UPDATE_VELOCITY");
    }
}
