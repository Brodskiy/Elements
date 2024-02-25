using Modules.Gameplay.Scripts.GameElement.PoolObjects;
using UnityEditor;

namespace Modules.Gameplay.Scripts.Editor
{
    [CustomEditor(typeof(BalloonItemPoolObject))]
    internal class BalloonComponentEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
            {
                DrawDefaultInspector();
                if (serializedObject.FindProperty("_withRotation").boolValue)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("_rotationRange"));
                }

                serializedObject.ApplyModifiedProperties();
            }
        
    }
}