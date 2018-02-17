
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using MassAnimation.Resources;
using MassAnimation.Resources.Entities;
using MassAnimation.UnityPluginConnector;
using MassAnimation.Utility;

using UnityEditor;
using UnityEngine;


namespace Assets.Scripts.NFEditor
{

    public class AppUIWindow : EditorWindow
	{

        #region constants	


        private const string DocDirectory = "documentation";
        private const string DocName = "documentation.pdf";

        private const string NotProperlyMsg = "Please get the right conditions met, before running this function. ";


        #endregion


        #region Private Fields

        private Texture2D _exportIcon;
        private Texture2D _fromImageIcon;
	    private Texture2D _loadAudioIcon;
	    private Texture2D _levelOfControlIcon;

	    private Vector2 _scrollPosition;

	    private bool _showsBrowsPosition;

	    private bool _showsPrimaryExpression;

        private bool _showEyeDetails;
        private bool _showEyeColors;

        private bool _showEyesDirectionPosition;

        private bool _showsMovementsPosition;

        private bool _mouthPosition;

	    private bool _highLevelPosition;

	    private bool _musclesPositon;

		[SerializeField]
	    private int _browDropDownIndex;

		[SerializeField]
	    private int _primaryExpressionDropDownIndex;

		[SerializeField]
		private int _musclesDropDownIndex;

		[SerializeField]
	    private float _browSliderPosition;

        [SerializeField]
        private int _EyeColorDropDownIndex;

        [SerializeField]
		private float _primaryExpressionSliderPosition;		
			    
		[SerializeField]
	    private float _musclesSliderPosition;

        [SerializeField]
        private float _eyeHorizontalSliderPosition, _eyeVerticalSliderPosition;

        [SerializeField]
        private float _lipNarrow, _jawClose;


        private static bool _modelPathSet = false;
        private static bool doUndo = false;

        private static readonly List<EditorWindow> AllWindows = new List<EditorWindow>();

	    #endregion


		#region public methods


        [MenuItem("Window/NaturalFront 3D Facial Animation Plugin Free")]
	    public static void ShowWindow()
	    {

            GetWindowWithRect<AppUIWindow>(new Rect(50, 20, 320, 770), true, "NaturalFront 3D Facial Animation Plugin Free");

            OnGetReady();

        }

	    public void Start()
	    {		
			try
			{
				RegisterEventHandling();
                            
			}
			catch(UnityException exp)
			{
				Debug.LogError(exp.Message);
			}
	    }


		public void Update()
		{
			if(doUndo)
            {
				Undo.PerformUndo();
				doUndo = false;
			}
		}

	    public void OnGUI()
	    {

			try
			{

				Undo.RecordObject(this, "AppUIWindow");

                InitializeValues();

		        DrawMenu();

		        DrawPanelButtons();

                DrawMainButtons();		        

		        DrawPoseConfigurationControl();

				RegisterEventHandling();

			}
			catch(UnityException exp)
			{
				Debug.LogError(exp.Message);
			}

	    }

		#endregion
		

		#region private methods	    


		#region drawing methods

	    private void DrawPoseConfigurationControl()
	    {
            var boxRect = new Rect(10, 100, 300, 650);
	        GUI.Box(boxRect, "");

	        float shift = 0;
	        
	        _scrollPosition = GUI.BeginScrollView(boxRect, _scrollPosition, boxRect);
	        {
	            var headFoldoutRect = Controller.Instance.IsMostAbstractLevel
	                                      ? DrawPrimaryExpressionSection(boxRect, ref shift)
	                                      : DrawBrowsSection(boxRect, ref shift);

                var eyeFoldoutRect = DrawEyeSection(headFoldoutRect, ref shift);

                DrawMouthSection(eyeFoldoutRect, shift);
            }

	        GUI.EndScrollView();

            DrawSaveKeyReset(boxRect);

	    }

