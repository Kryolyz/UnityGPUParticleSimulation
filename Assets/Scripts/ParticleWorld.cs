using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

public class ParticleWorld : MonoBehaviour
{
    [SerializeField] 
    private ShaderManager shaderManager;
    [SerializeField]
    private ComputeShader debugRender;

    // Particle Compute Buffer
    private ComputeBuffer _particles;
    private ComputeBuffer _particleList;
    private ComputeBuffer _particleListHead;

    // Particle Attributes
    [SerializeField]
    private float restitutionCoefficient = 0.2f;
    [SerializeField]
    private DispatchSizeScriptableObject particlesDimensionsDispatchSize;
    [SerializeField]
    private Vector3Int particlesDimensions = new Vector3Int();

    [SerializeField]
    private DispatchSizeScriptableObject textureDimensionsDispatchSize;
    [SerializeField]
    private Vector3Int textureDimensions = new Vector3Int();
    [SerializeField]
    private UpdatableGlobalVariable subStepsVariable;
    [SerializeField]
    private UpdatableGlobalVariable timeStepSubStepsVariable;
    [SerializeField]
    private UpdatableGlobalVariable spawnParticleCountVariable;

    UpdatableGlobalVariable restitutionVariable;
    [SerializeField]
    private int collisionGridScale = 2;

    [SerializeField]
    private MeshFilter quad;

    [SerializeField]
    private InputManager _inputManager;

    private void OnValidate()
    {
        textureDimensionsDispatchSize.dispatchSize = particlesDimensions;
        particlesDimensionsDispatchSize.dispatchSize = particlesDimensions;
        if (!restitutionVariable)
            restitutionVariable = ScriptableObject.CreateInstance<UpdatableGlobalVariable>();
        restitutionVariable.floatVar = restitutionCoefficient;
    }

    void Awake()
    {
        // set dimensions for shaders
        textureDimensionsDispatchSize.dispatchSize = particlesDimensions;
        particlesDimensionsDispatchSize.dispatchSize = particlesDimensions;

        // Initialize compute buffers
        _particles = new ComputeBuffer(particlesDimensions.x * particlesDimensions.y, 17 * sizeof(float));
        _particleList = new ComputeBuffer(particlesDimensions.x * particlesDimensions.y, sizeof(int));
        _particleListHead = new ComputeBuffer(textureDimensions.x * textureDimensions.y, sizeof(int));

        // Set global shader values
        shaderManager.globalBuffers.Add(new Pair<string, ComputeBuffer>("Particles", _particles));
        shaderManager.globalBuffers.Add(new Pair<string, ComputeBuffer>("ParticleList", _particleList));
        shaderManager.globalBuffers.Add(new Pair<string, ComputeBuffer>("ParticleListHead", _particleListHead));

        shaderManager.globalInts.Add(new Pair<string, int>("ParticleBufferWidth", particlesDimensions.x));
        shaderManager.globalInts.Add(new Pair<string, int>("TextureWidth", textureDimensions.x));
        shaderManager.globalInts.Add(new Pair<string, int>("GridSize", collisionGridScale));

        shaderManager.fixedUpdateGlobalFloats.Add(new Pair<string, UpdatableGlobalVariable>("dt", timeStepSubStepsVariable));
        shaderManager.fixedUpdateGlobalFloats.Add(new Pair<string, UpdatableGlobalVariable>("restitutionCoefficient", restitutionVariable));
        shaderManager.fixedUpdateGlobalInts.Add(new Pair<string, UpdatableGlobalVariable>("spawnedParticlesCount", spawnParticleCountVariable));

        _inputManager.OnLeftMouseDown += onLeftClick;
    }

    [ContextMenu("Render Debug")]
    public void renderDebug()
    {
        Awake();
        debugRender.Dispatch(0, textureDimensions.x / 8, textureDimensions.y / 8, 1);
        _particleList.Release();
        _particleListHead.Release();
        _particles.Release();
    }
    private void asyncGpuReadback_particles(AsyncGPUReadbackRequest request)
    {
        NativeArray<Particle> particles = request.GetData<Particle>();
        int counterEnd = 0;
        int counter0 = 0;
        for (int i = 0; i < particles.Length / 10; i++)
        {
            Particle particle = particles[i];
            bool isAtEnd = particle.p.x >= textureDimensions.x - 1 && particle.p.y < 1.0f;
            bool isAt0 = particle.p.x < 1.0f && particle.p.y < 1.0f;

            if (isAtEnd)
            {
                Debug.Log("Particle " + i + " Position: " + particle.p);
                counterEnd++;
            }

            if (isAt0)
            {
                Debug.Log("Particle " + i + " Position: " + particle.p);
                counter0++;
            }
        }
        if (counterEnd > 1)
            print("Counted End: " + counterEnd);
        if (counter0 > 1)
            print("Counted 0: " + counter0);
    }

    private void onLeftClick()
    {
        Debug.Log("Left Click");
        Debug.Log("Mouse Position" + _inputManager.mousePosition);
    }

    private void OnDisable()
    {
        _particleList?.Release();
        _particleListHead?.Release();
        _particles?.Release(); 
        _inputManager.OnLeftMouseDown -= onLeftClick;
    }
}
