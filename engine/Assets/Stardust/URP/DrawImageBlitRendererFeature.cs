using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

namespace Stardust.URP
{
    internal class DrawImageBlitRendererFeature : ScriptableRendererFeature
    {
        [FormerlySerializedAs("m_Shader")] public Shader mShader;
        public string cameraTag = "Projector";
        Material _mMaterial;

        DrawImageBlitPass _mRenderPass;

        public override void AddRenderPasses(ScriptableRenderer renderer,
            ref RenderingData renderingData)
        {
            if (renderingData.cameraData.camera.CompareTag(cameraTag))
                renderer.EnqueuePass(_mRenderPass);
        }

        public override void SetupRenderPasses(ScriptableRenderer renderer,
            in RenderingData renderingData)
        {
            if (renderingData.cameraData.camera.CompareTag(cameraTag))
            {
                _mRenderPass.ConfigureInput(ScriptableRenderPassInput.Color);
                _mRenderPass.SetTarget(renderer.cameraColorTargetHandle, cameraTag);
            }
        }

        public override void Create()
        {
            _mMaterial = CoreUtils.CreateEngineMaterial(mShader);
            _mRenderPass = new DrawImageBlitPass(_mMaterial);
        }

        protected override void Dispose(bool disposing)
        {
            CoreUtils.Destroy(_mMaterial);
        }
    }
}