	    private Rect DrawPrimaryExpressionSection(Rect boxRect, ref float shift)
	    {
	        var primaryExpressionBox = new Rect(boxRect.xMin + 5, boxRect.yMin + 10, boxRect.width - boxRect.width * 0.1f, 10);
	        _showsPrimaryExpression = EditorGUI.Foldout(primaryExpressionBox, _showsPrimaryExpression, "Expressions", true);

	        if (_showsPrimaryExpression)
	        {
	            var dropDownBox = DrawEmotionalControl(primaryExpressionBox, ref _primaryExpressionDropDownIndex, ref _primaryExpressionSliderPosition);

	            shift += dropDownBox.height + 5;
	        }

	        return primaryExpressionBox;
	    }

        #region draw eyes

        private Rect DrawEyeSection(Rect boxRect, ref float shift)
        {
            var sectionRect = new Rect(boxRect.xMin, boxRect.yMax + shift + 10, boxRect.width, boxRect.height * 2);

            _showEyeDetails = EditorGUI.Foldout(sectionRect, _showEyeDetails, "Eyes", true);
            if (_showEyeDetails)
            {
                var eyeColorRect = DrawEyeColorSection(sectionRect, ref shift);
                var eyeDirectionRect = DrawEyeDirectionSection(eyeColorRect, ref shift);

                shift += sectionRect.height;

                return sectionRect;
            }
            else
            {
                return sectionRect;
            }

        }

        private Rect DrawEyeColorSection(Rect boxRect, ref float shift)
        {
            var EyeColorBox = new Rect(boxRect.xMin, boxRect.yMax + 5, boxRect.width, 10);

            var EyeColorBoxShift = new Rect(EyeColorBox.xMin + 10, EyeColorBox.yMin, EyeColorBox.width, EyeColorBox.height);
            _showEyeColors = EditorGUI.Foldout(EyeColorBoxShift, _showEyeColors, "Colors", true);

            if (_showEyeColors)
            {
                var dropDownBox = DrawEyeColorControl(EyeColorBoxShift, ref _EyeColorDropDownIndex);

                shift += dropDownBox.height + 5;
            }

            return EyeColorBox;
        }

        private Rect DrawEyeColorControl(Rect initialBox, ref int dropDownIndex)
        {
            var dropDownBox = new Rect(initialBox.xMin + 5, initialBox.yMax + 10, 100, 20);

            int EyeColorIndex = Controller.Instance.SelectedEyeColorIndex;
            dropDownIndex = EditorGUI.Popup(
                dropDownBox,
                EyeColorIndex,
                new[] { "Brown", "Blue", "Gray", "Hazel" });

            if (EyeColorIndex != dropDownIndex)
            {
                ResetPoseConfig(DropDownEnum.EyeColors);
                UpdateEyeColor();
                IntensityEventArgs args = new IntensityEventArgs(dropDownIndex);
                Controller.Instance.OnEyeColorChanged(args);
            }
            return dropDownBox;
        }

