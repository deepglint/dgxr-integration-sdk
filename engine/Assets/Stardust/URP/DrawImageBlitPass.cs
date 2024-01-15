using System.Collections.Generic;
using DGXR;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

internal class DrawImageBlitPass : ScriptableRenderPass
{
    ProfilingSampler m_ProfilingSampler = new ProfilingSampler("QuadWarp");
    Material m_Material;
    RTHandle m_CameraColorTarget;
    Texture m_Intensity;
    private Dictionary<Camera,QuadWarp> _quadWarps = new Dictionary<Camera,QuadWarp>();

    public DrawImageBlitPass(Material material)
    {
        m_Material = material;
        renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    }

    public void SetTarget(RTHandle colorHandle, Texture intensity)
    {
        m_CameraColorTarget = colorHandle;
        m_Intensity = intensity;
    }

    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        ConfigureTarget(m_CameraColorTarget);
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        var cameraData = renderingData.cameraData;
        if (renderingData.cameraData.camera.tag != "Projector")
        // if (cameraData.camera.cameraType != CameraType.Game)
            return;

        if (m_Material == null)
            return;

        CommandBuffer cmd = CommandBufferPool.Get();
        using (new ProfilingScope(cmd, m_ProfilingSampler))
        {
            _quadWarps.TryGetValue(cameraData.camera, out QuadWarp quadWarp);
            if (quadWarp)
            {
                m_Material.SetTexture("_BaseMap",quadWarp._tex[0]);
                m_Material.SetMatrix("_Homography",quadWarp.GetMatrix4x4());
                Blitter.BlitCameraTexture(cmd, m_CameraColorTarget, m_CameraColorTarget, m_Material, 0);
            }
            else
            {
                QuadWarp newQuadWarp = cameraData.camera.GetComponent<QuadWarp>();
                if (newQuadWarp)
                {
                    _quadWarps.Add(cameraData.camera,newQuadWarp);
                    m_Material.SetTexture("_BaseMap",newQuadWarp._tex[0]);
                    m_Material.SetMatrix("_Homography",newQuadWarp.GetMatrix4x4());
                    // m_Material.mainTexture = m_Intensity;
                    Blitter.BlitCameraTexture(cmd, m_CameraColorTarget, m_CameraColorTarget, m_Material, 0);
                }
            }
        }
        context.ExecuteCommandBuffer(cmd);
        cmd.Clear();

        CommandBufferPool.Release(cmd);
    }
}