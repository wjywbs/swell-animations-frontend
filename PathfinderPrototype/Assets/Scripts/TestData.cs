using UnityEngine;
using System.Collections;
using swellanimations;

public class TestData {
    public static Node[] createTestAnimation(Node firstFrame)
    {
        Node[] frames = new Node[100];
        for(int x = 0; x < frames.Length; x++)
        {
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