        private Rect DrawEyeDirectionSection(Rect browsFoldoutRect, ref float shift)
        {
            var eyesDirectionFoldoutRect = new Rect(browsFoldoutRect.xMin, browsFoldoutRect.yMax + shift + 5, browsFoldoutRect.width, browsFoldoutRect.height);

            var eyesDirectionShift = new Rect(eyesDirectionFoldoutRect.xMin + 10, eyesDirectionFoldoutRect.yMin, eyesDirectionFoldoutRect.width - 10, eyesDirectionFoldoutRect.height);
            _showEyesDirectionPosition = EditorGUI.Foldout(eyesDirectionShift, _showEyesDirectionPosition, "Directions", true);
            if (_showEyesDirectionPosition)
            {
                const int slidersLenght = 100;
                const int slidersThickness = 10;
                var verticalSliderRect = new Rect(eyesDirectionShift.xMin + slidersLenght + 25, eyesDirectionShift.yMax + 10, slidersThickness, slidersLenght);
                var horizontalSliderRect = new Rect(eyesDirectionShift.xMin + 20, verticalSliderRect.yMax + 5, slidersLenght, slidersThickness);


                float eyeVerticalStrength = Controller.Instance.VerticalEyeMovement;
                _eyeVerticalSliderPosition = GUI.VerticalSlider(verticalSliderRect, eyeVerticalStrength, 100, -100);

                if (eyeVerticalStrength != _eyeVerticalSliderPosition)
                {

                    UpdateEyeAnimation();

                    IntensityEventArgs args = new IntensityEventArgs(_eyeVerticalSliderPosition);
                    Controller.Instance.OnVerticalEyeMoved(args);

                }

                float eyeHorizontalStrength = Controller.Instance.HorizontalEyeMovement;
                _eyeHorizontalSliderPosition = GUI.HorizontalSlider(horizontalSliderRect, eyeHorizontalStrength, 100, -100);

                if (eyeHorizontalStrength != _eyeHorizontalSliderPosition)
                {
                    UpdateEyeAnimation();

                    IntensityEventArgs args = new IntensityEventArgs(_eyeHorizontalSliderPosition);
                    Controller.Instance.OnHorizontalEyeMoved(args);

                }

                shift += slidersLenght + slidersThickness + 20;
            }
            else
            {
                shift += eyesDirectionFoldoutRect.height + 10;
            }

            return eyesDirectionFoldoutRect;

        }

        #endregion


        private Rect DrawEmotionalControl(Rect initialBox, ref int dropDownIndex, ref float sliderPosition)
	    {
	        var dropDownBox = new Rect(initialBox.xMin + 5, initialBox.yMax + 10, 100, 20);

			int expressionIndex = Controller.Instance.SelectedExpressionIndex;
	        dropDownIndex = EditorGUI.Popup(
	            dropDownBox,
				expressionIndex,
				new[] {"Neutral", "Happiness", "Sadness", "Surprise", "Fear", "Anger", "Disgust"});

			if (expressionIndex != dropDownIndex)
			{
				ResetPoseConfig(DropDownEnum.expression);
				IntEventArgs args = new IntEventArgs(dropDownIndex);
				Controller.Instance.OnExpressionChanged(args);
			}

            float expressionStrength = Controller.Instance.ExpressionMovement;           
			
			sliderPosition = DrawSliderWithTextBox(
	            new Rect(dropDownBox.xMax + 5, dropDownBox.yMin, 100, 20),
				expressionStrength);

		
			if (expressionStrength != sliderPosition )
			{
				UpdateAnimation();
				
				IntensityEventArgs args = new IntensityEventArgs(sliderPosition);
				Controller.Instance.OnExpressionValueChanged(args);
				
			}

			return dropDownBox;
	    }


		private Rect DrawMouthMuscleControl(Rect initialBox, ref int dropDownIndex, ref float sliderPosition)
		{
			var dropDownBox = new Rect(initialBox.xMin + 5, initialBox.yMax + 10, 100, 20);

			int mouthMuscleIndex = Controller.Instance.SelectedMouthMuscleIndex;
			dropDownIndex = EditorGUI.Popup(
				dropDownBox,
				mouthMuscleIndex,
				new[] {"Happiness", "Sadness", "Surprise", "Fear", "Anger", "Disgust"});


			if (mouthMuscleIndex != dropDownIndex)
			{
				ResetPoseConfig(DropDownEnum.mouthMuscles);
				IntEventArgs args = new IntEventArgs(dropDownIndex);
				Controller.Instance.OnMouthMuscleChanged(args);
			}
			
			float mouthMuscleStrength = Controller.Instance.MouthMuscleMovement;		
			
			sliderPosition = DrawSliderWithTextBox(
				new Rect(dropDownBox.xMax + 5, dropDownBox.yMin, 100, 20),
				mouthMuscleStrength);


			if (mouthMuscleStrength != sliderPosition )
			{
                UIController.RaiseAreaResetEvent(AnimatableArea.MOUTH);

				UpdateAnimation();
				
				IntensityEventArgs args = new IntensityEventArgs(sliderPosition);
				Controller.Instance.OnMouthMuscleValueChanged(args);
				
			}
			
			return dropDownBox;
		}

		
		private Rect DrawBrowsSection(Rect boxRect, ref float shift)
		{
	        var browsFoldoutRect = new Rect(boxRect.xMin + 5, boxRect.yMin + 10, boxRect.width - boxRect.width * 0.1f, 10);
	        _showsBrowsPosition = EditorGUI.Foldout(browsFoldoutRect, _showsBrowsPosition, "Brows", true);
	        if (_showsBrowsPosition)
	        {
	            DrawBrowSection(browsFoldoutRect, ref shift);
	        }
	        return browsFoldoutRect;
	    }

