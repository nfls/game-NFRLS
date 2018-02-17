using Assets.Scripts.NFScript;
using MassAnimation.Resources.Entities;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.NFEditor
{
    public class NoteProWindow : EditorWindow
    {
        private const string MESSAGE =
            "\n\nNice try! However, this feature is only available in the Pro-version of NaturalFront Unity Plugin that exports to FBX files: \n\n • unlimited customized 3D-heads, AND \n\n • unlimited realistic animation. \n\nWith the FBX files, the high-quality models with animation can instantly run in other 3D apps, or be imported into other scenes of Unity.  ";


        public void OnGUI()
        {
            Color defaultTextColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;

            GUI.skin.button.normal.textColor = defaultTextColor;
            GUI.skin.button.onHover.textColor = defaultTextColor;
            GUI.skin.label.normal.textColor = defaultTextColor;
            GUI.skin.label.onNormal.textColor = defaultTextColor;
            GUI.skin.label.onHover.textColor = defaultTextColor;
            EditorStyles.radioButton.onFocused.textColor = defaultTextColor;
            EditorStyles.radioButton.onHover.textColor = defaultTextColor;
            EditorStyles.radioButton.onActive.textColor = defaultTextColor;
            EditorStyles.radioButton.onNormal.textColor = defaultTextColor;
            EditorStyles.radioButton.normal.textColor = defaultTextColor;

            DrawInstructions();

        }

        private void DrawInstructions()
        {
            GUI.TextArea(new Rect(5, 30, 400, 180), MESSAGE);

            GUIStyle linkStyle = new GUIStyle(GUI.skin.label);
            Color linkColor = EditorGUIUtility.isProSkin ? new Color(0.5f, 0.5f, 1) : Color.blue;
            linkStyle.normal.textColor = linkColor;
            linkStyle.hover.textColor = linkColor;

            if (GUI.Button(new Rect(5, 235, 350, 20), "Get the Pro-version", linkStyle))
            {
                Application.OpenURL("https://www.assetstore.unity3d.com/en/#!/content/85459");
            }
        }

    }
}
