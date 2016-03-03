using UnityEngine;
using System.Collections;
using swellanimations;

public class AnimationData
{

    public static string NodeToString(Node node)
    {
        return "Name: " + node.name +
        " positionX: " + node.positionX +
        " positionY: " + node.positionY +
        " positionX: " + node.positionZ +
        " rotationX: " + node.rotationX +
        " rotationY: " + node.rotationY +
        " rotationZ: " + node.rotationZ;
    }

    //The following are used for debug purposes
    public static void PrintAllNodes(Node node, string spacing)
    {
        Debug.Log(spacing + NodeToString(node)); ;
        foreach (Node childNode in node.children)
        {
            PrintAllNodes(childNode, spacing + "-");
        }

    }

    public static Node GenerateNode(Transform model)
    {
        Node node = CreateNodeFromGameObject(model);
        GenerateChildren(model, node);
        return node;
    }

    public static Node CreateNodeFromGameObject(Transform transform)
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

    public static void GenerateChildren(Transform children, Node parent)
    {
        foreach (Transform transform in children)
        {
            Node child = CreateNodeFromGameObject(transform);
            child.parent = parent;
            parent.children.Add(child);
            GenerateChildren(transform, child);
        }
    }

}
