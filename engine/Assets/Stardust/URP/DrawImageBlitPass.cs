using System.Collections.Generic;
using Stardust.Scripts;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Stardust.URP
{
    internal class DrawImageBlitPass : ScriptableRenderPass
    {
        ProfilingSampler _mProfilingSampler = new ProfilingSampler("QuadWarp");
        Material _mMaterial;
        RTHandle _mCameraColorTarget;
        string _cameraTag;
    
        private Dictionary<Camera,QuadWarp> _quadWarps = new Dictionary<Camera,QuadWarp>();
        private static readonly int BaseMap = Shader.PropertyToID("_BaseMap");
        private static readonly int Homography = Shader.PropertyToID("_Homography");

        public DrawImageBlitPass(Material material)
        {
            _mMaterial = material;
            renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        }

        public void SetTarget(RTHandle colorHandle, string cameraTag)
        {
            _mCameraColorTarget = colorHandle;
            this._cameraTag = cameraTag;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            ConfigureTarget(_mCameraColorTarget);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var cameraData = renderingData.cameraData;
            if (!renderingData.cameraData.camera.CompareTag(_cameraTag))
                return;

            if (_mMaterial == null)
                return;

            CommandBuffer cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, _mProfilingSampler))
            {
                _quadWarps.TryGetValue(cameraData.camera, out QuadWarp quadWarp);
                if (quadWarp)
                {
                    _mMaterial.SetTexture(BaseMap,quadWarp.tex[0]);
                    _mMaterial.SetMatrix(Homography,quadWarp.GetMatrix4X4());
                    Blitter.BlitCameraTexture(cmd, _mCameraColorTarget, _mCameraColorTarget, _mMaterial, 0);
                }
                else
                {
                    QuadWarp newQuadWarp = cameraData.camera.GetComponent<QuadWarp>();
                    if (newQuadWarp)
                    {
                        _quadWarps.Add(cameraData.camera,newQuadWarp);
                        _mMaterial.SetTexture(BaseMap,newQuadWarp.tex[0]);
                        _mMaterial.SetMatrix(Homography,newQuadWarp.GetMatrix4X4());
                        Blitter.BlitCameraTexture(cmd, _mCameraColorTarget, _mCameraColorTarget, _mMaterial, 0);
                    }
                }
            }
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();

            CommandBufferPool.Release(cmd);
        }
    }
}