	    private void DrawBrowSection(Rect browsFoldoutRect, ref float shift)
	    {
			try
			{
		        var movementFoldoutRect = new Rect(
		            browsFoldoutRect.xMin + 10,
		            browsFoldoutRect.yMax + shift + 5,
		            browsFoldoutRect.width - 10,
		            browsFoldoutRect.height);

		        _showsMovementsPosition = EditorGUI.Foldout(movementFoldoutRect, _showsMovementsPosition, "Movements", true);
		        shift += movementFoldoutRect.height + 5;

		        if (_showsMovementsPosition)
		        {
		            var dropDownBox = new Rect(movementFoldoutRect.xMin + 5, movementFoldoutRect.yMax + 10, 100, 20);

					int browIndex = Controller.Instance.SelectedBrowIndex; 
		            _browDropDownIndex = EditorGUI.Popup(
		                dropDownBox,
						browIndex,
		                new[] { "Brow Mid Up", "Brow Mid Down", "Brow Out Up" });


					if (browIndex != _browDropDownIndex)
					{
						ResetPoseConfig(DropDownEnum.brow);
						IntEventArgs args = new IntEventArgs(_browDropDownIndex);
						Controller.Instance.OnBrowChanged(args);
					}

					float browStrength = Controller.Instance.BrowMovement;
		            _browSliderPosition = DrawSliderWithTextBox(
		                new Rect(dropDownBox.xMax + 5, dropDownBox.yMin, 100, 20),
						browStrength);

					if (browStrength != _browSliderPosition)
					{
                        UIController.RaiseAreaResetEvent(AnimatableArea.EYEBROW);

						UpdateAnimation();

						IntensityEventArgs args = new IntensityEventArgs(_browSliderPosition);
						Controller.Instance.OnBrowValueChanged(args);

					}
					
					shift += dropDownBox.height + 5;
		        }

			}
			catch(UnityException exp)
			{
				Debug.LogError(exp.Message);
			}
	    }

        private void DrawMouthSection(Rect boxRect, float shift)
        {
            var sectionRect = new Rect(boxRect.xMin, boxRect.yMax + shift + 10, boxRect.width, boxRect.height);
            shift = 0;

            _mouthPosition = EditorGUI.Foldout(sectionRect, _mouthPosition, "Mouth", true);
            if (_mouthPosition)
            {
                var highLevelRect = DrawHighLevelSection(sectionRect, ref shift);

                if (!Controller.Instance.IsMostAbstractLevel)
                {
                    DrawMusclesSection(highLevelRect, ref shift);
                }
            }
        }

