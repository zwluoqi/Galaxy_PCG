


using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SkyCell))]
public class SkyCellEditor: UnityEditor.Editor
{
        
    public SkyCell my;
    private GUISkin guiSkin;

    private void OnEnable()
    {
        my = (SkyCell)target;
        guiSkin = Resources.Load<GUISkin>("solar");
        guiSkin.label.alignment = TextAnchor.LowerLeft;
        guiSkin.label.normal.textColor = Color.white;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var astronomicals = GameObject.FindObjectsOfType<Astronomical>();
        var curAcceleration = AstronomicalUtil.GetAccelerationByPosition(astronomicals,my.transform.position);
        SkyCell.ShowAccelration = EditorGUILayout.Toggle("显示加速度", SkyCell.ShowAccelration);
        EditorGUILayout.Vector3Field("引力加速度", curAcceleration);
    }

    public void OnSceneGUI()
    {
        //第一个参数为在场景中显示的位置(以物体的中心位置为基准)
        //第二个参数为显示的名字
        //用于在场景中显示设置的名字
        
        var _astronomicals = FindObjectsOfType<Astronomical>();

        if (SolarSystemSimulater.Inst.showDistance)
        {
            foreach (var astronomical in _astronomicals)
            {
                if (astronomical.GetComponent<SkyCell>() != my)
                {
                    var sqrMagnitude = Vector3.SqrMagnitude(astronomical.transform.position - my.transform.position);
                    Handles.DrawLine(my.transform.position, astronomical.transform.position);
                    var centerPos = (my.transform.position + astronomical.transform.position) * 0.5f;

                    var acceleration = GlobalDefine.G * astronomical.Mass / sqrMagnitude;
                    if (SkyCell.ShowAccelration)
                    {
                        Handles.Label(centerPos, Mathf.Sqrt(sqrMagnitude) + "M" + ",A:" + acceleration, guiSkin.label);
                    }
                    else
                    {
                        Handles.Label(centerPos, Mathf.Sqrt(sqrMagnitude) + "M", guiSkin.label);
                    }
                }
            }
        }

        if (SolarSystemSimulater.Inst != null)
        {
            Simulater.Inst.Update();
        }
        
        if (SolarSystemSimulater.Inst.showGravity)
        {
            foreach (var my in _astronomicals)
            {
                var gravityCircleMin = my.surfaceGravity * SolarSystemSimulater.Inst.minGravityScale;
                var gravityCircleMax = my.surfaceGravity * SolarSystemSimulater.Inst.maxGravityScale;
                //Mass = surfaceGravity * Radius * Radius / GlobalDefine.G;
                var circleMin = my.Radius * Mathf.Sqrt(my.surfaceGravity / gravityCircleMin);
                var circleMax = my.Radius * Mathf.Sqrt(my.surfaceGravity / gravityCircleMax);

                Handles.color = my._color;
                Handles.DrawWireDisc(my.transform.position, Vector3.forward, circleMin);
                Handles.DrawWireDisc(my.transform.position, Vector3.forward, circleMax);
            }
        }
    }
}