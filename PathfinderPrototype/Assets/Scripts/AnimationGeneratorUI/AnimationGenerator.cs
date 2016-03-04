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
    public Transform model;

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

    [SerializeField]
    private Node[] frames;

    private int currentFrame = 0;
    public float timeBetweenFrames = 0.1f;

    private bool animationPlaying = false;
    private float m_LastEditorUpdateTime;

    private Vector3 beginPostion;
    private Quaternion beginRotation;
    private bool drawPlane = true;

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
        if (drawPlane)
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

    public void ToggleAnimation()
    {
        animationPlaying = !animationPlaying;
        if (animationPlaying)
        {
            currentFrame = 0;
            beginPostion = model.position;
            beginRotation = model.rotation;
            drawPlane = false;
        }
        else
        {
            model.position = beginPostion;
            model.rotation = beginRotation;
            drawPlane = true;
        }
    }

    public void UpdateAnimation(float deltaTime)
    {
        if(animationPlaying && deltaTime >= timeBetweenFrames)
        {
            if (currentFrame < points.Count)
            {
                AnimateFrame(currentFrame);
                currentFrame++;
            }
            else
            {
                ToggleAnimation();
            }
        }
    }

    public void GenerateAnimation()
    {
        frames = TestData.CreateTestAnimation(AnimationData.GenerateNode(model), points);
    }

    public void AnimateFrame(int frame)
    {
        //You wold want to use the frame number to get the correct fraome
        //ex: Node node = frames[frame];

        //This code I have here just moves the model to each point on the line, obviously not what we want in the final version
        Node node = frames[frame];
        model.position = points[currentFrame];
        model.position = new Vector3(0f, 0f, 0f);
        model.rotation = Quaternion.Euler(0f, 0f, 0f);
        if (currentFrame + 1 < points.Count)
        {
            model.rotation = Quaternion.LookRotation((points[currentFrame + 1] - model.position).normalized);
        }

    }


}
