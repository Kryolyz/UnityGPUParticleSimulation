using UnityEngine;

[RequireComponent(typeof(Shader))]
public abstract class StandardShaderProgram : MonoBehaviour, IShaderProgram
{
    protected string shaderName;
    virtual public string ShaderName => shaderName;

    [SerializeField]
    protected ComputeShader shader;
    virtual public ComputeShader Shader => shader;

    [SerializeField]
    protected DispatchSizeScriptableObject dispatchSize;
    virtual public Vector3Int DispatchSize => dispatchSize.dispatchSize;

    [SerializeField]
    protected UpdatableGlobalVariable dispatchFrequencyObject;
    virtual public float DispatchFrequency => dispatchFrequencyObject.floatVar;

    [SerializeField]
    protected int outerDispatchPriority = 0;
    virtual public int OuterDispatchPriority => outerDispatchPriority;

    [SerializeField]
    protected int innerDispatchPriority = 0;
    virtual public int InnerDispatchPriority => innerDispatchPriority;

    private int dispatchCounter = 0;
    [HideInInspector]
    virtual public int DispatchCounter { get => dispatchCounter; set => dispatchCounter = value; }

    public void loadShader(string shaderName)
    {
        this.shaderName = shaderName;
        if (shader == null)
            shader = Resources.Load<ComputeShader>("Shaders/"+ shaderName);
    }

    public virtual void dispatch()
    {
        shader.Dispatch(0, DispatchSize.x / 8, DispatchSize.y / 8, DispatchSize.z);
    }

    abstract public void init();
}
