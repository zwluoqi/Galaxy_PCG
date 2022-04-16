using System;
using System.Collections;
using System.Collections.Generic;
using Planet;
using Planet.Setting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public struct PlanetSettingData
{
    public bool gpu;
    public bool ocean;
    public float radius;
}

public class PlanetMesh : MonoBehaviour
{

    public bool GPU = true;
    [Range(2,256)]
    public int resolution = 4;
    [Range(2,256)]
    public int oceanResolution = 4;
    
    public ShapeSettting ShapeSettting;
    public ColorSettting ColorSettting;
    public WaterRenderSetting WaterRenderSettting;
    
    public MeshFilter[] _meshFilterss;
    public MeshFilter[] _oceanMeshFilterss;

    
    [NonSerialized]
    public bool shapeSetttingsFoldOut;
    [NonSerialized]
    public bool colorSetttingsFoldOut;
    [NonSerialized]
    public bool waterRenderSetttingsFoldOut;
    
    [NonSerialized]
    public bool showNormalAndTangent;


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

    void UpdateBase()
    {
        InitedMeshed();

        Refresh();
    }

    void Refresh()
    {
        _vertexGenerate .UpdateConfig(ShapeSettting);
        _colorGenerate .UpdateConfig(ColorSettting,WaterRenderSettting);
        PlanetSettingData settingData = GetPlanetSettingData();
        settingData.ocean = false;
        _terrainGenerate.UpdateMesh(resolution,_vertexGenerate,settingData,_colorGenerate);
        settingData.ocean = true;
        _oceanTerrainGenerate.UpdateMesh(oceanResolution,_vertexGenerate,settingData,_colorGenerate);
    }

    private void InitedMeshed()
    {
        bool refresh = false;
        var resetTerrain = CreateMeshed(ref _meshFilterss,true);
        if (resetTerrain || _terrainGenerate == null)
        {
            if (_terrainGenerate == null)
            {
                _terrainGenerate = new TerrainGenerate();
            }
            _terrainGenerate.UpdateMeshFilter(_meshFilterss,resolution);
            refresh = true;
        }
        
        var resetOceanTerrain = CreateMeshed(ref _oceanMeshFilterss,false);
        if (resetOceanTerrain || _oceanTerrainGenerate == null)
        {
            if (_oceanTerrainGenerate == null)
            {
                _oceanTerrainGenerate = new TerrainGenerate();
            }
            _oceanTerrainGenerate.UpdateMeshFilter(_oceanMeshFilterss,oceanResolution);
            refresh = true;
        }

        if (refresh)
        {
            Refresh();
        }
    }

    private bool CreateMeshed(ref MeshFilter[] meshFilterss,bool collide)
    {
        bool reCreateSharedMesh = false;
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
                meshFilterss[i].sharedMesh = new Mesh {name = i + "", hideFlags = HideFlags.DontSave};
                if (collide)
                {
                    meshFilterss[i].GetComponent<MeshCollider>().sharedMesh = meshFilterss[i].sharedMesh;
                }

                reCreateSharedMesh = true;
            }
            else
            {
                meshFilterss[i].sharedMesh.hideFlags = HideFlags.DontSave;
            }
        }

        return reCreateSharedMesh;
    }


    private PlanetSettingData GetPlanetSettingData()
    {
        PlanetSettingData settingData = new PlanetSettingData();
        settingData.gpu = GPU;
        settingData.radius = ShapeSettting.radius;
        // settingData.ocean = ocean;
        return settingData;
    }
    


    void UpdateShape()
    {
        InitedMeshed();
        _vertexGenerate .UpdateConfig(ShapeSettting);
        PlanetSettingData settingData = GetPlanetSettingData();
        settingData.ocean = false;
        _terrainGenerate.UpdateShape(_vertexGenerate,settingData,_colorGenerate);
        settingData.ocean = true;
        _oceanTerrainGenerate.UpdateShape(_vertexGenerate,settingData,_colorGenerate);
    }
    
    void UpdateColor()
    {
        InitedMeshed();
        _colorGenerate .UpdateConfig(ColorSettting,WaterRenderSettting);
        PlanetSettingData settingData = GetPlanetSettingData();
        settingData.ocean = false;
        _terrainGenerate.UpdateColor(_colorGenerate,settingData);
        settingData.ocean = true;
        _oceanTerrainGenerate.UpdateColor(_colorGenerate,settingData);
    }

    void UpdateWaterRender()
    {
        _oceanTerrainGenerate.UpdateWaterRender(WaterRenderSettting);
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

    public void OnBaseUpdate()
    {
        Debug.LogWarning(this.name+"OnBaseUpdate Start");
        UpdateBase();
        Debug.LogWarning(this.name+"OnBaseUpdate End");
    }

    private void OnDrawGizmos()
    {
        if (showNormalAndTangent)
        {
            _oceanTerrainGenerate.OnDrawGizmos(ShapeSettting.radius);
        }
    }

    public void UpdateMaterialProperty()
    {
        InitedMeshed();
        PlanetSettingData settingData = GetPlanetSettingData();
        settingData.ocean = false;
        _terrainGenerate.UpdateMaterialProperty(ColorSettting,WaterRenderSettting,settingData);
        settingData.ocean = true;
        _oceanTerrainGenerate.UpdateMaterialProperty(ColorSettting,WaterRenderSettting,settingData);
    }

    public void OnWaterRenderSetttingUpdated()
    {
        Debug.LogWarning(this.name+"UpdateWaterRender Start");
        UpdateWaterRender();
        Debug.LogWarning(this.name+"UpdateWaterRender End");
    }
}
