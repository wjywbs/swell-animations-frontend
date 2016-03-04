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
            firstFrame.positionX = points[x].x;
            firstFrame.positionY = points[x].y;
            firstFrame.positionZ = points[x].z;
            frames[x] = CreateFrame(firstFrame, x);
        }
        return frames;
    }

    public static Node CreateFrame(Node frame, int currentFrame)
    {
        frame.positionX += 1;
        foreach (Node childNode in frame.children)
        {
            CreateFrame(childNode, currentFrame);
        }
        return frame;
    }
}
