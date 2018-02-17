using System.Collections;
using System.Linq;

using MassAnimation.Avatar.Entities;

using UnityEngine;

namespace Assets.Scripts.NFScript
{

    [ExecuteInEditMode]
    public class UpdateMeshFromShape : MonoBehaviour 
    {
	    MeshFilter mf;
        ShapeUnity shape;

	    public void SetShape(ShapeUnity shape)
	    {
		    this.shape = shape;
	    }

	    void Start() 
	    {
		    mf = this.GetComponent<MeshFilter>();
	    }

	    public void UpdateMesh() 
	    {
		    if (null == shape)
		    {
			    return;
		    }
		    var mesh = mf.sharedMesh;
		    var shapeVertices = shape.GeoModel.Vertices;

		    mesh.vertices = shapeVertices.AllPoints.Select(p => new Vector3(p.X, p.Y, p.Z)).ToArray();
		    mesh.RecalculateNormals();
	    }
    }

}