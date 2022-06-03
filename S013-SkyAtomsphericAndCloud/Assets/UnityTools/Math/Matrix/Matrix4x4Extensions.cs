using UnityEngine;

namespace UnityTools.Math.Matrix
{
    public static class Matrix4x4Extensions
    {

        public static UnityEngine.Matrix4x4 ToUnityEngineMatrix4x4(this System.Numerics.Matrix4x4 matrix4X4)
        {
            UnityEngine.Matrix4x4 ret = UnityEngine.Matrix4x4.identity;
            ret.m00 = matrix4X4.M11;
            ret.m01 = matrix4X4.M12;
            ret.m02 = matrix4X4.M13;
            ret.m03 = matrix4X4.M14;
            ret.m10 = matrix4X4.M21;
            ret.m11 = matrix4X4.M22;
            ret.m12 = matrix4X4.M23;
            ret.m13 = matrix4X4.M24;
            ret.m20 = matrix4X4.M31;
            ret.m21 = matrix4X4.M32;
            ret.m22 = matrix4X4.M33;
            ret.m23 = matrix4X4.M34;
            ret.m30 = matrix4X4.M41;
            ret.m31 = matrix4X4.M42;
            ret.m32 = matrix4X4.M43;
            ret.m33 = matrix4X4.M44;
            return ret;
        }
        
        public static Quaternion ExtractRotation(this Matrix4x4 matrix)
        {
            Vector3 forward;
            forward.x = matrix.m02;
            forward.y = matrix.m12;
            forward.z = matrix.m22;

            Vector3 upwards;
            upwards.x = matrix.m01;
            upwards.y = matrix.m11;
            upwards.z = matrix.m21;

            return Quaternion.LookRotation(forward, upwards);
        }

        public static Vector3 ExtractPosition(this Matrix4x4 matrix)
        {
            Vector3 position;
            position.x = matrix.m03;
            position.y = matrix.m13;
            position.z = matrix.m23;
            return position;
        }

        public static Vector3 ExtractScale(this Matrix4x4 matrix)
        {
            Vector3 scale;
            scale.x = new Vector4(matrix.m00, matrix.m10, matrix.m20, matrix.m30).magnitude;
            scale.y = new Vector4(matrix.m01, matrix.m11, matrix.m21, matrix.m31).magnitude;
            scale.z = new Vector4(matrix.m02, matrix.m12, matrix.m22, matrix.m32).magnitude;
            return scale;
        }

        public static Vector3 XY0(this Vector2 vector2)
        {
            return new Vector3(vector2.x,vector2.y,0);
        }
    }
}