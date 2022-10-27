using Rhino;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RhinoInside
{
  [InitializeOnLoad]
  [ExecuteInEditMode]
  public class Cube : MonoBehaviour
  {
    public static void Create()
    {
      var self = new GameObject("Cube");
      self.AddComponent<LoftSurface>() ;
    }

    public static UnityEngine.Mesh CreateLoft(List<List<Vector3>> controlPoints)
    {
      if (controlPoints.Count > 0 )
      {
        var profileCurves = new List<Rhino.Geometry.Curve>();
        foreach(var controlPointsRow in controlPoints)
          profileCurves.Add(Rhino.Geometry.Curve.CreateInterpolatedCurve(controlPointsRow.ToRhino(), 3));

        return Rhino.Geometry.Mesh.CreateFromBrep(
                Rhino.Geometry.Brep.CreateFromLoft(
                    profileCurves,
                    Rhino.Geometry.Point3d.Unset,
                    Rhino.Geometry.Point3d.Unset,
                    Rhino.Geometry.LoftType.Normal,
                    false)[0], Rhino.Geometry.MeshingParameters.Default)[0].ToHost();
      }

      return null;
    }


    void Start()
    {
      gameObject.AddComponent<MeshFilter>();

      var material = new Material(Shader.Find("Standard"))
      {
        color = new Color(1.0f, 0.0f, 0.0f, 1f)
      };

      gameObject.AddComponent<MeshRenderer>().material = material;

    }

    void Update()
    {
      var controlPoints = new List<List<Vector3>>();
      {
        int i = 0;
        List<Vector3> controlPointsRow = null;
        foreach (Transform controlSphere in transform)
        {
          if ((i++ % VCount) == 0)
          {
            controlPointsRow = new List<Vector3>(VCount);
            controlPoints.Add(controlPointsRow);
          }

          controlPointsRow.Add(gameObject.transform.worldToLocalMatrix.MultiplyPoint(controlSphere.gameObject.transform.position));
        }
      }

      gameObject.GetComponent<MeshFilter>().mesh = CreateLoft(controlPoints);
    }
  }
}