        private Rect DrawHighLevelSection(Rect mouthFoldoutRect, ref float shift)
	    {
	        var highLevelRect = new Rect(
	            mouthFoldoutRect.xMin + 10,
	            mouthFoldoutRect.yMax + 5,
	            mouthFoldoutRect.width - 10,
	            mouthFoldoutRect.height);
	        _highLevelPosition = EditorGUI.Foldout(highLevelRect, _highLevelPosition, "High-level", true);
	        if (_highLevelPosition)
	        {
	            var firstLabelRect = new Rect(highLevelRect.xMin + 10, highLevelRect.yMax + 5, 112, 20);
	            var secondLabelRect = new Rect(
	                firstLabelRect.xMin,
	                firstLabelRect.yMax + 5,
	                firstLabelRect.width,
	                firstLabelRect.height);
	            GUI.Label(firstLabelRect, "Lip narrow -> wide");
	            GUI.Label(secondLabelRect, "Jaw close -> open");

                float lipStrength = Controller.Instance.LipNarrowWide;
                _lipNarrow = DrawSliderWithTextBox(new Rect(firstLabelRect.xMax + 5, firstLabelRect.yMin, 100, 20), lipStrength);
                
                if (lipStrength != _lipNarrow)
                {
                    UIController.RaiseAreaResetEvent(AnimatableArea.MOUTH);

                    UpdateAnimation();

                    
                    IntensityEventArgs args = new IntensityEventArgs(_lipNarrow);
                    Controller.Instance.OnLipNarrowWideChanged(args);
                }

                float jawStrength = Controller.Instance.JawOpenClose;
                _jawClose = DrawSliderWithTextBox(new Rect(secondLabelRect.xMax + 5, secondLabelRect.yMin, 100, 20), jawStrength);
                
                if (jawStrength != _jawClose)
                {
                    UIController.RaiseAreaResetEvent(AnimatableArea.MOUTH);

                    UpdateAnimation();

                    
                    IntensityEventArgs args = new IntensityEventArgs(_jawClose);
                    Controller.Instance.OnJawOpenCloseChanged(args);
                }

	            shift += firstLabelRect.height + secondLabelRect.height + 10;
	        }

	        return highLevelRect;
	    }

	    private void DrawMusclesSection(Rect highLevelRect, ref float shift)
	    {
	        var musclesRect = new Rect(
	            highLevelRect.xMin,
	            highLevelRect.yMax + shift + 10,
	            highLevelRect.width,
	            highLevelRect.height);
	        _musclesPositon = EditorGUI.Foldout(musclesRect, _musclesPositon, "Muscles", true);
	        if (_musclesPositon)
	        {
				DrawMouthMuscleControl(musclesRect, ref _musclesDropDownIndex, ref _musclesSliderPosition);
	        }
	    }

	    private static void DrawSaveKeyReset(Rect boxRect)
	    {
	        const int buttonHeight = 25;
			const int buttonWidth = 80;

	        var rememberRect = new Rect(boxRect.xMin + 55, (boxRect.yMax - buttonHeight) - 15, buttonWidth, buttonHeight);
	        var resetRect = new Rect(rememberRect.xMax + 20, rememberRect.yMin, buttonWidth, buttonHeight);
            

	        var temp = GUI.backgroundColor;
	        GUI.backgroundColor = Color.yellow;

	        if ( GUI.Button(rememberRect, "Save Key") )
            {
                OnRemember();
            }

            if (GUI.Button(resetRect, "Reset"))
            {
                OnReset();
            }


	        GUI.backgroundColor = temp;
	    }

	    private static float DrawSliderWithTextBox(Rect sliderRect, float sliderPosition)
	    {
	        sliderPosition = GUI.HorizontalSlider(sliderRect, sliderPosition, 0, 100);

	        var sliderTextBox = new Rect(sliderRect.xMax + 10, sliderRect.yMin, 40, 20);
	        GUI.TextField(sliderTextBox, String.Format("{0:00}", sliderPosition));

	        return sliderPosition;
	    }

	    private static void DrawMainButtons()
	    {
	        var temp = GUI.backgroundColor;
	        GUI.backgroundColor = Color.yellow;

            if (GUI.Button(new Rect(20, 70, 70, 20), "Animate"))
            {
                OnAnimate();
            }

            if (GUI.Button(new Rect(120, 70, 120, 20), "Audio Visualizator"))
            {
                OnVisualizeAudio();
            }

	        GUI.backgroundColor = temp;
	    }


