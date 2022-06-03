using UnityEditor;
using UnityEngine;

namespace UnityTools.MeshTools
{
    public class DrawMeshVertexHelper : MonoBehaviour
    {
        public bool vertexId;
        private void OnDrawGizmos()
        {
            var meshFilter = GetComponent<MeshFilter>();
            var mesh = meshFilter.sharedMesh;
            var v = mesh.vertices;
            var n = mesh.normals;
            var t = mesh.tangents;
            var uv = mesh.uv;
            for (int i = 0; i < v.Length; i++)
            {
                if (vertexId)
                {
                    #if UNITY_EDITOR
                    var pos = this.transform.localToWorldMatrix * new Vector4(v[i].x, v[i].y, v[i].z, 1);
                    Handles.Label(pos,i.ToString()+$"({uv[i].ToString()})");
                    #endif
                }
            }
        }
    }
}