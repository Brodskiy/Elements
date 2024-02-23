using UnityEditor;
using UnityEngine;

namespace Core.DI.Editor
{
    internal class DiPanel : EditorWindow
    {
        private const string DefaultNamespaceFieldValue = "Enter namespace";
        private const string DefaultNameFieldValue = "Enter name";

        private GUIStyle _toolbarButtonStyle;
        private static string _namespaceField;
        private static string _textField;

        [MenuItem("Tools/DI/Create new controller")]
        public static void Open()
        {
            GetWindow<DiPanel>();
            
            _namespaceField = DefaultNamespaceFieldValue;
            _textField = DefaultNameFieldValue;
        }

        private void ShowButton(Rect position)
        {
            _toolbarButtonStyle ??= new GUIStyle(GUI.skin.button)
            {
                padding = new RectOffset()
            };

            if (GUI.Button(position, EditorGUIUtility.IconContent("_Help"), _toolbarButtonStyle))
            {
                Application.OpenURL("https://docs.unity3d.com/Manual/index.html");
            }
        }

        private void OnGUI()
        {
            _textField = EditorGUILayout.TextField(_textField);
            _namespaceField = EditorGUILayout.TextField(_namespaceField);

            if (!GUILayout.Button("Create container"))
            {
                return;
            }

            if (!IsFieldValid(ref _textField, DefaultNameFieldValue))
            {
                return;
            }
            
            if (!IsFieldValid(ref _namespaceField, DefaultNamespaceFieldValue))
            {
                return;
            }
            
            GetWindow<DiPanel>().Close();
            DiContainerGenerator.Generation(RemoveWhitespace(_namespaceField), RemoveWhitespace(_textField));
            AssetDatabase.Refresh();
        }

        private bool IsFieldValid(ref string fieldValue, string defaultValue)
        {
            if (fieldValue.Length == 0)
            {
                fieldValue = "Can not be empty.";
                return false;
            }
            
            if (string.Equals(fieldValue, defaultValue))
            {
                fieldValue = "DefaultName";
            }

            return true;
        }

        private string RemoveWhitespace(string text)
        {
            return text.Replace(" ", "");
        }
    }
}