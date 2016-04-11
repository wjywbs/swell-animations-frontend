using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using swellanimations;
using System;

[AddComponentMenu("Animation/Animation Generator")]
[System.Serializable]
public class AnimationGenerator : MonoBehaviour
{
    public int widthLines = 100;
    public int heightLines = 100;
    public float cellWidth = 32.0f;
    public float cellHeight = 32.0f;
    public bool drawing = false;
    public float framesOfAnimation = 100f;
    public BezierSpline spline;

    [SerializeField]
    public Transform model;

    private Dictionary<string, Transform> modelMap = new Dictionary<string, Transform>();
    private Dictionary<Transform, Vector3> originalPositionMap = new Dictionary<Transform, Vector3>();
    private Dictionary<Transform, Quaternion> originalRotationMap = new Dictionary<Transform, Quaternion>();

    [SerializeField]
    private List<Vector3> points;

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

    private Node[] frames;

    public int currentFrame = 0;

    public float timeBetweenFrames;
    private float timeSinceLastFrame = 0.0f;

    public bool animationPlaying = false;
    private float m_LastEditorUpdateTime;

    [SerializeField]
    private Vector3 beginPostion;
    [SerializeField]
    private Quaternion beginRotation;
    public bool drawPlane = true;
    public bool drawLOA = true;

    [SerializeField]
    private string serializedAnimation;

    void OnDrawGizmos()
    {
        DrawGrid();
        if (drawLOA)
        {
            Gizmos.color = Color.blue;
            if (points.Count > 1)
            {
                for (int x = 1; x < points.Count; x++)
                {
                    Gizmos.DrawLine(points[x - 1], points[x]);
                }
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
        points.Add(point);
        drawLOA = true;
    }

    public void ClearPoints()
    {
        points.Clear();
        frames = null;
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
    }

    public void StopAnimation()
    {
        currentFrame = 0;
        animationPlaying = false;
        RestoreToOriginal(model);
    }

    public void RestoreToOriginal(Transform t)
    {
        t.position = originalPositionMap[t];
        t.rotation = originalRotationMap[t];
        foreach (Transform child in t)
        {
            RestoreToOriginal(child);
        }
    }

    public void UpdateAnimation(float deltaTime)
    {
        if (animationPlaying)
        {
            if (timeSinceLastFrame >= timeBetweenFrames)
            {
                if (currentFrame < points.Count)
                {
                    currentFrame++;
                    AnimateFrame();
                }
                else
                {
                    StopAnimation();
                }
                timeSinceLastFrame = 0;
            }
            else
            {
                timeSinceLastFrame += deltaTime;
            }
        }
    }

    public void FillModelMap(Transform loc)
    {
        modelMap.Add(loc.gameObject.name, loc);
        originalPositionMap.Add(loc, loc.position);
        originalRotationMap.Add(loc, loc.rotation);
        foreach (Transform t in loc)
        {
            FillModelMap(t);
        }
    }

    public void ClearMaps()
    {
        modelMap.Clear();
        originalPositionMap.Clear();
        originalRotationMap.Clear();
    }

    public void GenerateAnimation()
    {
        if (points != null && points.Count > 0)
        {
            GenerateSpline();
            beginPostion = model.position;
            beginRotation = model.rotation;
            currentFrame = 0;
            frames = BackendAdapter.GenerateFromBackend(AnimationData.CreateModelData(model, points));
            serializedAnimation = BackendAdapter.serializeNodeArray(frames);
            Debug.Log("Just serialized: " + serializedAnimation);
            ClearMaps();
            FillModelMap(model);
            for (int x = 0; x < frames.Length; x++)
            {
                if (x < frames.Length - 1)
                {
                    nextNodeMap.Clear();
                    mapNextNode(frames[x + 1]);
                }
                CalculateRotations(frames[x]);
            }
        }
    }

    public void GenerateSpline()
    {
        //The amount of points - 1 needs to be divisible by 3
        spline = new BezierSpline();
        spline.transform = transform;
        if ((points.Count - 1) % 3 != 0)
        {
            int remainder = (points.Count - 1) % 3;
            points.RemoveRange(points.Count - remainder, remainder);
        }
        spline.points = points.ToArray();
        spline.ComputeControlPoints();
        points.Clear();
        for (float t = 0f; t < 1; t += 1 / framesOfAnimation)
        {
            points.Add(spline.GetPoint(t));
        }
    }

    public void CalculateRotations(Node n)
    {
        Transform t = modelMap[n.name];
        Vector3 newPosition = new Vector3(
                n.position.x,
                n.position.y,
                n.position.z);
        Vector nextFrame = nextNodeMap[n.name].position;
        Vector3 nextPosition = new Vector3(nextFrame.x, nextFrame.y, nextFrame.z);
        Vector3 forward = (nextPosition - newPosition).normalized;
        Quaternion rotation = Quaternion.LookRotation(forward);
        n.eularAngles.x = rotation.eulerAngles.x;
        n.eularAngles.y = rotation.eulerAngles.y;
        n.eularAngles.z = rotation.eulerAngles.z;
        foreach (Node child in n.children)
        {
            CalculateRotations(child);
        }
    }

    
    Dictionary<string, Node> nextNodeMap = new Dictionary<string, Node>();
    public void mapNextNode(Node node)
    {
        nextNodeMap.Add(node.name, node);
        foreach (Node child in node.children)
        {
            mapNextNode(child);
        }
    }

    public bool CheckDistances(Transform t, Vector3 newPosition)
    {
        foreach (Transform o in modelMap.Values)
        {
            if (o != t && Vector3.Distance(newPosition, o.position) < Vector3.Distance(t.position, o.position))
            {
                return false;
            }
        }
        return true;
    }

    public void SetModel(Node n)
    {
        if (modelMap.ContainsKey(n.name))
        {
            Transform t = modelMap[n.name];
            
            //Vector3 newPosition = new Vector3(
            //        n.position.x,
            //        n.position.y,
            //        n.position.z);
            //if (CheckDistances(t, newPosition))
            //{
            //    t.position = newPosition;
                t.eulerAngles = new Vector3(
                    n.eularAngles.x,
                    n.eularAngles.y,
                    n.eularAngles.z
               );
            //}
            foreach (Node child in n.children)
            {
                SetModel(child);
            }
        }
        else
        {
            Debug.Log("oh shit! map doesn't contain " + n.name);
            return;
        }
    }

    public void RestoreAnimation()
    {
        if (frames == null && serializedAnimation != null && !drawing && points.Count > 0)
        {
            ClearMaps();
            FillModelMap(model);
            Debug.Log("Restored using: " + serializedAnimation);
            frames = BackendAdapter.deserializeNodeArray(serializedAnimation);
            Debug.Log("Restored: " + frames);
        }
    }

    public void AnimateFrame()
    {
        if (currentFrame < frames.Length)
        {
            Node n = frames[currentFrame];
            model.position = new Vector3(
                     n.position.x,
                     n.position.y,
                     n.position.z);
             SetModel(n);
            //AnimationData.PrintAllNodes(node,"-");
            //AnimationData.PrintAllTransforms(model, "-");
        }
    }

    public float GetFrameRate()
    {
        return 1 / timeBetweenFrames;
    }

    public int GetNumFrames()
    {
        return frames == null ? 0 : frames.Length;
    }
}
