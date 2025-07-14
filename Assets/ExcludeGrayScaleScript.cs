using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class ExcludeGrayScaleScript : ScriptableRendererFeature
{
    class ExcludePass : ScriptableRenderPass
    {
        private List<ShaderTagId> shaderTagIdList; // List of ShaderTagId
        private LayerMask excludeLayer;

        public ExcludePass(LayerMask layer)
        {
            excludeLayer = layer;
            // Initialize the ShaderTagId list with a valid tag, e.g., "UniversalForward"
            shaderTagIdList = new List<ShaderTagId> { new ShaderTagId("UniversalForward") }; // Or use another tag if needed
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (shaderTagIdList == null || shaderTagIdList.Count == 0)
            {
                Debug.LogWarning("ShaderTagId list is invalid. DrawingSettings is created with default pipeline ShaderTagId");
                // Fallback to default pipeline ShaderTagId
                CreateDrawingSettings(new ShaderTagId("UniversalPipeline"), ref renderingData, SortingCriteria.CommonOpaque);
            }
            else
            {
                // Create the drawing settings with the valid list of ShaderTagId
                var drawingSettings = CreateDrawingSettings(shaderTagIdList, ref renderingData, SortingCriteria.CommonOpaque);
                var filteringSettings = new FilteringSettings(RenderQueueRange.all, excludeLayer);
                context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filteringSettings);
            }
        }
    }

    public LayerMask excludeLayer;
    private ExcludePass pass;

    public override void Create()
    {
        pass = new ExcludePass(excludeLayer);
        pass.renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(pass);
    }
}
