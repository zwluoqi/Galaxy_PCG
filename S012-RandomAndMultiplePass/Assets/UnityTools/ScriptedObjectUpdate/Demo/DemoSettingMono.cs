using UnityEngine;

namespace UnityTools.ScriptedObjectUpdate
{
    public class DemoSettingMono : MonoBehaviour, ISettingUpdate
    {
        public DemoSetting demoSetting;
        public void UpdateSetting(ScriptableObject scriptableObject)
        {
            throw new System.NotImplementedException();
        }
    }
}