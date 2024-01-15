using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

internal class DrawImageBlitRendererFeature : ScriptableRendererFeature
{
    public Shader m_Shader;
    public float m_Intensity;
    public Texture Sprite;
    Material m_Material;

    DrawImageBlitPass m_RenderPass = null;

    public override void AddRenderPasses(ScriptableRenderer renderer,
        ref RenderingData renderingData)
    {
        if (renderingData.cameraData.camera.tag == "Projector")
            renderer.EnqueuePass(m_RenderPass);
    }

    public override void SetupRenderPasses(ScriptableRenderer renderer,
        in RenderingData renderingData)
    {
        if (renderingData.cameraData.camera.tag == "Projector")
        {
            // Calling ConfigureInput with the ScriptableRenderPassInput.Color argument
            // ensures that the opaque texture is available to the Render Pass.
            m_RenderPass.ConfigureInput(ScriptableRenderPassInput.Color);
            m_RenderPass.SetTarget(renderer.cameraColorTargetHandle, Sprite);
        }
    }

    public override void Create()
    {
        m_Material = CoreUtils.CreateEngineMaterial(m_Shader);
        m_RenderPass = new DrawImageBlitPass(m_Material);
    }

    protected override void Dispose(bool disposing)
    {
        CoreUtils.Destroy(m_Material);
    }
}