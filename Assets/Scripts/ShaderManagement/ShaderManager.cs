using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShaderManager : MonoBehaviour
{
    //private List<IShaderProgram> _shaders = new List<IShaderProgram>();

    /** 
     * @brief List of list of shader programs. Each sublist has an associated frequency to control 
     * the call frequency. Uses shaders attached to this gameobject and sorts them according to outer and inner dispatch priority.
     */
    [HideInInspector]
    private List<Pair<float, List<IShaderProgram>>> _shaders = new List<Pair<float, List<IShaderProgram>>>();

    [HideInInspector]
    public List<Pair<string, int>> globalInts = new List<Pair<string, int>>();
    [HideInInspector]
    public List<Pair<string, float>> globalFloat = new List<Pair<string, float>>();
    [HideInInspector]
    public List<Pair<string, Vector2>> globalVector2s = new List<Pair<string, Vector2>>();
    [HideInInspector]
    public List<Pair<string, Vector3>> globalVector3s = new List<Pair<string, Vector3>>();
    [HideInInspector]
    public List<Pair<string, Vector4>> globalVector4 = new List<Pair<string, Vector4>>();
    [HideInInspector]
    public List<Pair<string, ComputeBuffer>> globalBuffers = new List<Pair<string, ComputeBuffer>>();

    [HideInInspector]
    public List<Pair<string, UpdatableGlobalVariable>> fixedUpdateGlobalInts = new List<Pair<string, UpdatableGlobalVariable>>();
    [HideInInspector]
    public List<Pair<string, UpdatableGlobalVariable>> fixedUpdateGlobalFloats = new List<Pair<string, UpdatableGlobalVariable>>();
    [HideInInspector]
    public List<Pair<string, UpdatableGlobalVariable>> fixedUpdateGlobalVector2 = new List<Pair<string, UpdatableGlobalVariable>>();

    private void Start()
    {
        sortShaderPrograms();
        setGlobals();
        updateGlobalVariables();
        initializeShaders();
    }

    private void OnEnable()
    {
        executeOneTimeShaders();
    }

    private void executeOneTimeShaders()
    {
        foreach (var shaderList in _shaders)
        {
            float frequency = shaderList.Item1;
            if (frequency == 0)
            {
                foreach (var shaderProgram in shaderList.Item2)
                {
                    shaderProgram.dispatch();
                }
            }
        }
    }

    [ContextMenu("Sort Shaders")]
    private void sortShaderPrograms()
    {
        _shaders = new List<Pair<float, List<IShaderProgram>>>();
        List<IShaderProgram> allShaders = GetComponents<IShaderProgram>().ToList();
        Debug.Log("Total Number shaders: " + allShaders.Count);   
        if (allShaders == null)
        {
            throw new Exception("No ShaderPrograms attached to ShaderManager");
        }

        SortedSet<int> allOuterPriorities = new SortedSet<int>();
        foreach (var shader in allShaders)
        {
            allOuterPriorities.Add(shader.OuterDispatchPriority);
        }

        //int index = 0;
        foreach (int outerPriority in allOuterPriorities)
        {
            // get all shader at this outer priority
            var shadersAtThisOuterPriority = allShaders.Where((a) => a.OuterDispatchPriority == outerPriority);

            // get set of different frequencies at this outer priority
            SortedSet<float> frequenciesAtThisOuterPriority = new SortedSet<float>();
            foreach (var shader in shadersAtThisOuterPriority)
            {
                frequenciesAtThisOuterPriority.Add(shader.DispatchFrequency);
            }

            foreach (var frequency in frequenciesAtThisOuterPriority)
            {
                var thisOuterPriorityThisFrequency = shadersAtThisOuterPriority.Where((a) => ((int)(a.DispatchFrequency * 1000)) == ((int)(frequency * 1000)));
                var sortedByInnerPriority = thisOuterPriorityThisFrequency.OrderBy((a) => a.InnerDispatchPriority).ToList();
                _shaders.Add(new Pair<float, List<IShaderProgram>>(frequency, sortedByInnerPriority));
            }

        }

        Debug.Log("Shader Lists: " + _shaders.Count);
        foreach (var shaderList in _shaders)
        {
            Debug.Log("Outer Priority: " + shaderList.Item2[0].OuterDispatchPriority);
            Debug.Log("Frequency: " + shaderList.Item1);
            string shaderChain = "";
            foreach (var shaderProgram in shaderList.Item2)
            {
                shaderChain += " -> " + shaderProgram.ShaderName;
            }
            Debug.Log(shaderChain);
        }
    }

    private void updateGlobalVariables()
    {
        if (fixedUpdateGlobalInts.Count > 0)
        {
            foreach (var variable in fixedUpdateGlobalInts)
            {
                foreach (var shaderList in _shaders)
                {
                    foreach (var shader in shaderList.Item2)
                    {
                        shader.Shader.SetInt(variable.Item1, variable.Item2.intVar);
                    }
                }
            }
        }

        if (fixedUpdateGlobalFloats.Count > 0)
        {
            foreach (var variable in fixedUpdateGlobalFloats)
            {
                foreach (var shaderList in _shaders)
                {
                    foreach (var shader in shaderList.Item2)
                    {
                        shader.Shader.SetFloat(variable.Item1, variable.Item2.floatVar);
                    }
                }
            }
        }

        if (fixedUpdateGlobalVector2.Count > 0)
        {
            foreach (var variable in fixedUpdateGlobalVector2)
            {
                foreach (var shaderList in _shaders)
                {
                    foreach (var shader in shaderList.Item2)
                    {
                        shader.Shader.SetVector(variable.Item1, variable.Item2.vector2Var);
                    }
                }
            }
        }
    }

    private void FixedUpdate()
    {
        updateGlobalVariables();
        executeShaders();
    }

    void executeShaders()
    {
        foreach (var shaderList in _shaders)
        {
            float frequency = shaderList.Item1;
            if (shaderList.Item2.Count == 1)
                frequency = shaderList.Item2[0].DispatchFrequency;
            if (frequency > 0.0001f && frequency < 1) // means should only be called every (1 / frequency) frames
            {
                foreach (var shaderProgram in shaderList.Item2)
                {
                    if (shaderProgram.DispatchCounter >= (1 / frequency) - 1)
                    {
                        shaderProgram.dispatch();
                        shaderProgram.DispatchCounter = 0;
                    }
                    else
                        shaderProgram.DispatchCounter += 1;
                }
            }
            else
            {
                for (int i = 0; i < frequency; ++i)
                {
                    foreach (var shaderProgram in shaderList.Item2)
                    {
                        shaderProgram.dispatch();
                    }
                }
            }
        }
    }

    void initializeShaders()
    {
        foreach (var shaderList in _shaders)
        {
            foreach (var shader in shaderList.Item2)
                shader.init();
        }
    }

    void setGlobals()
    {
        foreach (var shaderList in _shaders)
        {
            foreach (var shaderProgram in shaderList.Item2)
            {
                if (globalInts.Count > 0)
                {
                    foreach (var global in globalInts)
                        shaderProgram.Shader.SetInt(global.Item1, global.Item2);
                }

                if (globalFloat.Count > 0)
                {
                    foreach (var global in globalFloat)
                        shaderProgram.Shader.SetFloat(global.Item1, global.Item2);
                }

                if (globalVector2s.Count > 0)
                {
                    foreach (var global in globalVector2s)
                        shaderProgram.Shader.SetVector(global.Item1, global.Item2);
                }

                if (globalVector3s.Count > 0)
                {
                    foreach (var global in globalVector3s)
                        shaderProgram.Shader.SetVector(global.Item1, global.Item2);
                }

                if (globalVector4.Count > 0)
                {
                    foreach (var global in globalVector4)
                        shaderProgram.Shader.SetVector(global.Item1, global.Item2);
                }

                if (globalBuffers.Count > 0)
                {
                    foreach (var global in globalBuffers)
                        shaderProgram.Shader.SetBuffer(0, global.Item1, global.Item2);
                }
            }
        }
    }
}
