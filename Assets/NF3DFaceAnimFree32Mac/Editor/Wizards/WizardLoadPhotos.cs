
using MassAnimation.Utility;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.NFEditor
{

    public class WizardLoadPhotos : EditorWindow, IWizardPage
    {

        #region constants

        private const int MinPhotoDimension = 100;
        private const float SectionHeight = 150;

        private const string ResourcePath = "Resources";
        private const string SampleFrontPhoto = "SampleFront.jpg";

        #endregion


        #region private members

        private bool started = false;
   
		private Texture2D frontImgTexture;	

        #endregion


        #region public methods

        public void OnGUI()
        {
            if (!started) 
			{
                Start();
            }

            var labelRect = new Rect((float)0 + 10, (float)0 + 30, 200, 20);
            GUI.Label(labelRect, "   Step 1 – Load the photo");			

            const float left = (float)0 + 40;        
            float top = labelRect.yMax + 80;        

			string imgPath = null;
			DrawSection(left, top, "Frontal Image (Required)", (Texture)Resources.Load("FrontImage"), ref frontImgTexture, ref imgPath);
			Controller.FrontImagePath = imgPath;

            WizardController.DrawResetBackNext(this, true, "Next");

        }

        public void OnNext()
        {
            if ((frontImgTexture != null) && (5 < frontImgTexture.width) && (5 < frontImgTexture.height))
            {
                WizardController.GoToPage<WizardClickFrontal>(this);
                WizardClickFrontal.mainTexture = frontImgTexture;

            }
        }

        public void OnBack()
        {
            WizardController.GoToPage<WizardOverview>(this);
        }


        public void OnReset()
        {
            frontImgTexture = null;
            WizardClickFrontal.mainTexture = null;
 
        }

        #endregion


        #region private methods

        private void Start()
        {
            frontImgTexture = WizardClickFrontal.mainTexture;

            started = true;
        }

        private static void DrawSection(float left, float top, string headTitle, Texture example, ref Texture2D userTexture, ref string imgPath)
        {
            var labelRect = new Rect(left, top, 200, 18);
            var textureRect = new Rect(left + 20, top + labelRect.height + 60, 100, SectionHeight - 40);
            var photoRect = new Rect(
                textureRect.xMax + 140,
                textureRect.yMin,
                textureRect.width * 1.5f,
                textureRect.height);

            var useSampleRect = new Rect(
                photoRect.xMin + 20,
                photoRect.yMax + 25,
                (float)(WizardController.ButtonWidth * 1.2), WizardController.ButtonHeight);

            var loadRect = new Rect(photoRect.xMax + 120, photoRect.yMin + 25, WizardController.ButtonWidth, WizardController.ButtonHeight);
            var resetRect = new Rect(photoRect.xMax + 120, photoRect.yMin + 45 + WizardController.ButtonHeight, WizardController.ButtonWidth, WizardController.ButtonHeight);

            var temp = GUI.backgroundColor;

            GUI.Label(labelRect, headTitle);
            GUI.Box(textureRect, example);

            
            GUI.backgroundColor = Color.white;
           
            if ((userTexture != null) && (MinPhotoDimension < userTexture.width) && (MinPhotoDimension < userTexture.height))

            {
                GUI.Box(photoRect, userTexture);
			}
            else
            {
                userTexture = new Texture2D(4, 4, TextureFormat.DXT1, false);
                GUI.Box(photoRect, "Not Loaded");
                if (GUI.Button(useSampleRect, "Use sample"))
                {
                    string photoDir = FileLocator.GetCustomDirectory(ResourcePath);
                    string photoPath = Path.Combine(photoDir, SampleFrontPhoto);
                    if (File.Exists(photoPath))
                    {
                        imgPath = photoPath;
                        WWW www = new WWW("file:///" + photoPath);
                        www.LoadImageIntoTexture(userTexture);
                    }

                }

            }

            GUI.backgroundColor = Color.yellow;
            if (GUI.Button(loadRect, "Load")) {

                string[] filters = { "Image files", "jpg,png,jpeg", "All Files", "*" };
                string fileDirection = EditorUtility.OpenFilePanelWithFilters("Image to load", "", filters);
                if (fileDirection.Length != 0) 
				{
					imgPath = fileDirection;
                    WWW www = new WWW("file:///" + fileDirection);
                    www.LoadImageIntoTexture(userTexture);
                }
            }
            
            if (GUI.Button(resetRect, "Reset")) 
			{
                userTexture = null;
            }

            GUI.backgroundColor = temp;

        }

        #endregion
    }

}
