using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.Rendering;

public class ParticleWorld : MonoBehaviour
{
    public ComputeShader initializeParticles;
    public ComputeShader applyGravity;
    public ComputeShader applyDrag;
    public ComputeShader updateParticles;
    public ComputeShader blitParticles;
    public ComputeShader renderParticles;
    public ComputeShader computeCollisions;

    private RenderTexture _particles;
    private RenderTexture _particlesBuffer;
    private RenderTexture _particleStates;
    private RenderTexture _particleStatesBuffer;

    private RenderTexture _renderTexture;
    private RenderTexture _renderedParticles;
    private RenderTexture _renderedStates;

    public Vector2 particleCircleCenter = new Vector2(0, 0);
    public float particleCircleRadius = 10.0f;
    public float particleCircleThickness = 15.0f;
    public Material material;

    // Particle Compute Buffer
    ComputeBuffer _particleList;

    // Particle Attributes
    [SerializeField]
    private float drag = .1f;
    [SerializeField]
    private float restitutionConstant = 0.2f;

    public Vector2Int particlesDimensions = new Vector2Int(256, 256);
    public Vector2Int textureDimensions = new Vector2Int(512, 512);

    //private ComputeBuffer buffer;
    //private bool usePrimaryBuffer = true;

    public MeshFilter quad;

    void Start()
    {
        _particleList = new ComputeBuffer(256*256, 15 * sizeof(float));

        // Create a new texture with the specified width and height.
        _particles = new RenderTexture(particlesDimensions.x, particlesDimensions.y, 0, RenderTextureFormat.ARGBFloat);
        _particles.enableRandomWrite = true;
        _particles.Create();
        _particlesBuffer = new RenderTexture(particlesDimensions.x, particlesDimensions.y, 0, RenderTextureFormat.ARGBFloat);
        _particlesBuffer.enableRandomWrite = true;
        _particlesBuffer.Create();

        _particleStates = new RenderTexture(particlesDimensions.x, particlesDimensions.y, 0, RenderTextureFormat.ARGBFloat);
        _particleStates.enableRandomWrite = true;
        _particleStates.Create();
        _particleStatesBuffer = new RenderTexture(particlesDimensions.x, particlesDimensions.y, 0, RenderTextureFormat.ARGBFloat);
        _particleStatesBuffer.enableRandomWrite = true;
        _particleStatesBuffer.Create();

        // Create a new texture for rendering.
        _renderTexture = new RenderTexture(textureDimensions.x, textureDimensions.y, 0, RenderTextureFormat.ARGBFloat);
        _renderTexture.enableRandomWrite = true;
        _renderTexture.filterMode= FilterMode.Point;
        _renderTexture.Create();
        _renderedParticles = new RenderTexture(textureDimensions.x, textureDimensions.y, 0, RenderTextureFormat.ARGBFloat);
        _renderedParticles.enableRandomWrite = true;
        _renderedParticles.filterMode = FilterMode.Point;
        _renderedParticles.Create();
        _renderedStates = new RenderTexture(textureDimensions.x, textureDimensions.y, 0, RenderTextureFormat.ARGBFloat);
        _renderedStates.enableRandomWrite = true;
        _renderedStates.filterMode = FilterMode.Point;
        _renderedStates.Create();

        // Set the texture as the output of the compute shader.
        material.mainTexture = _renderTexture;
        quad.GetComponent<MeshRenderer>().material = material;

        initializeParticles.SetTexture(0, "Particles", _particles);
        initializeParticles.SetTexture(0, "ParticleStates", _particleStates);
        initializeParticles.SetVector("_Center", particleCircleCenter);
        initializeParticles.SetFloat("_Radius", particleCircleRadius);
        initializeParticles.SetFloat("_Thickness", particleCircleThickness);

        applyDrag.SetTexture(0, "Particles", _particles);
        applyDrag.SetTexture(0, "ParticleStates", _particleStates);
        applyDrag.SetFloat("drag", drag);

        updateParticles.SetTexture(0, "Particles", _particles);
        updateParticles.SetTexture(0, "ParticleStates", _particleStates);
        updateParticles.SetTexture(0, "UpdatedParticles", _particlesBuffer);
        updateParticles.SetTexture(0, "UpdatedParticleStates", _particleStatesBuffer);
        updateParticles.SetFloat("dt", Time.fixedDeltaTime);

        blitParticles.SetTexture(0, "Particles", _particles);
        blitParticles.SetTexture(0, "ParticleStates", _particleStates);
        blitParticles.SetTexture(0, "UpdatedParticles", _particlesBuffer);
        blitParticles.SetTexture(0, "UpdatedParticleStates", _particleStatesBuffer);

        //float[] data = new float[1];
        //buffer = new ComputeBuffer(1, sizeof(float));
        //buffer.SetData(data);
        //renderParticles.SetBuffer(0, "WaitBuffer", buffer);
        renderParticles.SetTexture(0, "Particles", _particles);
        renderParticles.SetTexture(0, "ParticleStates", _particleStates);
        renderParticles.SetTexture(0, "Result", _renderTexture);
        renderParticles.SetTexture(0, "RenderedStates", _renderedStates);
        renderParticles.SetTexture(0, "RenderedParticles", _renderedParticles);

        computeCollisions.SetTexture(0, "Particles", _particles);
        computeCollisions.SetTexture(0, "ParticleStates", _particleStates);
        computeCollisions.SetTexture(0, "RenderedStates", _renderedStates);
        computeCollisions.SetTexture(0, "RenderedParticles", _renderedParticles);
        computeCollisions.SetFloat("restitutionConstant", restitutionConstant);

        initializeParticles.Dispatch(0, _particles.width / 8, _particles.height / 8, 1);
    }

    private void FixedUpdate()
    {
        clearRenderTextures();
        applyDrag.Dispatch(0, _particles.width / 8, _particles.height / 8, 1);
        computeCollisions.Dispatch(0, _particles.width / 8, _particles.height / 8, 1);
        //updateParticles.Dispatch(0, _particles.width / 8, _particles.height / 8, 1);
        //blitParticles.Dispatch(0, _particles.width / 8, _particles.height / 8, 1);
        //renderParticles.Dispatch(0, _particles.width / 8, _particles.height / 8, 1);
        //computeCollisions.SetBool("updateVelocity", false);
        //computeCollisions.Dispatch(0, _particles.width / 8, _particles.height / 8, 1);
        //computeCollisions.Dispatch(0, _particles.width / 8, _particles.height / 8, 1);
        //computeCollisions.Dispatch(0, _particles.width / 8, _particles.height / 8, 1);
        //computeCollisions.SetBool("updateVelocity", true);
        //computeCollisions.Dispatch(0, _particles.width / 8, _particles.height / 8, 1);

        int fsize = sizeof(float);
        int isize = sizeof(int);
        print(10 * fsize + 3 * fsize + 2 * isize);
        print(fsize * 15);
    }

    private void clearRenderTextures()
    {
        RenderTexture.active = _renderTexture;
        GL.Clear(true, true, Color.clear);
        RenderTexture.active = _renderedStates;
        GL.Clear(true, true, Color.clear);
        RenderTexture.active = _renderedParticles;
        GL.Clear(true, true, Color.clear);
    }

    private void Update()
    {

    }
}
