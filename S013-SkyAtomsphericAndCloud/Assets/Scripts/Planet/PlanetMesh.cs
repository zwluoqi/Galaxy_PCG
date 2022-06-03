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
    public float radius;
}

public class PlanetMesh : MonoBehaviour, ISettingUpdate
{

    public bool GPU = true;
    [Range(2,256)]
    public int resolution = 4;
    
    public ShapeSettting ShapeSettting;
    public ColorSettting ColorSettting;
    public WaterRenderSetting WaterRenderSettting;
    public RandomSetting randomSetting;
    
    public MeshFilter[] _meshFilterss;
    

    [NonSerialized]
    public bool showNormalAndTangent;


    private ColorGenerate _colorGenerate = new ColorGenerate();
    private VertexGenerate _vertexGenerate = new VertexGenerate();

    private TerrainGenerate _terrainGenerate;


    private void Awake()
    {
        OnBaseUpdate();
    }

    private void OnDestroy()
    {
        _terrainGenerate?.Dispose();
        _terrainGenerate = null;
    }

    void UpdateBase()
    {
        InitedMeshed();

        Refresh();
    }

    void Refresh()
    {
        _vertexGenerate .Update(ShapeSettting,randomSetting);
        _colorGenerate .Update(ColorSettting,WaterRenderSettting,randomSetting);
        PlanetSettingData settingData = GetPlanetSettingData();
        _terrainGenerate.UpdateMesh(resolution,settingData);
    }

    private void InitedMeshed()
    {
        bool refresh = false;
        var resetTerrain = CreateMeshed(ref _meshFilterss,true);
        if (resetTerrain || _terrainGenerate == null)
        {
            if (_terrainGenerate == null)
            {
                _terrainGenerate = new TerrainGenerate(_vertexGenerate,_colorGenerate);
            }
            _terrainGenerate.UpdateMeshFilter(_meshFilterss,resolution);
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
                Transform transform1;
                (transform1 = meshRenderer.transform).SetParent(this.transform);
                transform1.localPosition = Vector3.zero;
                transform1.localScale = Vector3.one;
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
                if (Application.isPlaying)
                {
                    meshFilterss[i].sharedMesh = new Mesh {name = i + ""};
                }
                else
                {
                    meshFilterss[i].sharedMesh = new Mesh {name = i + "", hideFlags = HideFlags.DontSave};
                }
                if (collide)
                {
                    meshFilterss[i].GetComponent<MeshCollider>().sharedMesh = meshFilterss[i].sharedMesh;
                }

                reCreateSharedMesh = true;
            }
            else
            {
                if (collide)
                {
                    meshFilterss[i].GetComponent<MeshCollider>().sharedMesh = meshFilterss[i].sharedMesh;
                }

                if (!Application.isPlaying)
                {
                    meshFilterss[i].sharedMesh.hideFlags = HideFlags.DontSave;
                }
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
        _vertexGenerate .Update(ShapeSettting,randomSetting);
        PlanetSettingData settingData = GetPlanetSettingData();
        _terrainGenerate.UpdateShape(settingData);
    }
    
    void UpdateColor()
    {
        InitedMeshed();
        _colorGenerate .Update(ColorSettting,WaterRenderSettting,randomSetting);
        PlanetSettingData settingData = GetPlanetSettingData();
        _terrainGenerate.UpdateColor(settingData);
    }

    void UpdateWaterRender()
    {
        _terrainGenerate.UpdateWaterRender();
    }


    public void OnShapeSetttingUpdated()
    {
        System.DateTime start = System.DateTime.Now;
        Debug.LogWarning(this.name+"OnShapeSetttingUpdated Start");
        UpdateShape();
        var space = System.DateTime.Now - start;
        Debug.LogWarning(this.name+"OnShapeSetttingUpdated End:"+space.TotalMilliseconds+"MS");
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
            _terrainGenerate.OnDrawGizmos(ShapeSettting.radius);
        }
    }

    public void UpdateMaterialProperty()
    {
        InitedMeshed();
        PlanetSettingData settingData = GetPlanetSettingData();
        _terrainGenerate.UpdateMaterialProperty(settingData);
    }

    public void OnWaterRenderSetttingUpdated()
    {
        Debug.LogWarning(this.name+"UpdateWaterRender Start");
        UpdateWaterRender();
        Debug.LogWarning(this.name+"UpdateWaterRender End");
    }


    public void OnRandomSettingUpdate()
    {
        Debug.LogWarning(this.name+"OnRandomSettingUpdate Start");
        UpdateBase();
        Debug.LogWarning(this.name+"OnRandomSettingUpdate End");
    }


    public void UpdateLod()
    {
        PlanetSettingData settingData = GetPlanetSettingData();
        var refreshShape = _terrainGenerate.UpdateLod(Camera.main.transform.position);
        if (refreshShape)
        {
            _terrainGenerate.UpdateShape(settingData);
        }
    }

    public void UpdateSetting(ScriptableObject scriptableObject)
    {
        if (scriptableObject is ShapeSettting)
        {
            OnShapeSetttingUpdated();
        }else if (scriptableObject is ColorSettting)
        {
            OnColorSetttingUpdated();
        }else if (scriptableObject is WaterRenderSetting)
        {
            OnWaterRenderSetttingUpdated();
        }else if (scriptableObject is RandomSetting)
        {
            OnRandomSettingUpdate();
        }
    }
}
