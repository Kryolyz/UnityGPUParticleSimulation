using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

public class ParticleWorld : MonoBehaviour
{
    [SerializeField] 
    private ShaderManager shaderManager;
    //[SerializeField]
    //private ComputeShader sprayParticles;
    //[SerializeField]
    //private ComputeShader updateParticles;
    //[SerializeField]
    //private ComputeShader resetGrid;
    //[SerializeField]
    //private ComputeShader computeCollisions;
    //[SerializeField]
    //private ComputeShader applyGravity;
    //[SerializeField]
    //private ComputeShader applyDrag;
    //[SerializeField]
    //private ComputeShader renderParticles;
    [SerializeField]
    private ComputeShader debugRender;

    //private RenderTexture _renderTexture;

    //[SerializeField]
    //private float sprayThickness = 1.0f;
    //[SerializeField]
    //private Vector2 sprayInitialPosition = new Vector2(50, 200);
    //[SerializeField]
    //private Vector2 sprayInitialVelocity = new Vector2(10, 0);
    //[SerializeField]
    //private int spawnOnFrame = 5;
    //private int spawnedParticlesCount = 0;
    //[SerializeField]
    //private Material material;

    // Particle Compute Buffer
    private ComputeBuffer _particles;
    private ComputeBuffer _particleList;
    private ComputeBuffer _particleListHead;

    // Particle Attributes
    //[SerializeField]
    //private float drag = .1f;
    //[SerializeField]
    //private Vector2 gravity = new Vector2(0, -9.81f);
    [SerializeField]
    private float restitutionCoefficient = 0.2f;

    //[SerializeField]
    //private int subSteps = 2;

    //[SerializeField]
    //private Vector2Int particlesDimensions = new Vector2Int(256, 256);
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
    //restitutionVariable.floatVar = restitutionCoefficient;
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

        // Create a new texture for rendering.
        //_renderTexture = new RenderTexture(textureDimensions.x, textureDimensions.y, 0, RenderTextureFormat.ARGBFloat);
        //_renderTexture.enableRandomWrite = true;
        //_renderTexture.filterMode= FilterMode.Point;
        //_renderTexture.wrapMode = TextureWrapMode.Clamp;
        //_renderTexture.Create();

        // Set the texture as the output of the compute shader.
        //material.mainTexture = _renderTexture;
        //quad.GetComponent<MeshRenderer>().material = material;

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
        //applyGravity.SetVector("g", gravity);

        //renderParticles.SetTexture(0, "Result", _renderTexture);

        //debugRender.SetTexture(0, "Result", _renderTexture);

        _inputManager.OnLeftMouseDown += onLeftClick;

        //textureDimensions.x *= 2;

