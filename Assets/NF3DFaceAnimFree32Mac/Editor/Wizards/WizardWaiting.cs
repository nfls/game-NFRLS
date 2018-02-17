
using System;
using System.Threading;

using MassAnimation.Avatar.Entities;

using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.NFEditor
{

    public class WizardWaiting : EditorWindow
    {
        private const string Message =
            "\nAfter clicking 'OK', please wait while the 3D-head is created.\n\nPlease note:\n\n -- The waiting time will depend on the processing power of your computer (e.g. ~120 secs are expected for a 64-bit computer meeting the minimum requirements);\n\n -- The quality of the created 3D model will depend on the photo and the precision of point-positioning.";

        public void OnGUI()
        {        

            GUI.TextArea(new Rect(5, 30, 450, 150), Message);

            var labelRect = new Rect(10, 40, 100, 30);
            var gridRect = new Rect(labelRect.xMin + 30, labelRect.yMax + 10, 450, 100);
            
            var buttonRect = new Rect(gridRect.xMax - 20, gridRect.yMax + 30, 75, 20);
            GUI.Label(labelRect, "");

            if (GUI.Button(buttonRect, "OK"))
            {

                try
                {
                    
                    EditorUtility.DisplayProgressBar("Building 3D avatar, please wait ...", "50% done...", 0.5f);     

                    bool avatarBuilt = Controller.BuildAvatar();  
                    
                    if (avatarBuilt)
                    {
                        ShowAvatarBuiltNote();
                    }                     

                    EditorUtility.ClearProgressBar();                                               
                }
                catch (Exception exp)
                {
                    
                    EditorUtility.DisplayDialog("Error", exp.Message, "Ok");
                }

                WizardController.ActiveWindow = this;

                Close();

            }

        }

        private static void ShowAvatarBuiltNote()
        {

            float recWidth = 470;
            float recHeight = 420;
            float recLeft = (Screen.currentResolution.width - recWidth) * 0.5f;
            float recTop = (Screen.currentResolution.height - recHeight) * 0.5f;

            GetWindowWithRect<NoteAvatarBuiltWindow>(new Rect(recLeft, recTop, recWidth, recHeight), true, "Congratulations");
        }

    }

}
