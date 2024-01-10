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
            foreach (var screen in screens) 
            {
                string direction = screen.name;
                int index = screens.ToList().IndexOf(screen);
               
                string dir = "https://static-1253924368.cos.ap-beijing.myqcloud.com/test/lingjing/" + direction+".jpeg";
               
                StartCoroutine(LoadTexture(dir, meries[index], meshes[index]));
            }


    }
    void Update(){
       
       
    }
    IEnumerator LoadTexture(string path, Material material, MeshRenderer meshRenderer)
    {
        // Texture2D newTexture = new Texture2D(1920, 1200);
        // // Assuming path is a byte array
        // newTexture.LoadImage(path);
        // if (material.mainTexture != null && material.mainTexture is Texture2D)
        // {
        //     Texture2D oldTexture = (Texture2D)material.mainTexture;
        //     Destroy(oldTexture);
        // }
        // // 将新的 Texture2D 对象赋给 material.mainTexture
        // material.mainTexture = newTexture;
        //
        // // 更新 meshRenderer 的材质
        // meshRenderer.material = material;
        //
        // // if (material.mainTexture != null) Destroy(material.mainTexture);
        // // material.mainTexture = texture;
        // // meshRenderer.material = material;
        // yield return null;
        if (path.Contains("://") || path.Contains(":///"))
        {
            using (WWW www = new WWW(path))
            {
                yield return www;
                if (www.error == null)
                {
                    material.mainTexture = www.texture;
                    meshRenderer.material = material;
                }
                else
                {
                    Debug.LogError("Failed to load texture: " + www.error);
                }
            }
        }
        yield return null;
    }
}
}
