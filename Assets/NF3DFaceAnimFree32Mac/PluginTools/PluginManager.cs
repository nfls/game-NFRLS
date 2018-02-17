using System;

using MassAnimation.Avatar.Entities;
using MassAnimation.AvatarService.AvatarResource;
using MassAnimation.Resources;
using MassAnimation.Resources.Entities;
using MassAnimation.UnityPluginConnector;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Assets.Scripts.NFScript
{

    [ExecuteInEditMode]
    public class PluginManager : MonoBehaviour
    {


        #region members & properties

        PluginConnector _plugin;
		volatile bool animationRunning;

        public GameObject eyeLeft;
        public GameObject eyeRight;

        internal static PluginManager instance;
        public static PluginManager Instance 
		{
			get 
			{
                if (instance == null)
                {
                    instance = (PluginManager)GameObject.FindObjectOfType<PluginManager>();
                    if (instance == null)
                    {
                        instance = new GameObject().AddComponent<PluginManager>();
                        instance.name = "PluginManager";
                        instance.Initialize();
                    }
                }
                return instance;
            }
			set
			{
				instance = value;
			}
		}

        private IntuitiveProModel currentModel;
        public IntuitiveProModel CurrentModel 
		{
			get 
			{
				return currentModel;
			}
			private set 
			{
				currentModel = value;
			}
		}

        public AnimatableUnity Model 
		{ 
			get 
			{ 
				return _plugin.Model;  
			}
			set
			{
				_plugin.Model = value;
			}
		}

        private bool _noActionDisplayed;
        public bool NoActionDisplayed
        {
            get
            {
                return _noActionDisplayed;
            }
            set
            {
                _noActionDisplayed = value;
            }
        }

        #endregion


        #region events

        public event EventHandler<AvatarEventArgs> AvatarUpdated;

        private void OnAvatarUpdated(AvatarEventArgs e)
        {
            EventHandler<AvatarEventArgs> temp = AvatarUpdated;

            if (temp != null)
            {
                temp(this, e);
            }
        }

        #endregion


        #region methods

        private void Initialize() 
		{
			_plugin = PluginConnector.Instance;

            AvatarUpdated += UIController.OnAvatarUpdated;

#if UNITY_EDITOR
            EditorApplication.update += Update;
#endif

            _noActionDisplayed = false;

        }

        public void Update()
		{
			if (null == CurrentModel || !animationRunning)
			{
				return;
			}
			
			CurrentModel.UpdateMeshes();
			
		}

        public void ChangeEyes(float eyeHorizontalMovement, float eyeVerticalMovement)
        {
            try
            {

                if (_plugin != null)
                {
                    animationRunning = true;

                    eyeLeft = GameObject.Find(MeshGenerator.LeftEye);
                    eyeLeft.transform.rotation = Quaternion.Euler(70 - eyeVerticalMovement / 5, eyeHorizontalMovement / 4, 180);
                    eyeRight = GameObject.Find(MeshGenerator.RightEye);
                    eyeRight.transform.rotation = Quaternion.Euler(70 - eyeVerticalMovement / 5, eyeHorizontalMovement / 4, 180);

                }
            }
            catch (UnityException exp)
            {
                UnityEngine.Debug.LogError(exp.Message);
            }
        }

        public void ChangeEyeColor(int EyeColorIndex)
        {
            try
            {
                if (_plugin != null)
                {
                    animationRunning = true;

                    eyeLeft = GameObject.Find(MeshGenerator.LeftEye);
                    eyeRight = GameObject.Find(MeshGenerator.RightEye);

                    switch (EyeColorIndex)
                    {
                        case 0:
                            eyeLeft.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = Resources.Load("brown01") as UnityEngine.Texture;
                            eyeRight.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = Resources.Load("brown01") as UnityEngine.Texture;
                            break;
                        case 1:
                            eyeLeft.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = Resources.Load("blue01") as UnityEngine.Texture;
                            eyeRight.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = Resources.Load("blue01") as UnityEngine.Texture;
                            break;
                        case 2:
                            eyeLeft.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = Resources.Load("gray01") as UnityEngine.Texture;
                            eyeRight.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = Resources.Load("gray01") as UnityEngine.Texture;
                            break;
                        case 3:
                            eyeLeft.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = Resources.Load("hazel01") as UnityEngine.Texture;
                            eyeRight.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = Resources.Load("hazel01") as UnityEngine.Texture;
                            break;
                        default:
                            eyeLeft.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = Resources.Load("brown01") as UnityEngine.Texture;
                            eyeRight.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = Resources.Load("brown01") as UnityEngine.Texture;
                            break;
                    }

                }
            }
            catch (UnityException exp)
            {
                UnityEngine.Debug.LogError(exp.Message);
            }
        }


        public void ChangePose(PoseConfigViewModel poseViewModel, ControlLever ctrlLever)
		{
			try
			{

				if (_plugin != null)
				{
                    animationRunning = true;
                    
                    _plugin.Pose(poseViewModel, ctrlLever);

				}
			}
			catch(UnityException exp)
			{
				UnityEngine.Debug.LogError(exp.Message);
			}
		}

		
        public bool IsAnimating()
		{
			return animationRunning;
		}
		
        public bool HasGeneratedModel()
		{
			return CurrentModel != null;
		}
		
        public void GenerateModel()
		{
            try
            {
                MeshGenerator gen = new MeshGenerator();
			    var go = gen.GenerateGameObjects(PluginManager.Instance.Model);
			    CurrentModel = go.GetComponent<IntuitiveProModel>();
            }
            catch (Exception exp)
            {
                Debug.Log(exp);
                throw;
            }
        }

        #endregion


        public void ProcessTexture()
        {
            string texPath = ResourceDirectories.TempModelDirectory;
            AvatarResourceManager.CopyTexture(texPath);

        }


    }

}