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
            frames[x] = createFrame(firstFrame, x);
        }
        return frames;
    }

    private static Node createFrame(Node frame, int currentFrame)
    {
        frame.positionX += currentFrame;
        foreach (Node childNode in frame.children)
        {
            createFrame(childNode, currentFrame);
        }
        return frame;
    }
}
