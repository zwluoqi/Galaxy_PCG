
using System;
using UnityEngine;

public class AstronomicalData
{
    public float Mass;
    public Vector3 cacheCurPoss;
    public Vector3 cacheCurVelocity;
    // public Vector3 cacheCurAcceleration;
    
    // public float initMass;
    // public Vector3 initCurPoss;
    // public Vector3 initCurVelocity;
    // public Vector3 initCurAcceleration;
        
    public Vector3[] cachePrePoss = new Vector3[1024];
}

public class SimulaterThread
{
    public object _threadObj = new object();

    private AstronomicalData[] threadDatas = new AstronomicalData[128];
    private AstronomicalData center;
    private void SetPosition(AstronomicalData astronomical, int index)
    {
        astronomical.cachePrePoss[index] = astronomical.cacheCurPoss;
    }

    public void UpdateVelocity(AstronomicalData self,int astronomicalCount, float fixedTime)
    {
        for (int asIndex = 0; asIndex < astronomicalCount; asIndex++)
        {
            var astronomical = threadDatas[asIndex];
            if (astronomical != self)
            {
                var sqrtDistance = Vector3.SqrMagnitude(astronomical.cacheCurPoss- self.cacheCurPoss);
                var forceDir = (astronomical.cacheCurPoss- self.cacheCurPoss).normalized;
                var force = forceDir * GlobalDefine.G  * astronomical.Mass / sqrtDistance;
                var acceleration = force ;
                self.cacheCurVelocity += acceleration * fixedTime;
            }
        }
    }

    public void UpdatePosition(AstronomicalData self,float fixedTime)
    {
        if (center == self)
        {
                
        }
        else
        {
            Vector3 relativeVolocity;
            if (center != null)
            {
                relativeVolocity = self.cacheCurVelocity-
                                   center.cacheCurVelocity;
            }
            else
            {
                relativeVolocity = self.cacheCurVelocity;
            }
            
            self.cacheCurPoss += relativeVolocity * fixedTime;
        }
    }
    
    public void ThreadUpdate(Simulater simulater)
    {
        int astronomicalCount = 0;
        int iterNumbers = 0;
        float simulaterSpeed = 0;
        int centerIndex = 0;
        lock (_threadObj)
        {
            astronomicalCount = simulater.astronomicalListCount;
            iterNumbers = simulater.iterNumbers;
            simulaterSpeed = simulater.simulaterSpeed;
            centerIndex = simulater.centerIndex;
            center = null;
            for (int asIndex = 0; asIndex < astronomicalCount; asIndex++)
            {
                var astronomical = threadDatas[asIndex];
                if (astronomical == null)
                {
                    astronomical = new AstronomicalData();
                    threadDatas[asIndex] = astronomical;
                }
                var mainData = simulater.astronomicalDatas[asIndex];
                astronomical.Mass = mainData.Mass;
                astronomical.cacheCurPoss = mainData.cacheCurPoss;
                astronomical.cacheCurVelocity = mainData.cacheCurVelocity;
                // astronomical.cacheCurAcceleration = mainData.cacheCurAcceleration;
                if (centerIndex == asIndex)
                {
                    center = astronomical;
                }
            }
        }
        

        
        for (int i = 0; i < 1024; i++)
        {
            for (int asIndex = 0; asIndex < astronomicalCount; asIndex++)
            {
                var astronomical = threadDatas[asIndex];
                SetPosition(astronomical, i);
            }
            
            for (int j = 0; j < iterNumbers; j++)
            {
                for (int asIndex = 0; asIndex < astronomicalCount; asIndex++)
                {
                    var astronomical = threadDatas[asIndex];
                    UpdateVelocity(astronomical,astronomicalCount, simulaterSpeed * GlobalDefine.deltaTime);
                }

                for (int asIndex = 0; asIndex < astronomicalCount; asIndex++)
                {
                    var astronomical = threadDatas[asIndex];
                    UpdatePosition(astronomical, simulaterSpeed * GlobalDefine.deltaTime);
                }
            }
        }

        lock (_threadObj)
        {
            for (int asIndex = 0; asIndex < astronomicalCount; asIndex++)
            {
                var astronomical = threadDatas[asIndex];
                var mainData = simulater.astronomicalDatas[asIndex];
                Array.Copy(astronomical.cachePrePoss,mainData.cachePrePoss,1024);
            }
        }
    }
}