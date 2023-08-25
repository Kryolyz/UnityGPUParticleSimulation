using UnityEngine;

public class RenderParticlesProgram : StandardShaderProgram
{
    private RenderTexture _renderTexture;
    [SerializeField]
    private MeshRenderer _renderQuad;
    [SerializeField]
    private Material _material;

    [SerializeField]
    private DispatchSizeScriptableObject _textureSize;

    private void OnValidate()
    {
        init();
    }

    public override void init()
    {
        base.loadShader("RenderParticles");
        dispatchFrequencyObject = ScriptableObject.CreateInstance<UpdatableGlobalVariable>();
        dispatchFrequencyObject.floatVar = 1;
        createRenderTexture();
        shader.SetTexture(0, "Result", _renderTexture);
        _material.mainTexture= _renderTexture; 
        _renderQuad.material = _material;
    }

    private void createRenderTexture()
    {
        _renderTexture = new RenderTexture(_textureSize.dispatchSize.x, _textureSize.dispatchSize.y, 0, RenderTextureFormat.ARGBFloat);    
        _renderTexture.enableRandomWrite = true;
        _renderTexture.filterMode = FilterMode.Point;
        _renderTexture.wrapMode = TextureWrapMode.Clamp;
        _renderTexture.Create();
    }

    private void FixedUpdate()
    {
        clearRenderTextures();
        shader.Dispatch(0, dispatchSize.dispatchSize.x, dispatchSize.dispatchSize.y, dispatchSize.dispatchSize.z);
    }

    private void clearRenderTextures()
    {
        RenderTexture.active = _renderTexture;
        GL.Clear(true, true, Color.clear);
    }

    public override void dispatch()
    {
        clearRenderTextures();
        shader.Dispatch(0, DispatchSize.x, DispatchSize.y, DispatchSize.z);
    }
}
