using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Stardust.Scripts
{
    [ExecuteInEditMode]
    public class QuadWarp : MonoBehaviour
    {
        [FormerlySerializedAs("_mat")] public Material mat;
        [FormerlySerializedAs("_matUI")] public Material matUI;
        [FormerlySerializedAs("DisplayIndex")] public int displayIndex;

        [FormerlySerializedAs("_tex")] public List<Texture> tex = new List<Texture>();
        [FormerlySerializedAs("_uvs")] public Vector2[] uvs = new[] {new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0)};
        public List<Vector2[]> Vertices = new List<Vector2[]>();

        Matrix4x4 CalcHomography(Vector2 topLeft, Vector2 topRight, Vector2 bottomRight, Vector2 bottomLeft)
        {
            var sx = (topLeft.x - topRight.x) + (bottomRight.x - bottomLeft.x);
            var sy = (topLeft.y - topRight.y) + (bottomRight.y - bottomLeft.y);

            var dx1 = topRight.x - bottomRight.x;
            var dx2 = bottomLeft.x - bottomRight.x;
            var dy1 = topRight.y - bottomRight.y;
            var dy2 = bottomLeft.y - bottomRight.y;

            var z = (dx1 * dy2) - (dy1 * dx2);
            var g = ((sx * dy2) - (sy * dx2)) / z;
            var h = ((sy * dx1) - (sx * dy1)) / z;

            var system = new[]
            {
                topRight.x - topLeft.x + g * topRight.x,
                bottomLeft.x - topLeft.x + h * bottomLeft.x,
                topLeft.x,
                topRight.y - topLeft.y + g * topRight.y,
                bottomLeft.y - topLeft.y + h * bottomLeft.y,
                topLeft.y,
                g,
                h,
            };

            var mtx = Matrix4x4.identity;
            mtx.m00 = system[0];
            mtx.m01 = system[1];
            mtx.m02 = system[2];
            mtx.m10 = system[3];
            mtx.m11 = system[4];
            mtx.m12 = system[5];
            mtx.m20 = system[6];
            mtx.m21 = system[7];
            mtx.m22 = 1f;

            return mtx;
        }
        
        public Matrix4x4 GetMatrix4X4()
        {
            int surfaceIndex = 0;
            var homographyUV = CalcHomography(uvs[0], uvs[3], uvs[2], uvs[1]);
            var homographyVtx = CalcHomography(Vertices[surfaceIndex][0], Vertices[surfaceIndex][3],
                Vertices[surfaceIndex][2], Vertices[surfaceIndex][1]);
            var homography = homographyUV * homographyVtx.inverse;
            return homography;

        }

        public void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            var homographyUV = CalcHomography(uvs[0], uvs[3], uvs[2], uvs[1]);
            Graphics.SetRenderTarget(destination);
            GL.Clear(true, true, Color.clear);

            for (int surfaceIndex = 0; surfaceIndex < 1; surfaceIndex++)
            {
                GL.PushMatrix();
                GL.LoadOrtho();
                var homographyVtx = CalcHomography(Vertices[surfaceIndex][0], Vertices[surfaceIndex][3],
                    Vertices[surfaceIndex][2], Vertices[surfaceIndex][1]);
                var homography = homographyUV * homographyVtx.inverse;

                if (!XRWorldManager.Instance.isUIRender)
                {
                    mat.mainTexture = tex[surfaceIndex];
                    if (tex.Count > 1)
                    {
                        mat.SetTexture(OverlayTex,tex[1]);
                    }
                
                    mat.SetMatrix(Homography, homography);
                    mat.SetPass(0);
                }
                else
                {
                    matUI.mainTexture = tex[surfaceIndex];
                    if (tex.Count > 1)
                    {
                        matUI.SetTexture(OverlayTex,tex[1]);
                    }
                
                    matUI.SetMatrix(Homography, homography);
                    matUI.SetPass(0);
                }


#if !UNITY_EDITOR
            var rectPixel =
new Rect(0f, 0f, Display.displays[displayIndex].renderingWidth, Display.displays[displayIndex].renderingHeight);
#else
                var rectPixel = new Rect(0f, 0f, Screen.width, Screen.height);
#endif

                GL.Viewport(rectPixel);

                GL.Begin(GL.QUADS);

                for (var i = 0; i < 4; ++i)
                {
                    GL.Vertex(Vertices[surfaceIndex][i]);
                }

                GL.End();
                GL.PopMatrix();
            }

        }

        private static readonly int OverlayTex = Shader.PropertyToID("_OverlayTex");
        private static readonly int Homography = Shader.PropertyToID("_Homography");
    }
}