	    private void DrawMenu()
	    {
	        DrawFileMenu();

	        DrawModelMenu();

	        DrawMediaMenu();

	        DrawAnimationMenu();


	        DrawHelpMenu();
	    }


	    private static void DrawHelpMenu()
	    {

            if (GUI.Button(new Rect(222, 2, 50, 20), new GUIContent("Help")))
            {
                var helpMenu = new GenericMenu();

                helpMenu.AddItem(new GUIContent("Documentation"), false, ShowDocumentation);
                helpMenu.AddSeparator("");

                helpMenu.AddItem(new GUIContent("The NaturalFront difference"), false, ShowNFExplanationVideo); ;
                helpMenu.AddSeparator("");

                helpMenu.AddItem(new GUIContent("Join the discussion"), false, ShowNFUnityThread); ;
                helpMenu.AddSeparator("");

                helpMenu.AddItem(new GUIContent("NaturalFront software website"), false, ShowNFWebsite);
                helpMenu.AddSeparator("");

                helpMenu.AddItem(new GUIContent("About"), false, ShowAbout);
                helpMenu.ShowAsContext();
            }
        }


	    private static void DrawAnimationMenu()
	    {
	        if (GUI.Button(new Rect(152, 2, 70, 20), new GUIContent("Animation")))
	        {
	            var animationMenu = new GenericMenu();
	            animationMenu.AddItem(new GUIContent("Level of Control"), false, OnShowAbstractionLevel);
	            animationMenu.AddSeparator("");
	            
                animationMenu.AddItem(new GUIContent("Animate"), false, OnAnimate);
	            animationMenu.ShowAsContext();
	        }
	    }


	    private static void DrawMediaMenu()
	    {
	        if (GUI.Button(new Rect(102, 2, 50, 20), new GUIContent("Media")))
	        {
	            var loadAudioMenu = new GenericMenu();
	            loadAudioMenu.AddItem(new GUIContent("Load Audio"), false, OnLoadAudio);

	            loadAudioMenu.ShowAsContext();
	        }
	    }

	    private static void DrawModelMenu()
	    {
	        if (GUI.Button(new Rect(52, 2, 50, 20), new GUIContent("Model")))
	        {
	            var modelMenu = new GenericMenu();
	            modelMenu.AddItem(new GUIContent("From photo ..."), false, OnFromImages);
	            modelMenu.ShowAsContext();

            }
        }


	    private void DrawFileMenu()
	    {
	        if (GUI.Button(new Rect(2, 2, 50, 20), new GUIContent("File")))
	        {
	            var fileMenu = new GenericMenu();
 
	            fileMenu.AddItem(new GUIContent("Exit"), false, Exit);
	            fileMenu.ShowAsContext();
	        }
	    }



	    private void DrawPanelButtons()
	    {


	        DrawFromImageButton();

	        DrawLoadAudioButton();

	        DrawLevelOfControlButton();

            DrawExportButton();


        }


        private void DrawFromImageButton()
        {
            _fromImageIcon = (Texture2D)Resources.Load(@"from_image_icon");
            if (GUI.Button(new Rect(36 - 34, 30, 32, 32), new GUIContent(_fromImageIcon, "Generate 3D models from photo")))
            {
                if (!CheckOS())
                {
                    EditorUtility.DisplayDialog("OS is not supported", "The Unity plugin is not intended to run in this Operation System. ", "Ok");
                    return;
                }

                OnFromImages();
                
            }
        }

        private void DrawLoadAudioButton()
        {
            _loadAudioIcon = (Texture2D)Resources.Load(@"load_audio_icon");
            if (GUI.Button(new Rect(70 - 34, 30, 32, 32), new GUIContent(_loadAudioIcon, "Load audio")))
            {
                OnLoadAudio();
            }
        }


