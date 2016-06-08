using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using swellanimations;
using System;
using UnityEditor;

[AddComponentMenu("Animation/Animation Generator")]
[System.Serializable]
public class AnimationGenerator : MonoBehaviour
{
    public const float SELECT_RANGE = 3.0f;
    public const float ROTATION_POINT_RADIUS = 1;
    public const string FILE_LINE_SEPARATOR = "swellanimationsisthebesteverlonglivebun";

    public int mutatorStrength = 25;
    public bool forceRestore = false;

    public int widthLines = 10000;
    public int heightLines = 10000;
    public float cellWidth = 3200.0f;
    public float cellHeight = 3200.0f;
    public bool drawingLOA = false;
    public bool editingLOA = false;
    public bool detailingLOA = false;
    public bool addingRotationPoint = false;
    public bool deletingRotationPoint = false;
    public int framesOfAnimation = 100;
    public int currentDetailLOA = 0;

    public RotationPoint rotationPointToDelete;

    private int editStartIndex = 0;
    private int editEndIndex = 0;
    private List<Vector3> editPoints = new List<Vector3>();

    [SerializeField]
    public Transform model;

    private Dictionary<string, Transform> modelMap = new Dictionary<string, Transform>();
    private Dictionary<Transform, Vector3> originalPositionMap = new Dictionary<Transform, Vector3>();
    private Dictionary<Transform, Quaternion> originalRotationMap = new Dictionary<Transform, Quaternion>();

    [SerializeField]
    public List<Vector3> points;

    [SerializeField]//FIXME: @Angel "Do I need this?" -- Carlo
    public List<List<Vector3>> detailLoaPoints = new List<List<Vector3>>();

    [SerializeField]
    public List<RotationPoint> rotationPoints;

    public Vector3 planeOrigin = new Vector3();
    public Vector3 planeVector1 = new Vector3();
    public Vector3 planeVector2 = new Vector3();
    public Vector3 upVector = new Vector3();

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

    public bool drawPlane = true;
    public bool renderLOA = true;
    public bool smoothCurve = true;

    [SerializeField]
    public string serializedAnimation;

    public int selectedRotationPointIndex = 0;

    public Vector3 mouseLocation = new Vector3();

    public bool dirty = false;

    void OnDrawGizmos()
    {
        DrawGrid();
        DrawLOA();
        DrawEditLine();
        DrawRotationPoint();
        DrawDetailLines();
        //DrawPrimes();
        //DrawVectors(model);
        DrawPlaneVectors();
    }

    void DrawVectors(Transform trans)
    {
        foreach (Transform t in trans)
        {
            if (t.name.Contains("spine"))
            {
                Gizmos.color = Color.green;
                Gizmos.DrawRay(t.position, t.forward * 5);
                Gizmos.color = Color.red;
                Gizmos.DrawRay(t.position, t.up * 5);
                Gizmos.color = Color.yellow;
                Gizmos.DrawRay(t.position, (t.up + t.forward).normalized * 5);
            }
            DrawVectors(t);
        }
    }