        //initializeParticles.Dispatch(0, particlesDimensions.x / 8, particlesDimensions.y / 8, 1);
        //resetGrid.Dispatch(0, textureDimensions.x / 8, textureDimensions.y / 8, 1);
        //updateParticles.Dispatch(0, particlesDimensions.x / 8, particlesDimensions.y / 8, 1);
        //computeCollisions.DisableKeyword("UPDATE_VELOCITY");
        //computeCollisions.Dispatch(0, particlesDimensions.x / 8, particlesDimensions.y / 8, 1);
        //AsyncGPUReadback.Request(_particleList, asyncGpuReadback_particlelist);
        //AsyncGPUReadback.Request(_particleListHead, asyncGpuReadback_particlelistHead);
    }

    [ContextMenu("Render Debug")]
    public void renderDebug()
    {
        Awake();
        //clearRenderTextures();
        debugRender.Dispatch(0, textureDimensions.x / 8, textureDimensions.y / 8, 1);
        _particleList.Release();
        _particleListHead.Release();
        _particles.Release();
    }

    //int physicsFrame = 0;
    private void FixedUpdate()
    {
        //clearRenderTextures();

        //if (physicsFrame % spawnOnFrame == 0)
        //{
            //sprayParticles.SetFloat("thickness", sprayThickness);
            //sprayParticles.SetVector("initialVelocity", sprayInitialVelocity);
            //sprayParticles.SetVector("initialPosition", sprayInitialPosition);
            //sprayParticles.SetFloat("dt", Time.fixedDeltaTime / subSteps);
            //sprayParticles.SetInt("spawnedParticlesCount", spawnedParticlesCount);
            //sprayParticles.Dispatch(0, particlesDimensions.x / 8, particlesDimensions.y / 8, 1);
            //spawnedParticlesCount += Mathf.CeilToInt(sprayThickness);
        //}

        //for (int i = 0; i < subSteps; i++)
        //{
            //updateParticles.SetFloat("restitutionCoefficient", restitutionCoefficient);
            //updateParticles.SetFloat("dt", Time.fixedDeltaTime / subSteps);
            //computeCollisions.SetFloat("restitutionCoefficient", restitutionCoefficient);
            //applyGravity.SetVector("g", gravity);
            //applyDrag.SetFloat("drag", drag);

            //resetGrid.Dispatch(0, textureDimensions.x / 8, textureDimensions.y / 8, 1);
            //updateParticles.Dispatch(0, particlesDimensions.x / 8, particlesDimensions.y / 8, 1);

            //computeCollisions.DisableKeyword("UPDATE_VELOCITY");
            //computeCollisions.Dispatch(0, particlesDimensions.x / 8, particlesDimensions.y / 8, 1);

            //computeCollisions.EnableKeyword("UPDATE_VELOCITY");
            //computeCollisions.Dispatch(0, particlesDimensions.x / 8, particlesDimensions.y / 8, 1);

            //applyGravity.Dispatch(0, particlesDimensions.x / 8, particlesDimensions.y / 8, 1);
            //applyDrag.Dispatch(0, particlesDimensions.x / 8, particlesDimensions.y / 8, 1);
        //}

        //renderParticles.Dispatch(0, particlesDimensions.x / 8, particlesDimensions.y / 8, 1);

        //if (physicsFrame % 25 == 0)
        //{
        //AsyncGPUReadback.Request(_particleList, asyncGpuReadback_particlelist);
        //AsyncGPUReadback.Request(_particleListHead, asyncGpuReadback_particlelistHead);
        //AsyncGPUReadback.Request(_particles, asyncGpuReadback_particles);
        //}
        //Debug.Log("goddamn bs");
        //physicsFrame++;
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

    //[ContextMenu("Print Particle Linked List at 0")]
    //public void printParticleLinkedListAt0()
    //{
    //    AsyncGPUReadback.Request(_particles, asyncGpuReadback_particleLinkedList0);
    //}

    private void asyncGpuReadback_particleLinkedList0(AsyncGPUReadbackRequest obj)
    {
        Debug.Log("After callback: " + (Time.realtimeSinceStartup - time));
        NativeArray<Particle> particles = obj.GetData<Particle>();

        int[] particleListheads = new int[_particleListHead.count];
        _particleListHead.GetData(particleListheads);
        int[] particleListEntries = new int[_particleList.count];
        _particleList.GetData(particleListEntries);

        int nextLink = particleListheads[0];

        if (nextLink == -1)
            return;

        while (nextLink >= 0)
        {
            Debug.Log(nextLink);
            Particle particle = particles[nextLink];
            Debug.Log("Particle Position: " + particle.p);

            nextLink = particleListEntries[nextLink];
        }
    }

    double time;
    private void asyncGpuReadback_particlelistHead(AsyncGPUReadbackRequest obj)
    {
        Debug.Log("After callback: " + (Time.realtimeSinceStartup - time));
        int[] data = new int[_particleListHead.count];
        _particleListHead.GetData(data);

        for (int i = 0; i < data.Length / 10; i++) 
        {
            int index = data[i];
            if (index >= 0) 
            {
                Debug.Log("ParticleListHead:");
                int2 id;
                id.y = i / (512 / collisionGridScale);
                id.x = i % (512 / collisionGridScale);
                Debug.Log("ID: " + id);

                int2 pos;
                pos.y = index / 256;
                pos.x = index % 256;
                Debug.Log("Index: " + pos);
            }
        }
    }

    private void asyncGpuReadback_particlelist(AsyncGPUReadbackRequest obj)
    {
        int[] data = new int[_particleList.count];
        _particleList.GetData(data);

        for (int i = 0; i < data.Length; i++)
        {
            int index = data[i];
            if (index >= 0)
            {
                Debug.Log("ParticleList: ");
                int2 id;
                id.y = i / 256;
                id.x = i % 256;
                Debug.Log("ID: " + id);

                int2 pos;
                pos.y = index / 256;
                pos.x = index % 256;
                //pos.y = index / (512 / collisionGridScale);
                //pos.x = index % (512 / collisionGridScale);
                Debug.Log("Index: " + pos);
            }
        }
    }

    private void onLeftClick()
    {
        Debug.Log("Left Click");
        Debug.Log("Mouse Position" + _inputManager.mousePosition);
    }

    private int2 idFromIndex(int index) 
    {
        int2 id;
        id.y = index / 256;
        id.x = index % 256;
        return id;
    }

    private void OnDisable()
    {
        _particleList?.Release();
        _particleListHead?.Release();
        _particles?.Release(); 
        _inputManager.OnLeftMouseDown -= onLeftClick;
    }
}
