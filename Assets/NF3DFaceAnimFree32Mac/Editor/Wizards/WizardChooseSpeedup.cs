using System.Threading;

using UnityEditor;
using UnityEngine;

using MassAnimation.Avatar.Entities;

 
namespace Assets.Scripts.NFEditor
{

    public class WizardChooseSpeedup : EditorWindow, IWizardPage
    {

		#region constants


        private readonly GUIContent[] _contents =
            {
                new GUIContent("Speedy calculation with less tolerance of imprecise point-positioning"),
                new GUIContent("More tolerance of imprecise point-positioning at the cost of speed")
            };

        #endregion


        #region private members

        private int _selected;

        #endregion


        #region public methods

        public void OnGUI()
        {
            DrawInstructions();

            WizardController.DrawNext(this, "Next");

        }

        public void OnNext()
        {
            try
            {
                SetSpeedUp();

                WizardController.GoToPage<WizardWaiting>(
                    this,
                    new Rect(position.xMin, position.yMin + 100, position.width, position.height));       
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
            var labelRect = new Rect(10, 40, 300, 30);
            var gridRect = new Rect(labelRect.xMin + 30, labelRect.yMax + 20, 450, 100);
            
            GUI.Label(labelRect, "Preference - choose a processing speed ");

            _selected = GUI.SelectionGrid(gridRect, _selected, _contents, 1, EditorStyles.radioButton);
        }

        private void SetSpeedUp()
        {
            switch (_selected)
            {
                case 0:
                    Controller.SpeedUp = 2;
                    break;
                case 1:
                    Controller.SpeedUp = 0;
                    break;
                default:
                    Controller.SpeedUp = 1;
                    break;
            }

        }

        #endregion

    }

}
