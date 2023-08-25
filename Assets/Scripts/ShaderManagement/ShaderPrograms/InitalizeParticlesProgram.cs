using Unity.VisualScripting;
using UnityEngine;

public class InitalizeParticlesProgram : StandardShaderProgram
{
    [SerializeField]
    private Vector2 particleCircleCenter = new Vector2(0, 0);
    [SerializeField]
    private float particleCircleRadius = 10.0f;
    [SerializeField]
    private float particleCircleThickness = 15.0f;

    private void OnValidate()
    {
        init();
    }

    public override void init()
    {
        loadShader("InitializeParticles");
        createFrequencyObject();
        shader.SetVector("_Center", particleCircleCenter); 
        shader.SetFloat("_Radius", particleCircleRadius);
        shader.SetFloat("_Thickness", particleCircleThickness);
    }
    private void createFrequencyObject()
    {
        if (dispatchFrequencyObject== null)
        {
            dispatchFrequencyObject = ScriptableObject.CreateInstance<UpdatableGlobalVariable>();
            dispatchFrequencyObject.floatVar = 0;
        }
    }
}
