
using MassAnimation.Resources;
using MassAnimation.Resources.Entities;

using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.NFEditor
{
    public class WizardChooseEyeColor : EditorWindow, IWizardPage
    {

        #region constants
       
        private const string Message =
            "\n\n  Preference - choose an eye color ";

        #endregion


        #region private members

        private int _selected;

        #endregion


        #region public methods

        public void OnGUI()
        {
            DrawInstructions();

            DrawEyesList();

            WizardController.DrawNext(this, "Next");

        }

        public void OnNext()
        {
            try
            {
                SetEyeColor();
                WizardController.GoToPage<WizardChooseSpeedup>(
                    this,
                    
                    new Rect(position.xMin, position.yMin + 100, position.width * 1.3f, position.height*0.8f));       
            }
            catch (UnityException exp)
            {
                Debug.LogError(exp.Message);
            }
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
            GUI.Label(new Rect(10, 20, 150, 20), Message, new GUIStyle { fontSize = 11 });

        }

        private void DrawEyesList()
        {
            var style = EditorStyles.radioButton;
            style.imagePosition = ImagePosition.ImageAbove;
            style.stretchHeight = true;
            GUI.skin.button.alignment = TextAnchor.MiddleLeft;
            GUIContent[] contents =
            {
                new GUIContent {image = (Texture) Resources.Load("eyeL_brown_hi"), text = "Brown"},

                new GUIContent {image = (Texture) Resources.Load("eyeL_blue_hi"), text = "Blue"},              
                new GUIContent {image = (Texture) Resources.Load("eyeL_gray_hi"), text = "Gray"},
                new GUIContent {image = (Texture) Resources.Load("eyeL_hazel_hi"), text = "Hazel"}
            };

            var rect = new Rect(30, 90, 350, 160);      
            _selected = GUI.SelectionGrid(rect, _selected, contents, 4, style);
        }


		private void SetEyeColor()
		{
			switch(_selected)
			{
				case 0:
					Controller.ColorOfEyes = EyeColor.Brown;
					break;
				case 1:
					Controller.ColorOfEyes = EyeColor.Blue;
					break;
				case 2:
					Controller.ColorOfEyes = EyeColor.Gray;
					break;
				case 3:
					Controller.ColorOfEyes = EyeColor.Hazel;
					break;
				default:
					Controller.ColorOfEyes = EyeColor.Brown;
					break;
			}

        }

        #endregion

    }

}