using System;
using System.Collections.Generic;
using System.IO;

using MassAnimation.Adapters.PhotoAdapter;
using MassAnimation.Avatar.Entities;
using MassAnimation.Resources;
using MassAnimation.Resources.Entities;
using MassAnimation.Modeling;


using UnityEngine;

namespace Assets.Scripts.NFEditor
{

    public class ModelConnector 
	{

		public string _frontImgPath; 

		internal ModelConnector ()
		{
		}

		internal ModelConnector (string frontalImagePath)
		{
			_frontImgPath = frontalImagePath;
		}	


	    internal Animatable BuildAvatarFromFrontImage (Point[] pointLocations )
		{
			Animatable anim = null;
			
			try
			{

				if ( (_frontImgPath == null) || (!File.Exists(_frontImgPath))  )
				{
					return anim;
				}

			    Texture2D frontImg = FGCVT.FimPngIo.loadTexture2DFromPngOrJpgFile (_frontImgPath);
				
			    MassAnimation.Resources.Tuple<Texture2D, Point[]> imgPtPair = 
			        new MassAnimation.Resources.Tuple<Texture2D, Point[]> (
					    frontImg, pointLocations);
				
				MassAnimation.Resources.Tuple<Texture2D, Point[]>[] mdls = 
				    new MassAnimation.Resources.Tuple<Texture2D, Point[]>[1];
				
				mdls[0]= imgPtPair;
				
				string outputPath = ResourceDirectories.TempModelDirectory;
				
				AvatarAdapter avatarAdapter = null;
				
				try
				{
					
					avatarAdapter = new AvatarAdapter("avatar",                                                 
					                                  outputPath,                                                  
					                                  mdls,
					                                  ModelDensity.HIGH,
					                                  "",
					                                  false);
				}
				catch(Exception adExp)
				{
					Debug.LogException(adExp);
				}
				
				if (avatarAdapter != null)
				{
                    int speedUp = 1;
					IAvatar avatar = avatarAdapter.Run("", EyeColor.Brown, speedUp);
					anim = avatar.ToAnimatable();
				}
				
				
			}
			catch(UnityException exp)
			{
				Debug.LogException(exp);
			}
			
			return anim;
			
		}

		internal Animatable BuildAvatarFromFrontImage(string frontImagePath, Point[] pointLocations, EyeColor eyeColor, int speedUp )
		{
			Animatable anim = null;

			try
			{

			    Texture2D frontImg = FGCVT.FimPngIo.loadTexture2DFromPngOrJpgFile (frontImagePath);
				
				MassAnimation.Resources.Tuple<Texture2D, Point[]> imgPtPair = 
					new MassAnimation.Resources.Tuple<Texture2D, Point[]>(frontImg, pointLocations);
				
				MassAnimation.Resources.Tuple<Texture2D, Point[]>[] mdls = 
					new MassAnimation.Resources.Tuple<Texture2D, Point[]>[1];
				
				mdls[0]= imgPtPair;
				
				string outputPath = ResourceDirectories.TempModelDirectory;
				
				AvatarAdapter avatarAdapter = null;
				
				try
				{
					
					avatarAdapter = new AvatarAdapter("avatar",                                                 
					                                  outputPath,                                                  
					                                  mdls,
					                                  ModelDensity.HIGH,
					                                  "",
					                                  false);
				}
				catch(Exception adExp)
				{
					Debug.LogException(adExp);
				}
				
				if (avatarAdapter != null)
				{
                    IAvatar avatar = null;

                    try
                    {
                        avatar = avatarAdapter.Run("", eyeColor, speedUp);
                    }
                    catch (AvatarGenerationException agExp)
                    {
                        string fitErrorMsg = null;
                                  
                        if (string.Equals(agExp.Message, "User fiducial points not properly placed.", StringComparison.InvariantCultureIgnoreCase))
                        {
                            fitErrorMsg = "Sorry, you did not position the points properly. Please follow the demo videos exactly." ;
                        }

                        throw new ApplicationException(fitErrorMsg);

                    }

                    if (avatar != null)
                    {
                        anim = avatar.ToAnimatable();
                    }
				}
				
                if (anim != null)
                {
                   
                }

			}
			catch(Exception exp)
			{
				Debug.LogException(exp);
                throw;
			}

			return anim;

		}


	}

}