
using System;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.NFEditor
{

	internal sealed class WizardClickFrontal : EditorWindow, IWizardFeaturePointPage
	{

		#region constants
		
		#endregion
		
		
		#region private members

		private List<PickedPoint> _points;

		#endregion
			
			
		#region properties
	
		internal static Texture2D mainTexture;

		public List<PickedPoint> Points
		{
			get
			{
				return _points;
			}
		}

		public Texture ExampleTexture
		{
			get
			{
				return (Texture)Resources.Load("FrontImage_Point");
			}
		}		
		
		#endregion
		
		
		
		#region constructors
		
		internal WizardClickFrontal()
		{
            OnReset();
		}
		
		#endregion
		
		
		
		#region events & handlers
		
		
		#endregion
		
		
		#region public methods

		public void OnGUI()
		{
			try
			{
                var labelRect = new Rect((float)0 + 10, (float)0 + 10, 250, 20);
                GUI.Label(labelRect, "   Step 2 – click the points");

				WizardController.DrawFeaturePointsSection(this, mainTexture);
			}
			catch(UnityException exp)
			{
				Debug.LogError(exp.Message);
			}
		}
		
		public bool CheckPoints()
		{
			if (_points.Count == 11)
			{
				return true;
			}
			else
			{
				return false;
			}

		}
		
		public void OnNext()
		{
			try
			{
				if ( CheckPoints())
				{
					Controller.AddFrontImagePoints(_points);

                    float width = 450;
                    float height = 300;

                    float recLeft = position.xMin;
                    float recTop = position.yMin - 200;
                    float recWidth = width * 1.3f;
                    float recHeight = height * 0.8f;

                    WizardController.GoToPage<WizardChooseSpeedup>(this, new Rect(recLeft, recTop, recWidth, recHeight));

				}
			}
			catch(UnityException exp)
			{
				Debug.LogError(exp.Message);
			}
		}
		
		public void OnBack()
		{
			try
			{
				WizardController.GoToPage<WizardLoadPhotos>(this);
			}
			catch(UnityException exp)
			{
				Debug.LogError(exp.Message);
			}
		}
		
		public void OnReset()
		{
            _points = new List<PickedPoint>();
		}
		
		public int PointsNumber
		{
			get
			{
				return _points.Count;
			}
		}
		
		#endregion
		
		
		#region private methods
		
		
		
		#endregion
		

	}

}
