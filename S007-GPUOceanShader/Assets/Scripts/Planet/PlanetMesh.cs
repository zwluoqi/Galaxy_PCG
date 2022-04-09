using System;
using System.Collections;
using System.Collections.Generic;
using Planet;
using Planet.Setting;
using UnityEditor;
using UnityEngine;

public class PlanetMesh : MonoBehaviour
{
    public bool GPU = false;
    [Range(2,256)]
    public int resolution = 4;

    public ShapeSettting ShapeSettting;
    public ColorSettting ColorSettting;
    
    public MeshFilter[] _meshFilterss;

    // private Material _material;
    
    [NonSerialized]
    public bool shapeSetttingsFoldOut;
    [NonSerialized]
    public bool colorSetttingsFoldOut;
    

    private ColorGenerate _colorGenerate = new ColorGenerate();
    private VertexGenerate _vertexGenerate = new VertexGenerate();

    private TerrainGenerate _terrainGenerate;
    private GPUShapeGenerate _gpuShapeGenerate;

    public void Generate()
    {
        InitedMeshed();
        
        UpdateMesh();
    }

    private void InitedMeshed()
    {
        if (_gpuShapeGenerate != null)
        {
            _gpuShapeGenerate.Dispose();
        }
        _gpuShapeGenerate = new GPUShapeGenerate();

        _terrainGenerate = new TerrainGenerate();
        
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
            }
        }

        _terrainGenerate.Init(_meshFilterss);
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
        _vertexGenerate .UpdateConfig(ShapeSettting);
        _colorGenerate .UpdateConfig(ColorSettting);
        _terrainGenerate.UpdateMesh(resolution,_vertexGenerate,GPU?_gpuShapeGenerate:null,_colorGenerate);
    }
    
    void UpdateShape()
    {
        _vertexGenerate .UpdateConfig(ShapeSettting);
        _terrainGenerate.UpdateShape(_vertexGenerate,GPU?_gpuShapeGenerate:null,_colorGenerate);

    }
    
    void UpdateColor()
    {
        _colorGenerate .UpdateConfig(ColorSettting);
        _terrainGenerate.UpdateColor(_colorGenerate,GPU?_gpuShapeGenerate:null);
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
            Debug.LogWarning("OnValidate Start");

        Generate();
                    Debug.LogWarning("OnValidate End");

    }
}
