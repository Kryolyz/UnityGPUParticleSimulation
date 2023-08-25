using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UpdateParticlesProgram : MonoBehaviour, IShaderProgram
{
    private string shaderName;
    public string ShaderName => shaderName;

    [SerializeField]
    private ComputeShader shader;

    [SerializeField, Tooltip("Allows changing the substeps in Editor. No other purpose.")]
    private int subSteps = 8;
    private void OnValidate()
    {
        init();
    }

    [SerializeField]
    private UpdatableGlobalVariable dispatchFrequencyObject;
    [SerializeField]
    private UpdatableGlobalVariable timeStepObject;

    public ComputeShader Shader => shader;

    [SerializeField]
    protected DispatchSizeScriptableObject dispatchSize;
    public Vector3Int DispatchSize => dispatchSize.dispatchSize;

    [SerializeField]
    private int outerDispatchPriority = 1;
    public int OuterDispatchPriority => outerDispatchPriority;

    [SerializeField]
    private int innerDispatchPriority = 0;
    public int InnerDispatchPriority => innerDispatchPriority;

    public float DispatchFrequency => dispatchFrequencyObject.floatVar;

    private int dispatchCounter = 0;
    [HideInInspector]
    public int DispatchCounter { get => dispatchCounter; set => dispatchCounter = value; }

    private void loadShader()
    {
        if (shader == null)
            shader = Resources.Load<ComputeShader>("Shaders/UpdateParticles");
    }

    public void init()
    {
        shaderName = GetType().Name;
        loadShader();
        dispatchFrequencyObject.floatVar = subSteps;
        timeStepObject.floatVar = Time.fixedDeltaTime / subSteps;
    }

    public void dispatch()
    {
        shader.Dispatch(0, dispatchSize.dispatchSize.x / 8, dispatchSize.dispatchSize.y / 8, dispatchSize.dispatchSize.z);
    }
}