    void DrawPlaneVectors()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(planeVector1, planeVector1 * 5);
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(planeVector2, planeVector2 * 5);
    }

    void DrawPrimes()
    {
        if (frames == null || currentFrame >= frames.Length)
            return;
        Node frame = frames[currentFrame];
        System.Random rand = new System.Random();
        while (frame.children.Count > 0)
        {
            Vector3 tangent = new Vector3(
                                  (float)frame.eularAngles.x,
                                  (float)frame.eularAngles.y,
                                  (float)frame.eularAngles.z
                              );

            Vector3 position = new Vector3()
            {
                x = (float)frame.position.x,
                y = (float)frame.position.y,
                z = (float)frame.position.z
            };
            Gizmos.color = new Color((float)rand.NextDouble(), (float)rand.NextDouble(), (float)rand.NextDouble());
            Gizmos.DrawSphere(tangent + position, .1f);
            Gizmos.DrawSphere(position, .1f);
            frame = frame.children[0];
        }
    }

    void DrawEditLine()
    {
        if (editingLOA)
        {
            DrawLine(editPoints, Color.red);
        }
    }

    void DrawLOA()
    {
        if (renderLOA)
        {
            DrawLine(points, Color.blue);
        }
    }

    void DrawDetailLines()
    {
        for (int i = 0; i < detailLoaPoints.Count; ++i)
        {
            DrawLine(detailLoaPoints[i], Color.red);
        }
    }

    // Returns a position between 4 Vector3 with Catmull-Rom Spline algorithm
    // http://www.iquilezles.org/www/articles/minispline/minispline.htm
    // http://www.habrador.com/tutorials/catmull-rom-splines/
    Vector3 GetCatmullRomPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        Vector3 a = 0.5f * (2f * p1);
        Vector3 b = 0.5f * (p2 - p0);
        Vector3 c = 0.5f * (2f * p0 - 5f * p1 + 4f * p2 - p3);
        Vector3 d = 0.5f * (-p0 + 3f * p1 - 3f * p2 + p3);

        Vector3 pos = a + (b * t) + (c * t * t) + (d * t * t * t);
        return pos;
    }

    // Returns a list of points of the Catmull-Rom curve.
    // The first and last point will be dropped by the algorithm.
    List<Vector3> GetCatmullRomCurve(List<Vector3> line)
    {
        List<Vector3> curve = new List<Vector3>();
        for (int x = 1; x < line.Count - 2; x++)
        {
            Vector3 p0 = line[x - 1], p1 = line[x];
            Vector3 p2 = line[x + 1], p3 = line[x + 2];

            float step = 0.2f / Vector3.Distance(p1, p2);
            step = step > 0.1f ? 0.1f : step;

            // t is always between 0 and 1 and determines the resolution of the spline.
            // 0 is always at p1.
            for (float t = 0; t < 1; t += step)
            {
                // Find the coordinates between the control points with a Catmull-Rom spline.
                curve.Add(GetCatmullRomPoint(t, p0, p1, p2, p3));
            }
            // Add the last point.
            curve.Add(p2);
        }
        return curve;
    }

    void DrawLine(List<Vector3> line, Color color)
    {
        if (line == null || line.Count <= 1)
            return;

        Gizmos.color = color;
        if (smoothCurve && line.Count >= 4)
        {
            line = GetCatmullRomCurve(line);
        }
        for (int x = 1; x < line.Count; x++)
        {
            Gizmos.DrawLine(line[x - 1], line[x]);
        }
    }

    public void DrawRotationPoint()
    {
        if (rotationPoints == null || rotationPoints.Count < 1)
        {
            return;
        }

        foreach (RotationPoint rotPoint in rotationPoints)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(rotPoint.position, ROTATION_POINT_RADIUS);

            Vector3 prevPosition = model.position;
            Quaternion prevRotation = model.rotation;

            model.Translate(rotPoint.position);
            model.Rotate(rotPoint.rotation.eulerAngles);

            // Gizmos.color = Color.green;
            // Gizmos.DrawLine(rotPoint.position, (model.up * 1.1f + rotPoint.position));
            Handles.ArrowCap(0, rotPoint.position, rotPoint.rotation * Quaternion.Euler(-90, 0, 0), 2 * ROTATION_POINT_RADIUS);

            model.position = prevPosition;
            model.rotation = prevRotation;
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

    public void AddPoint(Vector3 point)
    {
        if (points == null)
        {
            points = new List<Vector3>();
        }
        points.Add(point);
        renderLOA = true;
    }

    public void AddDetailPoint(Vector3 point)
    {
        if (detailLoaPoints.Count == currentDetailLOA)
        {
            detailLoaPoints.Add(new List<Vector3>());
        }
        Debug.Assert(currentDetailLOA < detailLoaPoints.Count);
        detailLoaPoints[currentDetailLOA].Add(point);
    }

    public void ClearDetailPoints()
    {
        detailLoaPoints.Clear();
        currentDetailLOA = 0;
    }

    public void ClearPoints()
    {
        points.Clear();
        frames = null;
    }

    public void AddRotationPoint(Vector3 mouseLocation)
    {
        int index;
        if (rotationPoints == null)
        {
            rotationPoints = new List<RotationPoint>();
        }
        Vector3 closestPoint = FindClosestIntersect.Search(points, mouseLocation, out index);
        // The new point should at the middle of the 2 points.
        index++;

        RotationPoint rotationPoint = new RotationPoint();
        rotationPoint.position = closestPoint;
        rotationPoint.rotation = Quaternion.identity;
        rotationPoint.index = index;
        rotationPoint.range = mutatorStrength;
        rotationPoints.Add(rotationPoint);
        points.Insert(index, closestPoint);
        addingRotationPoint = false;
        foreach (RotationPoint rotPoint in rotationPoints)
        {
            if (rotPoint.index > index)
            {
                rotPoint.index++;
            }
        }
        GenerateAnimation();
    }

    public void ClearRotationPoints()
    {
        if (rotationPoints != null)
        {
            rotationPoints.Clear();
        }
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
        upVector = -planeVector1;
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
                if (currentFrame < frames.Length)
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
            currentFrame = 0;
            List<Vector3> line = points;
            if (smoothCurve && points.Count >= 4)
            {
                line = GetCatmullRomCurve(line);
            }
            ModelData modelData = AnimationData.CreateModelData(model, line, rotationPoints,
                                      detailLoaPoints, framesOfAnimation, mutatorStrength);
            swellanimations.Animation animation = BackendAdapter.GenerateFromBackend(modelData);
            frames = animation.frames.ToArray();
            serializedAnimation = BackendAdapter.serializeNodeArray(frames);
            //Debug.Log("Animation generated");
            ClearMaps();
            FillModelMap(model);
        }
    }

    public void SetModel(Node n)
    {
        if (modelMap.ContainsKey(n.name))
        {
            Transform t = modelMap[n.name];
            SetNodePose(n, t, true);
            foreach (Node child in n.children)
            {
                SetModel(child);
            }
        }
    }

    public void SetNodePose(Node n, Transform t, Boolean check)
    {
        Vector3 position = new Vector3();
        position.x = (float)n.position.x;
        position.y = (float)n.position.y;
        position.z = (float)n.position.z;
        t.position = position;
        if (n.eularAngles != null)
        {
            //The backend is not really returning the Euler angles, but instad a position that we must look at.
            Vector3 eulerAngles = new Vector3(
                                      (float)n.eularAngles.x,
                                      (float)n.eularAngles.y,
                                      (float)n.eularAngles.z
                                  );
            t.LookAt(eulerAngles + position, upVector);
        }
    }

    public void RestoreAnimation()
    {
        if (forceRestore || (frames == null && serializedAnimation != null && !drawingLOA && points != null && points.Count > 0))
        {
            ClearMaps();
            FillModelMap(model);
            frames = BackendAdapter.deserializeNodeArray(serializedAnimation);
            Debug.Log("Restored: " + frames.Length + " frames");
        }

    }

    public void AnimateFrame()
    {
        if (currentFrame < frames.Length)
        {
            Node n = frames[currentFrame];
            if (n.rotation != null)
            {
                upVector = Quaternion.Euler(new Vector3((float)n.rotation.x, (float)n.rotation.y, (float)n.rotation.z)) * upVector;
            }
            else if (currentFrame == 0)
            {
                upVector = -planeVector1;
            }
            
            SetNodePose(n, model, false);
            SetModel(frames[currentFrame]);
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

    public void EditStart(Vector3 point)
    {
        if (points.Count > 0)
        {
            editPoints.Clear();
            Vector3 pointOnLine = FindClosestIntersect.Search(points, point, out editStartIndex);
            if (Vector3.Distance(pointOnLine, point) < SELECT_RANGE)
            {
                editPoints.Add(pointOnLine);
            }
            else
            {
                editingLOA = false;
            }
        }
        else
        {
            editingLOA = false;
        }
    }

    public void EditEnd(Vector3 point)
    {
        Vector3 pointOnLine = FindClosestIntersect.Search(points, point, out editEndIndex);
        editPoints.Add(pointOnLine);
        if (editStartIndex > editEndIndex)
        {
            int temp = editStartIndex;
            editStartIndex = editEndIndex;
            editEndIndex = temp;
            editPoints.Reverse();
        }
        List<RotationPoint> newRotPointList = new List<RotationPoint>();
        foreach (RotationPoint rotPoint in rotationPoints)
        {
            if (rotPoint.index < editStartIndex || rotPoint.index > editEndIndex)
            {
                newRotPointList.Add(rotPoint);
            }
        }
        rotationPoints = newRotPointList;
        points.RemoveRange(editStartIndex, editEndIndex - editStartIndex + 1);
        points.InsertRange(editStartIndex, editPoints);
        editingLOA = false;
        editPoints.Clear();
    }

    public void AddEditPoint(Vector3 point)
    {
        editPoints.Add(point);
    }

    public RotationPoint getClosetestRotationPoint(Vector3 point)
    {
        RotationPoint closetRotPoint = rotationPoints[0];
        float closetsDistance = Vector3.Distance(point, closetRotPoint.position);
        if (rotationPoints != null)
        {
            foreach (RotationPoint rotPoint in rotationPoints)
            {
                float dist = Vector3.Distance(rotPoint.position, point);
                if (dist < closetsDistance)
                {
                    closetsDistance = dist;
                    closetRotPoint = rotPoint;
                }
            }
        }
        return closetRotPoint;
    }

    public void Export()
    {
        string fileName = EditorUtility.SaveFilePanelInProject("Export animation to file",
                              model.name + "_animation.txt",
                              "txt",
                              "Please enter a file name to save the animation to");
        if (fileName == string.Empty)
        {
            return;
        }
        var sr = File.CreateText(fileName);
        sr.Write(serializedAnimation);
        sr.Write(FILE_LINE_SEPARATOR);
        sr.Write(planeVector1.x + "," + planeVector1.y + "," + planeVector1.z);
        sr.Close();
    }
}
