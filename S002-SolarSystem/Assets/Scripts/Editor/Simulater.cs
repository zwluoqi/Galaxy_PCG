using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;


public class Simulater:IDisposable
{

    public static Simulater Inst = new Simulater();

    public int iterNumbers;
    public float simulaterSpeed;
    public int centerIndex;
    private Dictionary<int,AstronomicalData> cacheAstron = new Dictionary<int,AstronomicalData>(128);

    public int astronomicalListCount = 0;
    public AstronomicalData[] astronomicalDatas = new AstronomicalData[128];
    
    public SimulaterThread simulaterThread = new SimulaterThread();
    

    private Thread _thread;
    private bool threadRunning;

    public Simulater()
    {
        _thread = new Thread(StartThread);
        threadRunning = true;
        _thread.Start();
    }

    void StartThread()
    {
        while (threadRunning)
        {
            simulaterThread.ThreadUpdate(this);
            // Thread.Sleep(60);
        }
        Debug.LogError("thread done");
    }
    
     ~Simulater()
     {
         lock (simulaterThread._threadObj)
         {
             threadRunning = false;
         }

         Debug.LogError("Simulater ~");
         // _thread.Abort();
    }

     public void Dispose()
     {
         lock (simulaterThread._threadObj)
         {
             threadRunning = false;
         }

         Debug.LogError("Simulater Dispose");
     }



    
    public void Update()
    {
        
        var _astronomicals = GameObject.FindObjectsOfType<Astronomical>();
        
        lock (simulaterThread._threadObj)
        {
            astronomicalListCount = 0;
            int centerId = -1;
            if (SolarSystemSimulater.Inst.centerTrans != null)
            {
                centerId = SolarSystemSimulater.Inst.centerTrans.GetInstanceID();
            }

            centerIndex = -1;
            foreach (var astronomical in _astronomicals)
            {
                var id = astronomical.GetInstanceID();
                if (id == centerId)
                {
                    centerIndex = astronomicalListCount;
                }
                
                if (!cacheAstron.TryGetValue(id,out var tmp))
                {
                    tmp = new AstronomicalData();
                    cacheAstron[id] = tmp;
                }
                tmp.Mass = astronomical.Mass;
                tmp.cacheCurPoss = astronomical.transform.position;
                tmp.cacheCurVelocity = astronomical.GetCurrentVelocity();
                // tmp.cacheCurAcceleration = astronomical.GetCurAcceleration();
                astronomicalDatas[astronomicalListCount++] = tmp;
            }

            iterNumbers = SolarSystemSimulater.Inst.iterNumbers;
            simulaterSpeed = SolarSystemSimulater.Inst.simulaterSpeed;
        }

        if (!SolarSystemSimulater.Inst.showAstronomicalPreview)
        {
            return;
        }
        foreach (var astronomical in _astronomicals)
        {
            var id = astronomical.GetInstanceID();
            Handles.color = astronomical._color;
            Handles.DrawPolyLine(cacheAstron[id].cachePrePoss);
        }
    }
    
   
    
    

}
    
