using UnityEditor;
using UnityEngine;

namespace UnityTools.MeshTools
{
    public class DrawMeshNormalHelper : MonoBehaviour
    {
        public float length = 1;
        public bool normal;
        private void OnDrawGizmos()
        {
            var meshFilter = GetComponent<MeshFilter>();
            var mesh = meshFilter.sharedMesh;
            var v = mesh.vertices;
            var n = mesh.normals;
            var t = mesh.tangents;
            for (int i = 0; i < v.Length; i++)
            {
                if (normal)
                {
                    Gizmos.color = Color.blue;
                    var position = this.transform.position;
                    Gizmos.DrawLine(position + v[i], position + v[i] + n[i] * length);
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(position + v[i], position + v[i] + new Vector3(t[i].x, t[i].y, t[i].z) * length);
                }
            }
        }
    }
}