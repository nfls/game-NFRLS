using System.Linq;

using MassAnimation.Resources;

using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace Assets.Scripts.NFScript
{

    public class MeshGenerator
    {

        #region constants

        const string TextureFolder = "Assets/NF3DFaceAnimFree32Mac/_Textures/";
        const string MaterialsFolder = "Assets/NF3DFaceAnimFree32Mac/_Materials/";
        const string DefaultEyeColor = "brown01";

        internal const string LeftEye = "EyeLeft";
        internal const string RightEye = "EyeRight";

        #endregion


        #region members

        public GameObject AnimatableMeshPrefab;

        public GameObject EyeLeftPrefab;
        public GameObject EyeRightPrefab;

        public GameObject EyeLeft;
        public GameObject EyeRight;


        #endregion


        #region constructors

        public MeshGenerator()
	    {
            try
            {
#if UNITY_EDITOR
                AnimatableMeshPrefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/NF3DFaceAnimFree32Mac/_Prefabs/AnimatableMesh.prefab", typeof(GameObject));
                EyeLeftPrefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/NF3DFaceAnimFree32Mac/_Prefabs/eyeLeft.prefab", typeof(GameObject));
                EyeRightPrefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/NF3DFaceAnimFree32Mac/_Prefabs/eyeRight.prefab", typeof(GameObject));
#endif
            }
            catch (Exception exp)
            {
                Debug.Log(exp);
            }

        }

        #endregion


        #region methods

        public GameObject GenerateGameObjects(MassAnimation.Avatar.Entities.AnimatableUnity model)
	    {
		    var result = new GameObject("Model");

            try
            {
                var ipm = result.AddComponent<IntuitiveProModel>();
		        foreach (var shape in model.ModelShapes)
		        {
                    string shapeId = shape.ComponentId;

                    if (!shapeId.Contains(FaceComponentSources.Eye))
                    {
                        GenerateGameObject(shape, result);
                    }
                    else
                    {
                        GenerateEyeGameObject(shape, result);
                    }
                }
		        ipm.CacheModelShapeUpdater();
            }
            catch (Exception exp)
            {
                Debug.Log(exp);
                throw;
            }
            return result;
	    }


        private Material CreateNewMaterial(Material shared, string texName)
	    {
            Material material = null;

            try
            {

                material = new Material(shared);
                var name = texName;

                Texture2D tex2D = LoadTexture(texName);
#if UNITY_EDITOR
                AssetDatabase.CreateAsset(material, MaterialsFolder + name + ".mat");
#endif
                material.mainTexture = tex2D;
            }
            catch(System.Exception exp)
            {
                Debug.Log(exp.Message);
            }

		    return material;
	    }


        private Texture2D LoadTexture(string texName)
        {
            Texture2D tex = null;

#if UNITY_EDITOR
            tex = AssetDatabase.LoadAssetAtPath(TextureFolder + texName, typeof(Texture2D)) as Texture2D;
#endif

            return tex;
        }

        private void GenerateGameObject(MassAnimation.Avatar.Entities.ShapeUnity shape, GameObject parent)
	    {
		    try
		    {
			    var go = (GameObject)GameObject.Instantiate(AnimatableMeshPrefab);
			    go.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector | HideFlags.NotEditable;
			    go.transform.parent = parent.transform;
			    GenerateMaterial(shape, go);
			    GenerateMesh(shape, go);
		    }
		    catch(UnityException exp)
		    {
			    Debug.LogError(exp.Message);
		    }
	    }


        private void GenerateEyeGameObject(MassAnimation.Avatar.Entities.ShapeUnity shape, GameObject parent)
        {

            try
            {
                GameObject go = (GameObject)GameObject.Instantiate(AnimatableMeshPrefab); ;

                go.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector | HideFlags.NotEditable;
                go.transform.parent = parent.transform;
                GenerateEyeMesh(shape, go);
            }
            catch (Exception exp)
            {
                Debug.Log(exp);
                throw;
            }
        }


        private void GenerateMaterial(MassAnimation.Avatar.Entities.ShapeUnity shape, GameObject go)
        {
            if (null != shape.TextureEntity)
            {
                var texName = shape.TextureEntity.ImageName;
                go.GetComponent<Renderer>().material = CreateNewMaterial(go.GetComponent<Renderer>().sharedMaterial, texName);
            }
        }

        private void GenerateMesh(MassAnimation.Avatar.Entities.ShapeUnity shape, GameObject go)
        {
            if (null != shape.GeoModel)
            {
                var indices = shape.GeoModel.Indices;
                var vertices = shape.GeoModel.Vertices;

                var mesh = new Mesh();
                var mf = go.GetComponent<MeshFilter>();
                mf.mesh = mesh;

                mesh.vertices = vertices.AllPoints.Select(p => new Vector3(p.X, p.Y, p.Z)).ToArray();

                mesh.uv = shape.TexureMapping.UVs.AllPoints.Select(uv => new Vector2(uv.X, uv.Y)).ToArray();

                mesh.triangles = indices.AllCoordIndices.SelectMany(idx => idx).ToArray();

                mesh.RecalculateNormals();

                var umfs = go.GetComponent<UpdateMeshFromShape>();
                umfs.SetShape(shape);
            }
        }


        private void GenerateEyeMesh(MassAnimation.Avatar.Entities.ShapeUnity shape, GameObject go)
        {

            try
            {
                if (null != shape.GeoModel)
                {

                    var indices = shape.GeoModel.Indices;
                    var vertices = shape.GeoModel.Vertices;

                    if ((vertices != null) && (indices != null))
                    {
                        int numOfVtx = shape.GeoModel.Vertices.Count;

                        Vector3[] eyeVertics = new Vector3[numOfVtx];

                        int vtxCnt = 0;
                        foreach (Vector3 v in vertices.AllPoints.Select(p => new Vector3(p.X, p.Y, p.Z)).ToArray())
                        {
                            eyeVertics[vtxCnt] = v;
                            vtxCnt++;
                        }

                        GameObject curGO = null;

                        if (string.Equals(shape.ComponentId, FaceComponentSources.LeftEye))
                        {
                            EyeLeft = GameObject.Instantiate(EyeLeftPrefab) as GameObject;

                            EyeLeft.transform.name = LeftEye;
                            curGO = EyeLeft;
                        }
                        else if (string.Equals(shape.ComponentId, FaceComponentSources.RightEye))
                        {
                            EyeRight = GameObject.Instantiate(EyeRightPrefab) as GameObject;

                            EyeRight.transform.name = RightEye;
                            curGO = EyeRight;
                        }

                        if (curGO != null)
                        {
                            try
                            {
                                UnityEngine.Texture eyeTex = Resources.Load(DefaultEyeColor) as UnityEngine.Texture;

                                curGO.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = eyeTex;
                            }
                            catch (Exception exp)
                            {
                                Debug.Log(exp);
                                throw;
                            }

                            Vector3 eyeCenterSum = new Vector3(0, 0, 0);
                            foreach (Vector3 v in eyeVertics)
                            {
                                eyeCenterSum += v;
                            }

                            float eyeSizeSum = 0;
                            foreach (Vector3 v in eyeVertics)
                            {
                                eyeSizeSum += Vector3.Distance(v, eyeCenterSum);
                            }

                            curGO.transform.position = eyeCenterSum / numOfVtx / 100;

                            float eyeSize = eyeSizeSum / numOfVtx / 200;

                            curGO.transform.localScale = new Vector3(eyeSize, eyeSize, eyeSize);

                        }

                    }
                }
            }
            catch (Exception exp)
            {
                Debug.Log(exp);
                throw;
            }
        }



        #endregion

    }

}