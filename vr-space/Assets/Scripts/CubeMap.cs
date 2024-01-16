using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Linq;
namespace BodySource
{
public class CubeMap : MonoBehaviour
{

    private MeshRenderer _meshRenderer01;
    private MeshRenderer _meshRenderer02;
    private MeshRenderer _meshRenderer03;
    private MeshRenderer _meshRenderer04;
    private MeshRenderer _meshRenderer05;
    
    private Material _material01;
    private Material _material02;
    private Material _material03;
    private Material _material04;
    private Material _material05;
    
    
    [Serializable]
    public class Config
    {
        public GameObject Screen;
    }

    public List<GameObject> screens;
    public List<MeshRenderer> meshes= new List<MeshRenderer>();  
    public List<Material> meries=new List<Material>();   
    void Start()
    {
     
            _meshRenderer01=screens[0].GetComponent<MeshRenderer>();
            meshes.Add(_meshRenderer01);
            _meshRenderer02=screens[1].GetComponent<MeshRenderer>();
            
            meshes.Add(_meshRenderer02);
            _meshRenderer03=screens[2].GetComponent<MeshRenderer>();
            _meshRenderer04=screens[3].GetComponent<MeshRenderer>();
            _meshRenderer05=screens[4].GetComponent<MeshRenderer>();
            
            meshes.Add(_meshRenderer03);
            meshes.Add(_meshRenderer04);
            meshes.Add(_meshRenderer05);
            _material01= new Material(Shader.Find("Standard"));
            _material02= new Material(Shader.Find("Standard"));
            _material03= new Material(Shader.Find("Standard"));
            _material04= new Material(Shader.Find("Standard"));
            _material05= new Material(Shader.Find("Standard"));
            meries.Add(_material01);
            meries.Add(_material02);
            meries.Add(_material03);
            meries.Add(_material04);
            meries.Add(_material05);
            StartCoroutine(MyTask());
    }   
    
    IEnumerator MyTask()
    {  
        while(true)// or for(i;i;i)
        {
            foreach (var screen in screens) 
            {
                string direction = screen.name;
                int index = screens.ToList().IndexOf(screen);
                string dir = "https://static-1253924368.cos.ap-beijing.myqcloud.com/test/lingjing/" + direction+".png";
                StartCoroutine(LoadTexture(dir, meries[index], meshes[index]));
            } 
            yield return new WaitForSeconds(10.0f); // first
        }
    }
    IEnumerator LoadTexture(string path, Material material, MeshRenderer meshRenderer)
    {
        Texture2D newTexture = null;

        if (path.Contains("://") || path.Contains(":///"))
        {
            using (WWW www = new WWW(path))
            {
                yield return www;
                if (www.error == null)
                {
                    // 加载新纹理
                    newTexture = www.texture;
                }
                else
                {
                    Debug.LogError("Failed to load texture: " + www.error);
                }
            }
        }

        if (newTexture != null)
        {
            Texture2D oldTexture = (Texture2D)material.mainTexture;
            // 替换新纹理
            material.mainTexture = newTexture;
            meshRenderer.material = material;

            // 销毁旧纹理
            if ( oldTexture != null && oldTexture is Texture2D)
            {
                Destroy(oldTexture);
            }
        }

        yield return null;
    }
}
}
