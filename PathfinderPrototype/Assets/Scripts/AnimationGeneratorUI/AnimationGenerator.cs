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
        Node node = CreateNodeFromGameObject(model);
        GenerateChildren(transform, node);
        return node;
    }

    private Node CreateNodeFromGameObject(Transform transform)
    {
        return new Node()
        {
            name = transform.gameObject.name,
            positionX = transform.position.x,
            positionY = transform.position.y,
            positionZ = transform.position.z,
            rotationX = transform.rotation.eulerAngles.x,
            rotationY = transform.rotation.eulerAngles.y,
            rotationZ = transform.rotation.eulerAngles.z
        };
    }

    private void GenerateChildren(Transform children, Node parent)
    {
        foreach (Transform transform in children)
        {
            Node child = CreateNodeFromGameObject(transform);
            child.parent = parent;
            parent.children.Add(child);
            GenerateChildren(transform, child);
        }
    }

    public IEnumerator AnimateFrame(Node[] nodes)
    {
        while(currentFrame < nodes.Length)
        {
            Node currentNode = nodes[currentFrame];
            //code for setting the positions and rotations
            Debug.Log(currentFrame);
            currentFrame++;
            yield return new WaitForSeconds(timeBetweenFrames);
        }
        yield break;        
    }

    public void PlayAnimation()
    {
        Debug.Log("Play Animation: Points: " + points.Count);
        Node node = GenerateNode();
        PrintAllNodes(node, "|");
    }


    //The following are used for debug purposes
    private void PrintAllNodes(Node node, string spacing)
    {
        Debug.Log(spacing + PrintNode(node)); ;
        foreach (Node childNode in node.children)
        {
            PrintAllNodes(childNode, spacing + "-");
        }

    }

    private string PrintNode(Node node)
    {
        return "Name: " + node.name +
        " positionX: " + node.positionX +
        " positionY: " + node.positionY +
        " positionX: " + node.positionZ +
        " rotationX: " + node.rotationX +
        " rotationY: " + node.rotationY +
        " rotationZ: " + node.rotationZ;
    }
}