        private void DrawLevelOfControlButton()
        {
            _levelOfControlIcon = (Texture2D)Resources.Load(@"level_of_control");
            if (GUI.Button(new Rect(104 - 34, 30, 32, 32), new GUIContent(_levelOfControlIcon, "Change level of control")))
            {
                OnShowAbstractionLevel();
            }
        }


        private void DrawExportButton()
        {
            _exportIcon = (Texture2D)Resources.Load(@"export_icon");
            if (GUI.Button(new Rect(160 - 34, 30, 32, 32), new GUIContent(_exportIcon, "Export to FBX")))        
            {
                OnExport();
            }
        }

        private static void ShowDocumentation()
        {

            string docDir = FileLocator.GetCustomDirectory(DocDirectory);
            string docPath = Path.Combine(docDir, DocName);

            if (File.Exists(docPath))
            {
                Application.OpenURL(docPath);
            }

        }

        private static void ShowNFExplanationVideo()
        {
            Application.OpenURL("https://youtu.be/h9eHTCrzhXQ");
        }

        private static void ShowNFUnityThread()
        {
            Application.OpenURL("http://forum.unity3d.com/threads/released-want-good-3d-character-animation-but-are-short-on-time-budget-or-training-help-is-here.392317/");

        }

        private static void ShowNFWebsite()
        {
            Application.OpenURL("http://www.naturalfront.com/");

        }

        private static void ShowAbout()
		{
  
            float recWidth = 650;
            float recHeight = 400;
            float recLeft = (Screen.currentResolution.width - recWidth) * 0.5f;
            float recTop = (Screen.currentResolution.height - recHeight) * 0.5f;

            AllWindows.Add(GetWindowWithRect<AboutWindow>(new Rect(recLeft, recTop, recWidth, recHeight), true, "About"));
        }
		
		#endregion


		#region handlers

		private void RegisterEventHandling()
		{
			Controller.Instance.AbstractionLevelChanged += Repaint;
			
			Undo.undoRedoPerformed += Repaint;
		}


		private static void OnShowAbstractionLevel()
		{
			AllWindows.Add(GetWindowWithRect<AbstractionLevelWindow>(new Rect(200, 100, 250, 200), true, "Abstraction Level"));
			
		}


        private static void OnGetReady()
        {
            try
            {
                float recWidth = 650;
                float recHeight = 350;
                float recLeft = (Screen.currentResolution.width - recWidth) * 0.5f;
                float recTop = (Screen.currentResolution.height - recHeight) * 0.5f;

                var wizWnd = GetWindowWithRect<GetReadyWindow>(new Rect(recLeft, recTop, recWidth, recHeight), true, "Are you ready?");

                AllWindows.Add(wizWnd);
            }
            catch (System.Exception exp)
            {
                Debug.Log(exp.Message);
            }
        }


        private static void OnFromImages()
		{

            if (Controller.ReadyToProceed == false)
            {
                EditorUtility.DisplayDialog("Error", NotProperlyMsg, "Ok");
            }
            else
            {
                float recWidth = 700;
                float recHeight = 550;
                float recLeft = (Screen.currentResolution.width - recWidth) * 0.5f;
                float recTop = (Screen.currentResolution.height - recHeight) * 0.5f;

                var wizWnd = GetWindowWithRect<WizardOverview>(new Rect(recLeft, recTop, recWidth, recHeight), true, "Create customized 3D-model with built-in animation");
                AllWindows.Add(wizWnd);
            }
		}

        private static void OnLoadAudio()
        {
 
            try
            {

                ShowProNote();

            }
            catch (UnityException exp)
            {
                Debug.LogError(exp.Message);
            }
        }

        private static void OnExport()
        {

            try
            {
                ShowProNote();

            }
            catch (UnityException exp)
            {
                Debug.LogError(exp.Message);
            }
        }



        private static void OnAnimate()
        {
            if (Controller.ReadyToProceed == false)
            {
                EditorUtility.DisplayDialog("Error", NotProperlyMsg, "Ok");
            }
            else
            {
                if (Controller.Instance.IsFreeVersion)
                {
                    ShowProNote();
                }
            }

        }

