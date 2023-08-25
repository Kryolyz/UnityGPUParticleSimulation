using UnityEngine;

public interface IShaderProgram
{
    string ShaderName { get; }
    /** @brief Shader program to manage */
    ComputeShader Shader { get; }

    /** @brief Dispatch shader with this size */
    Vector3Int DispatchSize { get; }

    /** @brief Lower is higher priority. This is the priority OUTSIDE the frequency loop.
     * Loop Pseudocode:
     * foreach (outerPriority in allOuterPrioritiesInAttachedShaderPrograms)
     * {
     *      foreach (frequencyInShaderListAtThisOuterPriority)
     *      {
     *          for (int b = 0; b < thisFrequency; ++b) // Calling underlying shader thisFrequency times
     *              for (int b = 0; b < maxInnerPriorityOverAllShadersWithThisFrequency; ++b) // Calling in the order of innerFrequency
     *                  shader.dispatch();
     *      }
     * }
     */
    int OuterDispatchPriority { get; }

    /** @brief How often this shader should be dispatched in FixedUpdate. 0 means it does not get called inside FixedUpdate. */
    float DispatchFrequency { get; }

    /** @brief Used by ShaderManager to correctly apply DispatchFrequencies between 0 and 1. Should simply be an int variable. */
    int DispatchCounter { get; set; }

    /** @brief Lower is higher priority. This is the priority INSIDE their frequency loop, meaning all shaders with the same frequency are
     * called in the order of this priority. If two shaders have different frequencies, the outer priority is the only thing affecting their order.*/
    int InnerDispatchPriority { get; }

    /** @brief Initialize everything specific to this shader in here. Globals should be set in the manager.
     */

    void dispatch();
    void init();
}
