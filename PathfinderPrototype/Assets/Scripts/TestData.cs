using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using swellanimations;

public class TestData {
    public static Node[] CreateTestAnimation(Node firstFrame, List<Vector3> points)
    {
        Node[] frames = new Node[points.Count];
        for(int x = 0; x < frames.Length; x++)
        {
            Node node = new Node();
            node.position.x = points[x].x;
            node.position.y = points[x].y;
            node.position.z = points[x].z;
            node.eularAngles = firstFrame.eularAngles;
            node.children.AddRange(firstFrame.children);
            frames[x] = CreateFrame(node, x);
        }
        return frames;
    }

    private static Node CreateFrame(Node frame, int currentFrame)
    {
        frame.position.x += currentFrame;
        foreach (Node childNode in frame.children)
        {
            CreateFrame(childNode, currentFrame);
        }
        return frame;
    }
}
