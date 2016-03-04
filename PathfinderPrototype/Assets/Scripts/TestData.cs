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
            node.positionX = points[x].x;
            node.positionY = points[x].y;
            node.positionZ = points[x].z;
            node.rotationX = firstFrame.rotationX;
            node.rotationY = firstFrame.rotationY;
            node.rotationZ = firstFrame.rotationZ;
            node.children.AddRange(firstFrame.children);
            frames[x] = CreateFrame(node, x);
        }
        return frames;
    }

    private static Node CreateFrame(Node frame, int currentFrame)
    {
        frame.positionX += currentFrame;
        foreach (Node childNode in frame.children)
        {
            CreateFrame(childNode, currentFrame);
        }
        return frame;
    }
}
