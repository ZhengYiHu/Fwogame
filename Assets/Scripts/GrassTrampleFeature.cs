using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GrassTramplePassFeature : ScriptableRendererFeature
{
    class GrassPass : ScriptableRenderPass
    {
        Vector4[] tramplePositions;
        int numTramplePositions;

        public int NumTramplePositions
        {
            set { numTramplePositions = value; }
            get { return numTramplePositions; }
        }

        public GrassPass(Vector4[] tramplePositions)
        {
            this.tramplePositions = tramplePositions;
        }
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get("GrassTramplePassFeature");

            cmd.SetGlobalVectorArray("_GrassTramplePositions", tramplePositions);
            cmd.SetGlobalInt("_NumGrassTramplePositions", numTramplePositions);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }

    [SerializeField] int maxTrackedTransforms;

    GrassPass pass;
    List<Transform> trackingTransforms;
    Vector4[] tramplePositions;

    public void AddTrackedTransform(Transform transform)
    {
        trackingTransforms.Add(transform);
    }

    public void RemoveTrackedTransform(Transform transform)
    {
        trackingTransforms.Remove(transform);
    }

    public override void Create()
    {
        trackingTransforms = new List<Transform>();
        trackingTransforms.AddRange(FindObjectsByType<GrassTrampler>(FindObjectsSortMode.None).Select(o => o.transform));
        tramplePositions = new Vector4[maxTrackedTransforms];
        pass = new GrassPass(tramplePositions);

        // Configures where the render pass should be injected.
        pass.renderPassEvent = RenderPassEvent.BeforeRendering;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
#if UNITY_EDITOR
        trackingTransforms.RemoveAll(t => t.transform == null);
#endif
        //Clear the array
        for (int i = 0; i < tramplePositions.Length; i++)
        {
            tramplePositions[i] = Vector4.zero;
        }
        //Populate with positions of the tracked transforms
        int count = Mathf.Min(maxTrackedTransforms, trackingTransforms.Count);
        for (int i = 0; i < count; i++)
        {
            Vector3 trackedPosition = trackingTransforms[i].position;
            tramplePositions[i] = new Vector4(trackedPosition.x, trackedPosition.y, trackedPosition.z, 1);
        }
        pass.NumTramplePositions = count; 
        renderer.EnqueuePass(pass);
    }
}


