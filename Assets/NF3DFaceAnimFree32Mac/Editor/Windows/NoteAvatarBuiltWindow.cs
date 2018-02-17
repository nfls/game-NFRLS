using Assets.Scripts.NFScript;
using MassAnimation.Resources.Entities;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.NFEditor
{
    public class NoteAvatarBuiltWindow : EditorWindow
    {
        private const string MESSAGE_A =
           "\n\nCongratulations! You have just created a 3D-face. \n\nDo you know this 3D-face already has high-quality animation built-in? Try out using the custom editor! \n\nIt might take you days or weeks to create manually, even after years of training. \n\nTo make full use of your great creations, why not get the Pro-version of NaturalFront Plugin  \n\n • Exports unlimited 3D-heads with animation to FBX files \n\n • Automatic animation between key-frames \n\n • Synchronization between animation and audio, including lip-sync \n\n • Faster animation than this Free-version \n\n • Clear texture without the letters “NF”. ";  


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
            try
            { 
                GUI.TextArea(new Rect(5, 30, 400, 310), MESSAGE_A);

                GUIStyle linkStyle = new GUIStyle(GUI.skin.label);
                Color linkColor = EditorGUIUtility.isProSkin ? new Color(0.5f, 0.5f, 1) : Color.blue;
                linkStyle.normal.textColor = linkColor;
                linkStyle.hover.textColor = linkColor;

                if (GUI.Button(new Rect(5, 365, 350, 20), "Get the Pro-version ", linkStyle))
                {
                    Application.OpenURL("https://www.assetstore.unity3d.com/en/#!/content/85459");
                }

            }
            catch (UnityException exp)
            {
                Debug.LogError(exp.Message);
            }

        }

    }
}
