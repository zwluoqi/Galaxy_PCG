using System;
using System.Collections;
using System.Collections.Generic;
using Planet;
using Planet.Setting;
using UnityEditor;
using UnityEngine;

public class PlanetMesh : MonoBehaviour
{
    readonly Vector3[] faceNormal = {Vector3.forward, Vector3.back, Vector3.right, Vector3.left, Vector3.up, Vector3.down};

    
    [Range(2,256)]
    public int resolution = 4;

    public ShapeSettting ShapeSettting;
    public ColorSettting ColorSettting;
    
    public MeshFilter[] _meshFilterss;

    private Material _material;
    
    [NonSerialized]
    public bool shapeSetttingsFoldOut;
    [NonSerialized]
    public bool colorSetttingsFoldOut;
    
    private FaceGenerate[] faceGenerates;

    private ColorGenerate _colorGenerate;
    private ShapeGenerate _shapeGenerate;
    private GPUShapeGenerate _gpuShapeGenerate;

    public void Generate()
    {
        InitedMeshed();
        
        UpdateMesh();
    }

    private void InitedMeshed()
    {
        _colorGenerate = new ColorGenerate(ColorSettting);
        if (_gpuShapeGenerate != null)
        {
            _gpuShapeGenerate.Dispose();
        }
        _gpuShapeGenerate = new GPUShapeGenerate();
        _shapeGenerate = new ShapeGenerate(ShapeSettting);
        if (_material == null)
        {
            _material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        }
        
        if (_meshFilterss == null || _meshFilterss.Length == 0)
        {
            _meshFilterss = new MeshFilter[6];
            for (int i = 0; i < 6; i++)
            {
                var meshRenderer = (new GameObject(i+"")).AddComponent<MeshRenderer>();
                meshRenderer.transform.SetParent(this.transform);
                meshRenderer.transform.localPosition = Vector3.zero;
                meshRenderer.transform.localScale = Vector3.one;
                var meshFilter = meshRenderer.gameObject.AddComponent<MeshFilter>();
                meshRenderer.gameObject.AddComponent<MeshCollider>();
                _meshFilterss[i] = meshFilter;
            }
        }

        for (int i = 0; i < 6; i++)
        {
            if (_meshFilterss[i].sharedMesh == null)
            {
                _meshFilterss[i].sharedMesh = new Mesh();
                _meshFilterss[i].sharedMesh.name = i + "";
                _meshFilterss[i].GetComponent<MeshCollider>().sharedMesh = _meshFilterss[i].sharedMesh;
                _meshFilterss[i].GetComponent<MeshRenderer>().sharedMaterial = _material;
            }
        }

        if (faceGenerates == null)
        {
            faceGenerates = new FaceGenerate[6];
            for (int i = 0; i < 6; i++)
            {
                faceGenerates[i] = new FaceGenerate(_meshFilterss[i], faceNormal[i]);
            }
            
        }
    }

#if UNITY_EDITOR
    public void SaveMesh(int indx)
    {
        var mesh = new Mesh();
        mesh.vertices = _meshFilterss[indx].sharedMesh.vertices;
        mesh.triangles = _meshFilterss[indx].sharedMesh.triangles;
        mesh.uv = _meshFilterss[indx].sharedMesh.uv;
        mesh.normals = _meshFilterss[indx].sharedMesh.normals;
        mesh.colors = _meshFilterss[indx].sharedMesh.colors;
        mesh.UploadMeshData(false);
        foreach (var vector3 in mesh.vertices)
        {
            Debug.LogWarning(vector3);
        }
        AssetDatabase.CreateAsset(mesh,"Assets/mesh"+resolution+".asset");
    }
#endif


    void UpdateMesh()
    {
        for (int i = 0; i < 6; i++)
        {
            faceGenerates[i].Update(resolution,_shapeGenerate,_gpuShapeGenerate,_colorGenerate);
        }
    }
    
    void UpdateShape()
    {
        _shapeGenerate = new ShapeGenerate(ShapeSettting);
        for (int i = 0; i < 6; i++)
        {
            faceGenerates[i].UpdateShape(_shapeGenerate,_gpuShapeGenerate);
        }
    }
    
    void UpdateColor()
    {
        _colorGenerate = new ColorGenerate(ColorSettting);
        for (int i = 0; i < 6; i++)
        {
            faceGenerates[i].UpdateColor(_colorGenerate);
        }
    }

    

    public void OnShapeSetttingUpdated()
    {
        Debug.LogWarning("OnShapeSetttingUpdated Start");
        UpdateShape();
        Debug.LogWarning("OnShapeSetttingUpdated End");
    }

    public void OnColorSetttingUpdated()
    {
        Debug.LogWarning("OnColorSetttingUpdated Start");
        UpdateColor();
        Debug.LogWarning("OnColorSetttingUpdated End");
    }

    private void OnValidate()
    {
        Generate();    

        
    }
}
