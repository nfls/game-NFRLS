
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.NFEditor
{

    public class WizardOverview : EditorWindow, IWizardPage
    {

        #region constants

        private const string Message =
            "\n\n   Overview:\n\n\n\n     A front-face photo similar to the example is required.\n\n\n     IMPORTANT: To ensure good results, please strictly follow the ";

        private const string Message_2nd = "                It is also essential to avoid ";

        #endregion


        #region public methods


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

            if (GUI.Button(new Rect(20, 520, 75, 20), "Next"))
            {
                OnNext();
            }

        }

        public void OnNext()
        {
            WizardController.GoToPage<WizardLoadPhotos>(this);
        }

        public void OnBack()
        {
        }

        public void OnReset()
        {
            
        }

        #endregion


        #region private methods

        private void DrawInstructions()
        {

            GUIStyle messageStyle = new GUIStyle(GUI.skin.label);
            messageStyle.fontSize = 11;

            GUIStyle linkStyle = new GUIStyle(GUI.skin.label);
            Color linkColor = EditorGUIUtility.isProSkin ? new Color(0.5f, 0.5f, 1) : Color.blue;
            linkStyle.normal.textColor = linkColor;
            linkStyle.hover.textColor = linkColor;


            GUI.Label(new Rect(10, 20, 400, 200), Message, messageStyle);

            if (GUI.Button(new Rect(390, 136, 80, 20), "guidelines", linkStyle))
            {
                Application.OpenURL("https://www.youtube.com/watch?v=DS7IkiH5thw");
            }

            GUI.Label(new Rect(42, 165, 400, 120), Message_2nd, messageStyle);

            if (GUI.Button(new Rect(268, 164, 150, 20), "common mistakes", linkStyle))
            {
                Application.OpenURL("https://www.youtube.com/watch?v=nN2TkKnrzTg");
            }

            var frontImage = (Texture)Resources.Load("FrontImage");
            GUI.DrawTexture(new Rect(260, 240, 120, 155), frontImage, ScaleMode.StretchToFill);
            GUI.Label(new Rect(400, 355, 100, 20), "(example)");
        }

        #endregion

    }

}
