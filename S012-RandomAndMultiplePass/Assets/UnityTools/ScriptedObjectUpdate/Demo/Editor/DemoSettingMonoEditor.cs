using UnityEditor;

namespace UnityTools.ScriptedObjectUpdate
{
    [CustomEditor(typeof(DemoSettingMono))]
    public class DemoSettingMonoEditor : Editor
    {
        private SettingEditor<DemoSettingMono> shapeEdirot;

        private void OnEnable()
        {
            shapeEdirot = new SettingEditor<DemoSettingMono>();
            shapeEdirot.OnEnable(this);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            shapeEdirot.OnInspectorGUI(this);
        }
    }
}