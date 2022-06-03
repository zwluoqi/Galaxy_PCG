using System;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

namespace UnityTools.Physics.RayMarch
{
    public class TestRayBox:MonoBehaviour
    {
        public BoxCollider boxcollider;

        public int numberStepCloud=8;

        public Vector2 hitInfo;

        public float3 boxmin
        {
            get
            {
                return boxcollider.transform.position + boxcollider.center - boxcollider.size * 0.5f;
            }
        }
        
        public float3 boxmax
        {
            get
            {
                return boxcollider.transform.position + boxcollider.center + boxcollider.size * 0.5f;
            }
        }

        private void OnDrawGizmos()
        {
            var t = this;
            var transform = t.transform;
            var boxTrans = t.boxcollider.transform;
            var boxPos = boxTrans.position;
            t.hitInfo = RayIntersection.RayBoxIntersection(transform.position,
                transform.forward,
                t.boxmin,
                t.boxmax
            );

            
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position,transform.position+transform.forward*hitInfo.x);
            Gizmos.DrawCube(transform.position+transform.forward*hitInfo.x,Vector3.one*0.03f);
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position,transform.position+transform.forward*(hitInfo.x+hitInfo.y));
            Gizmos.DrawCube(transform.position+transform.forward*(hitInfo.x+hitInfo.y),Vector3.one*0.03f);

            Vector3 hitPoint = transform.position + transform.forward * hitInfo.x;

            Gizmos.color = Color.blue;
            float length = hitInfo.y;
            float step = length / numberStepCloud;
            float addValue = 0;
            for (int i = 0; i < numberStepCloud; i++)
            {
                Vector3 rayPos = hitPoint + addValue * transform.forward;
                Gizmos.DrawCube(rayPos,Vector3.one*0.03f);

                Vector3 dir = Vector3.one.normalized;
                var distance = RayIntersection.RayBoxIntersection(rayPos,
                    dir,
                    boxmin,
                    boxmax
                );
                
                Gizmos.color = Color.red;
                Gizmos.DrawLine(rayPos,rayPos+dir*distance.x);
                Gizmos.DrawCube(rayPos+dir*distance.x,Vector3.one*0.03f);
            
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(rayPos,rayPos+dir*(distance.x+distance.y));
                Gizmos.DrawCube(rayPos+dir*(distance.x+distance.y),Vector3.one*0.03f);

                
                
                addValue += step;
            }
        }
    }
}