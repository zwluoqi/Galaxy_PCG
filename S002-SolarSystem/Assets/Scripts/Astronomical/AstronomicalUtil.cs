using UnityEngine;

public class AstronomicalUtil
{
    public  static Vector3 GetAccelerationByPosition(Astronomical[] astronomicals,Vector3 transformPosition)
    {
        var curAcceleration = Vector3.zero;
        foreach (var astronomical in astronomicals)
        {
            var sqrtDistance = Vector3.SqrMagnitude(astronomical.transform.position - transformPosition);
            if (sqrtDistance < Mathf.Epsilon)
            {
                continue;
            }
            var forceDir = (astronomical.transform.position - transformPosition).normalized;
            // var force = forceDir * GlobalDefine.G * Mass * astronomical.Mass / sqrtDistance;
            // var acceleration = force / Mass;
            var acceleration = forceDir * GlobalDefine.G  * astronomical.Mass / sqrtDistance;
            curAcceleration += acceleration;
        }

        return curAcceleration;
    }
    
    public  static Vector3 GetAccelerationForce(Astronomical[] astronomicals,Astronomical self)
    {
        var curForce = Vector3.zero;
        foreach (var astronomical in astronomicals)
        {
            var sqrtDistance = Vector3.SqrMagnitude(astronomical.transform.position - self.transform.position);
            if (sqrtDistance < Mathf.Epsilon)
            {
                continue;
            }
            var forceDir = (astronomical.transform.position - self.transform.position).normalized;
            var force = forceDir * GlobalDefine.G * self.Mass * astronomical.Mass / sqrtDistance;
            // var acceleration = force / Mass;
            // var acceleration = forceDir * GlobalDefine.G  * astronomical.Mass / sqrtDistance;
            curForce += force;
        }

        return curForce;
    }
    
    public static Vector3 GetUpCircleInitVelocity(Astronomical self,Vector3 speedAxis,Astronomical center, Astronomical[] astrons)
    {

        var force = AstronomicalUtil.GetAccelerationForce(astrons,self);
        
        var distance = Vector3.Distance(self.transform.position, center.transform.position);
        var normal = force.normalized;
        // var randomVec = MathUtil.RandomVector();
        var randomVec = speedAxis;
        var tangentDir = Vector3.Cross(normal, randomVec).normalized;

        var vector3Value = center.InitVelocity + tangentDir * Mathf.Sqrt(force.magnitude * distance / self.Mass);
        return vector3Value;
    }
}