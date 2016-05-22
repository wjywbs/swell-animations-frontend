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
        return GenerateChildren(model, null, 1);
    }

    public static Transform FindChild(string name, Transform t)
    {
        if (t.name == name)
            return t;
        foreach (Transform child in t)
        {
            Transform ct = FindChild(name, child);
            if (ct != null)
                return ct;
        }
        return null;
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

    public static Node GenerateChildren(Transform model, Node parent, int x)
    {
        Transform nextSpine = FindChild("spine" + x, model);
        if(nextSpine != null)
        {
            Node child = CreateNodeFromGameObject(nextSpine);
            if (x > 1)
            {
                parent.children.Add(child);
            }
            parent = child;
            GenerateChildren(model, parent, ++x);
        }
        return parent;
    }

    public static ModelData CreateModelData(Transform model, List<Vector3> controlPoints, List<RotationPoint> rotationPoints, List<List<Vector3>> detailLoaPoints)
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
        foreach(List<Vector3> layer in detailLoaPoints)
        {
			AnimationLayer animationLayer = new AnimationLayer()
			{
				numFrames = 10,
				startFrame = 10
			};
            foreach(Vector3 point in layer)
            {
				animationLayer.Add(new Vector()
                {
                    x = point.x,
                    y = point.y,
                    z = point.z
                });
            }
			modelData.animationLayers.Add (animationLayer);
        }
        if (rotationPoints != null)
        {
            foreach (RotationPoint rotPoint in rotationPoints)
            {
                modelData.rotationpoints.Add(new swellanimations.RotationPoint()
                {
                    numFrames = 10,
                    startFrame = rotPoint.index,
                    Rotation = new Vector()
                    {
                        x = rotPoint.rotation.eulerAngles.x,
                        y = rotPoint.rotation.eulerAngles.y,
                        z = rotPoint.rotation.eulerAngles.z
                    }
                });
            }
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
