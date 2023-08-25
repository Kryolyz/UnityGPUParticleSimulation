using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SprayParticleProgram : StandardShaderProgram
{
    [SerializeField, Tooltip("For controlling Spawn Frequency from Editor"), Range(0, 1)]
    private float spawnFrequency = 0.1f;

    [SerializeField]
    private float sprayThickness = 1;
    [SerializeField]
    private Vector2 sprayInitialVelocity = new Vector2(10,10);
    [SerializeField]
    private Vector2 sprayInitialPosition = new Vector2(10,10);

    [SerializeField]
    private UpdatableGlobalVariable spawnedParticleCountObject;

    private ShaderManager manager;

    private void OnValidate()
    {
        dispatchFrequencyObject.floatVar = spawnFrequency;
        loadShader("SprayParticles");
    }

    public override void init()
    {
        loadShader("SprayParticles");
        dispatchFrequencyObject = ScriptableObject.CreateInstance<UpdatableGlobalVariable>();
        dispatchFrequencyObject.floatVar = spawnFrequency;
        shader.SetFloat("thickness", sprayThickness);
        shader.SetVector("initialVelocity", sprayInitialVelocity);
        shader.SetVector("initialPosition", sprayInitialPosition);
    }

    private void FixedUpdate()
    {
        dispatchFrequencyObject.floatVar = spawnFrequency;
        //shader.SetFloat("thickness", sprayThickness);
        //shader.SetVector("initialVelocity", sprayInitialVelocity);
        //shader.SetVector("initialPosition", sprayInitialPosition);
    }

    public override void dispatch()
    {
        shader.SetFloat("thickness", sprayThickness);
        shader.SetVector("initialVelocity", sprayInitialVelocity);
        shader.SetVector("initialPosition", sprayInitialPosition);
        shader.SetInt("spawnedParticlesCount", spawnedParticleCountObject.intVar);
        base.dispatch();
        spawnedParticleCountObject.intVar += Mathf.CeilToInt(sprayThickness);
    }

    private void OnDestroy()
    {
        spawnedParticleCountObject.intVar = 0;
    }
}
