using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using swellanimations;

public class AnimationData
{

    public static string NodeToString(Node node)
    {
        return "Node Name: " + node.name +
        " Node positionX: " + node.position.x +
        " Node positionY: " + node.position.y +
        " Node positionX: " + node.position.z +
        " Node rotationX: " + node.eularAngles.x +
        " Node rotationY: " + node.eularAngles.y +
        " Node rotationZ: " + node.eularAngles.z;
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

    public static string TransformToString(Transform node)
    {
        return "transform Name: " + node.name +
        " transform positionX: " + node.position.x +
        " transform positionY: " + node.position.y +
        " transform positionX: " + node.position.z +
        " transform rotationX: " + node.eulerAngles.x +
        " transform rotationY: " + node.eulerAngles.y +
        " transform rotationZ: " + node.eulerAngles.z;
    }

    //The following are used for debug purposes
    public static void PrintAllTransforms(Transform node, string spacing)
    {
        Debug.Log(spacing + TransformToString(node)); ;
        foreach (Transform childNode in node)
        {
            PrintAllTransforms(childNode, spacing + "-");
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
        Vector position = new Vector()
        {
            x = transform.position.x,
            y = transform.position.y,
            z = transform.position.z
        };
        Vector eulerAngles = new Vector()
        {
            x = transform.eulerAngles.x,
            y = transform.eulerAngles.y,
            z = transform.eulerAngles.z
        };
        return new Node()
        {
            name = transform.gameObject.name,
            position = position,
            eularAngles = eulerAngles
        };
    }

    public static void GenerateChildren(Transform children, Node parent)
    {
        foreach (Transform transform in children)
        {
            Node child;
            //For now we are only sending the spine this will change later
            if (transform.gameObject.name.Contains("spine"))
            {
                child = CreateNodeFromGameObject(transform);
                parent.children.Add(child);
            } else
            {
                child = parent;
            }
            GenerateChildren(transform, child);
        }
    }

    public static ModelData CreateModelData(Transform model, List<Vector3> controlPoints)
    {
        ModelData modelData = new ModelData();
        modelData.model = GenerateNode(model);
        //PrintAllNodes(modelData.model, "-");
        foreach(Vector3 point in controlPoints)
        {
            modelData.controlPoints.Add(new Vector()
            {
                x = point.x,
                y = point.y,
                z = point.z
            });
        }

        return modelData;
    }

    public static void PrintAnimation(Node[] frames)
    {
        for (int x = 0; x < frames.Length; x++)
        {
            Debug.Log("------------------------------Printing Frame " + x + "---------------------------------");
            AnimationData.PrintAllNodes(frames[x], "-");
            Debug.Log("-----------------------------End Printing Frame " + x + "-------------------------------");
        }
    }

}
