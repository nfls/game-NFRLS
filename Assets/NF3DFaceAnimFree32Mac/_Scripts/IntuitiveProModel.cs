using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Assets.Scripts.NFScript
{

    [ExecuteInEditMode]
    public class IntuitiveProModel : MonoBehaviour
    {
        public List<UpdateMeshFromShape> modelShapeUpdater = new List<UpdateMeshFromShape>();

        void Start()
        {
   
        }

        void Awake()
        {

        }

        public void Update()
        {

        }

        public void CacheModelShapeUpdater()
        {
            modelShapeUpdater.AddRange(GetComponentsInChildren<UpdateMeshFromShape>());
        }

        public void UpdateMeshes()
        {
            foreach (var msu in modelShapeUpdater)
            {
                msu.UpdateMesh();
            }
        }

        public void OnEditorPlay()
        {

        }

        public void OnEditorPaused()
        {

        }

        public void OnEditorStop()
        {
 
        }

        public void OnSceneSaved()
        {

        }

        public void OnProjectLoad()
        {
 
        }
    }


}
