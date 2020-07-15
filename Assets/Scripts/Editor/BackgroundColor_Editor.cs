using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(BackgroundColor))]
    public class BackgroundColor_Editor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var behaviour = (BackgroundColor) target;
            if (GUILayout.Button("Import"))
            {
                behaviour.ImportColors();
            }
        }
    }
}