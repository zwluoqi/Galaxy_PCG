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
            for (int i = 0; i < v.Length; i++)
            {
                if (vertexId)
                {
                    #if UNITY_EDITOR
                    Handles.Label(this.transform.position+v[i],i.ToString());
                    #endif
                }
            }
        }
    }
}