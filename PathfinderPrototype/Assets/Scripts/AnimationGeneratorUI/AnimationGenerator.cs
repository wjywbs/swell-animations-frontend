using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using swellanimations;

[AddComponentMenu("Animation/Animation Generator")]
[System.Serializable]
public class AnimationGenerator : MonoBehaviour
{
    public int widthLines = 100;
    public int heightLines = 100;
    public float cellWidth = 32.0f;
    public float cellHeight = 32.0f;

    [SerializeField]
    public GameObject model;

    [SerializeField]
    private List<Vector3> points = new List<Vector3>();

    public Vector3 planeOrigin = new Vector3();
    public Vector3 planeVector1 = new Vector3();
    public Vector3 planeVector2 = new Vector3();

    [SerializeField]
    public Transform planePoint1;
    [SerializeField]
    public Transform planePoint2;
    [SerializeField]
    public Transform planePoint3;
    public Plane editorPlane;

    private int currentFrame = 0;
    private float timeBetweenFrames = 0.1f;

    void OnDrawGizmos()
    {
        DrawGrid();
        Gizmos.color = Color.blue;
        if(points.Count > 1)
        {
            for (int x = 1; x < points.Count; x++)
            {
                Gizmos.DrawLine(points[x - 1], points[x]);
            }
        }
    }

    void DrawGrid()
    {
        calculatePlaneVectors();
        Vector3 lineStartBase = (planeVector1 * heightLines / 2);
        Gizmos.color = Color.grey;
        for (int x = -widthLines / 2; x < widthLines / 2; x++)
        {
            Vector3 lineIncrementBase = ((planeVector2 * x) + planeOrigin);
            Vector3 lineStart = lineIncrementBase + lineStartBase;
            Vector3 LineEnd = lineIncrementBase - lineStartBase;
            Gizmos.DrawLine(lineStart, LineEnd);
        }
        lineStartBase = (planeVector2 * widthLines / 2);
        for (int y = -heightLines / 2; y < heightLines / 2; y++)
        {
            Vector3 lineIncrementBase = ((planeVector1 * y) + planeOrigin);
            Vector3 lineStart = lineIncrementBase + lineStartBase;
            Vector3 LineEnd = lineIncrementBase - lineStartBase;
            Gizmos.DrawLine(lineStart, LineEnd);
        }
    }

    public void addPoint(Vector3 point)
    {
        Debug.Log("Add point: " + point);
        points.Add(point);
    }

    public void clearPoints()
    {
        points.Clear();
    }

    public void calculatePlaneVectors()
    {
        Vector3 vectorA = planePoint2.position - planePoint1.position;
        Vector3 vectorB = planePoint3.position - planePoint1.position;
        Vector3 normal = Vector3.Cross(vectorA, vectorB);
        Vector3 perpVectorA = Vector3.Cross(vectorA, normal);
        planeOrigin = planePoint1.position;
        planeVector1 = vectorA.normalized;
        planeVector2 = perpVectorA.normalized;
        editorPlane = new Plane(planePoint1.position, planePoint2.position, planePoint3.position);
    }

    public Node GenerateNode()
    {
        Node node = new Node()
        {
            name = model.name,
            positionX = model.transform.position.x,
            positionY = model.transform.position.y,
            positionZ = model.transform.position.z,
            rotationX = model.transform.rotation.eulerAngles.x,
            rotationY = model.transform.rotation.eulerAngles.y,
            rotationZ = model.transform.rotation.eulerAngles.z
        };
        node.rotationX = 0;
        model.name.Contains("spine");
        model.GetComponentsInChildren<GameObject>();
        return node;
    }

    public IEnumerator AnimateFrame()
    {
        Node[] nodes = new Node[50];
        if(currentFrame < nodes.Length)
        {
            //code for setting the positions and rotations
            currentFrame++;
            yield return new WaitForSeconds(timeBetweenFrames);
        }
        else
        {
            yield break;
        }
    }

    public void PlayAnimation()
    {
        Debug.Log("Play Animation: Points: " + points.Count);
    }
}
