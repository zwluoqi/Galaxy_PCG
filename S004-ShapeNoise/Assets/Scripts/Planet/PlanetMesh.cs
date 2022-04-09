using System;
using System.Collections;
using System.Collections.Generic;
using Planet;
using Planet.Setting;
using UnityEngine;

public class PlanetMesh : MonoBehaviour
{
    readonly Vector3[] faceNormal = {Vector3.forward, Vector3.back, Vector3.right, Vector3.left, Vector3.up, Vector3.down};

    
    [Range(2,128)]
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

    public void Generate()
    {
        InitedMeshed();
        
        UpdateMesh();
    }

    private void InitedMeshed()
    {
        _colorGenerate = new ColorGenerate(ColorSettting);
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
                meshRenderer.gameObject.AddComponent<Collider>();
                _meshFilterss[i] = meshFilter;
            }
        }

        for (int i = 0; i < 6; i++)
        {
            if (_meshFilterss[i].sharedMesh == null)
            {
                _meshFilterss[i].sharedMesh = new Mesh();
                _meshFilterss[i].GetComponent<MeshCollider>().sharedMesh = _meshFilterss[i].sharedMesh;
            }
            _meshFilterss[i].GetComponent<MeshRenderer>().sharedMaterial = _material;
        }

        if (faceGenerates == null)
        {
            faceGenerates = new FaceGenerate[6];
            for (int i = 0; i < 6; i++)
            {
                faceGenerates[i] = new FaceGenerate(_shapeGenerate, _colorGenerate, _meshFilterss[i], faceNormal[i]);
            }
        }
    }


    void UpdateMesh()
    {
        for (int i = 0; i < 6; i++)
        {
            faceGenerates[i].Update(resolution);
        }
    }
    
    void UpdateShape()
    {
        for (int i = 0; i < 6; i++)
        {
            faceGenerates[i].UpdateShape();
        }
    }
    
    void UpdateColor()
    {
        for (int i = 0; i < 6; i++)
        {
            faceGenerates[i].UpdateColor();
        }
    }

    

    public void OnShapeSetttingUpdated()
    {
        UpdateShape();
    }

    public void OnColorSetttingUpdated()
    {
        UpdateColor();
    }

    private void OnValidate()
    {
        Generate();
    }
}