        private static void OnRemember()
        {
            if (Controller.Instance.IsFreeVersion)
            {
                ShowProNote();
            }


        }

        private static void OnReset()
        {
            try
            { 
                Controller.Instance.ResetAnimation(null, null);
            }
            catch 
            {
  
            }
        }

        private static void ShowProNote()
        {
            
            float recWidth = 470;
            float recHeight = 310;
            float recLeft = (Screen.currentResolution.width - recWidth) * 0.5f;
            float recTop = (Screen.currentResolution.height - recHeight) * 0.5f;

            AllWindows.Add(GetWindowWithRect<NoteProWindow>(new Rect(recLeft, recTop, recWidth, recHeight), true, "Notes"));
        }


        private static void OnVisualizeAudio()
        {
            
            ShowProNote();
        }


		#endregion


		#region Sync


        private void InitializeValues()
        {
            try
            {
                _lipNarrow = Controller.Instance.LipNarrowWide;

            }
            catch (UnityException exp)
            {
                Debug.LogError(exp.Message);
            }
        }


		
		private void UpdateAnimation()
		{
			try
			{

				PoseConfigViewModel poseViewModel = UpdatePoseViewModel();
				Controller.Instance.UpdateAnimation(poseViewModel);
			}
			catch(UnityException exp)
			{
				Debug.LogError(exp.Message);
			}
		}

        private void UpdateEyeAnimation()
        {
            try
            {
                Controller.Instance.UpdateEyes(_eyeHorizontalSliderPosition, _eyeVerticalSliderPosition);

            }
            catch (UnityException exp)
            {
                Debug.LogError(exp.Message);
            }
        }

        private void UpdateEyeColor()
        {
            try
            {
                Controller.Instance.UpdateEyeColor(_EyeColorDropDownIndex);
            }
            catch (UnityException exp)
            {
                Debug.LogError(exp.Message);
            }
        }

        private void ResetPoseConfig(DropDownEnum dropDown)
		{
			try
			{
				

			}
			catch(UnityException exp)
			{
				Debug.LogError(exp.Message);
			}
		}

	

		private PoseConfigViewModel UpdatePoseViewModel()
		{
			PoseConfigViewModel poseConfigVMdl = new PoseConfigViewModel();

			try
			{
				poseConfigVMdl.BrowDropDownIndex = _browDropDownIndex;
				
				poseConfigVMdl.PrimaryExpressionDropDownIndex = _primaryExpressionDropDownIndex;
				
				poseConfigVMdl.MusclesDropDownIndex = _musclesDropDownIndex ;
				
				poseConfigVMdl.BrowSliderPosition = _browSliderPosition; 
				
				poseConfigVMdl.PrimaryExpressionSliderPosition = _primaryExpressionSliderPosition;

                poseConfigVMdl.EyeDirectionHorizontalSliderPosition = _eyeHorizontalSliderPosition;
                poseConfigVMdl.EyeDirectionVerticalSliderPosition = _eyeVerticalSliderPosition;

                poseConfigVMdl.LipNarrowWide = _lipNarrow; 
				poseConfigVMdl.JawOpenClose= _jawClose;
				
				poseConfigVMdl.MusclesSliderPosition = _musclesSliderPosition;
			}
			catch(UnityException exp)
			{
				Debug.LogError(exp.Message);
			}

			return poseConfigVMdl;
		}


        #endregion


        private bool CheckOS()
        {
            bool osSupported = true;

            osSupported = (Application.platform != RuntimePlatform.WindowsEditor);

            return osSupported;
        }



        private void Exit()
		{
			foreach(var window in AllWindows.Where(window => window != null))
			{
				window.Close();
			}
			if(WizardController.ActiveWindow != null)
			{
				WizardController.ActiveWindow.Close();
			}
			
			Close();
		}

		#endregion

	}
}