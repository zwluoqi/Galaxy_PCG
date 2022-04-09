using System;
using System.Collections;
using System.Collections.Generic;
using Planet;
using Planet.Setting;
using UnityEditor;
using UnityEngine;

public struct PlanetSettingData
{
    public bool gpu;
    public bool ocean;
}

public class PlanetMesh : MonoBehaviour
{

    public bool GPU = false;
    public bool hasOcean = false;
    [Range(2,256)]
    public int resolution = 4;
    [Range(2,256)]
    public int oceanResolution = 4;
    
    public ShapeSettting ShapeSettting;
    public ColorSettting ColorSettting;
    
    public MeshFilter[] _meshFilterss;
    public MeshFilter[] _oceanMeshFilterss;

    
    [NonSerialized]
    public bool shapeSetttingsFoldOut;
    [NonSerialized]
    public bool colorSetttingsFoldOut;
    

    private ColorGenerate _colorGenerate = new ColorGenerate();
    private VertexGenerate _vertexGenerate = new VertexGenerate();

    private TerrainGenerate _terrainGenerate;
    private TerrainGenerate _oceanTerrainGenerate;


    private void OnDestroy()
    {
        _terrainGenerate.Dispose();
        _oceanTerrainGenerate.Dispose();
        _terrainGenerate = null;
        _oceanTerrainGenerate = null;
    }

    public void Generate()
    {
        InitedMeshed();
        
        UpdateMesh();
    }

    private void InitedMeshed()
    {

        if (_terrainGenerate == null)
        {
            _terrainGenerate = new TerrainGenerate();
        }
        
        if (_oceanTerrainGenerate == null)
        {
            _oceanTerrainGenerate = new TerrainGenerate();
        }

        CreateMeshed(ref _meshFilterss,true);
        CreateMeshed(ref _oceanMeshFilterss,false);
       
        _terrainGenerate.Init(_meshFilterss);
        _oceanTerrainGenerate.Init(_oceanMeshFilterss);
    }

    private void CreateMeshed(ref MeshFilter[] meshFilterss,bool collide)
    {
        if (meshFilterss == null || meshFilterss.Length == 0)
        {
            meshFilterss = new MeshFilter[6];
        }
        
        for (int i = 0; i < 6; i++)
        {
            if (meshFilterss[i] == null)
            {
                var meshRenderer = (new GameObject(i + "")).AddComponent<MeshRenderer>();
                meshRenderer.transform.SetParent(this.transform);
                meshRenderer.transform.localPosition = Vector3.zero;
                meshRenderer.transform.localScale = Vector3.one;
                var meshFilter = meshRenderer.gameObject.AddComponent<MeshFilter>();
                if (collide)
                {
                    meshRenderer.gameObject.AddComponent<MeshCollider>();
                }

                meshFilterss[i] = meshFilter;
            }
        }

        for (int i = 0; i < 6; i++)
        {
            if (meshFilterss[i].sharedMesh == null)
            {
                meshFilterss[i].sharedMesh = new Mesh();
                meshFilterss[i].sharedMesh.name = i + "";
                if (collide)
                {
                    meshFilterss[i].GetComponent<MeshCollider>().sharedMesh = meshFilterss[i].sharedMesh;
                }
            }
        }
    }


    private PlanetSettingData GetPlanetSettingData()
    {
        PlanetSettingData settingData = new PlanetSettingData();
        settingData.gpu = GPU;
        // settingData.ocean = ocean;
        return settingData;
    }

    void UpdateMesh()
    {
        _vertexGenerate .UpdateConfig(ShapeSettting);
        _colorGenerate .UpdateConfig(ColorSettting);
        PlanetSettingData settingData = GetPlanetSettingData();
        settingData.ocean = false;
        _terrainGenerate.UpdateMesh(resolution,_vertexGenerate,settingData,_colorGenerate);
        settingData.ocean = true;
        _oceanTerrainGenerate.UpdateMesh(oceanResolution,_vertexGenerate,settingData,_colorGenerate);
    }


    void UpdateShape()
    {
        _vertexGenerate .UpdateConfig(ShapeSettting);
        PlanetSettingData settingData = GetPlanetSettingData();
        settingData.ocean = false;
        _terrainGenerate.UpdateShape(_vertexGenerate,settingData,_colorGenerate);
        settingData.ocean = true;
        _oceanTerrainGenerate.UpdateShape(_vertexGenerate,settingData,_colorGenerate);
    }
    
    void UpdateColor()
    {
        _colorGenerate .UpdateConfig(ColorSettting);
        PlanetSettingData settingData = GetPlanetSettingData();
        settingData.ocean = false;
        _terrainGenerate.UpdateColor(_colorGenerate,settingData);
        settingData.ocean = true;
        _oceanTerrainGenerate.UpdateColor(_colorGenerate,settingData);
    }


    public void OnShapeSetttingUpdated()
    {
        Debug.LogWarning(this.name+"OnShapeSetttingUpdated Start");
        UpdateShape();
        Debug.LogWarning(this.name+"OnShapeSetttingUpdated End");
    }

    public void OnColorSetttingUpdated()
    {
        Debug.LogWarning(this.name+"OnColorSetttingUpdated Start");
        UpdateColor();
        Debug.LogWarning(this.name+"OnColorSetttingUpdated End");
    }

    private void OnValidate()
    {
        Debug.LogWarning(this.name+"OnValidate Start");
        Generate();
        Debug.LogWarning(this.name+"OnValidate End");
    }
}
