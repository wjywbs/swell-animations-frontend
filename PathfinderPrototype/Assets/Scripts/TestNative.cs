using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using swellanimations;
using System.IO;
using System.Runtime.InteropServices;
using System;


public class TestNative : MonoBehaviour {

    [DllImport("TestDLL")]
    private static extern IntPtr setDataNegative(byte[] a, int size, ref uint outputSize);

	void Start () {
        Debug.Log("Hello");
        Node node = new Node()
        {
            name = "s" + 0,
            positionX = 0,
            positionY = 0,
            positionZ = 0,
            rotationX = 0,
            rotationY = 0,
            rotationZ = 0
        };
        //for(int x = 1; x < 100; x++)
        //{
        //    Node newNode = new Node()
        //    {
        //        name = "s" + x,
        //        positionX = x * 2,
        //        positionY = x * 2,
        //        positionZ = x * 2,
        //        rotationX = x * 2,
        //        rotationY = x * 2,
        //        rotationZ = x * 2

        //    };
        //    newNode.children.Add(node);
        //    node = newNode;
        //}
        printNodes(node, "Initial");
        ModeldataSerializer serializer = new ModeldataSerializer();
        MemoryStream msTestString = new MemoryStream();
        serializer.Serialize(msTestString, node);
        byte[] arr = msTestString.ToArray();
        int size = arr.Length;
        Debug.Log(Convert.ToBase64String(arr));

        //Debug.Log(setDataNegative(arr, size));
        uint outputSize = 0;
        IntPtr retData = setDataNegative(arr, size, ref outputSize);
        var bytes = new byte[(int)outputSize];
        Marshal.Copy(retData, bytes, 0, (int)outputSize);
        Debug.Log(Convert.ToBase64String(bytes));

        MemoryStream stream = new MemoryStream(bytes, false);
        Node myData = null;
        myData = (Node)serializer.Deserialize(stream, null, typeof(Node));
        msTestString.Close();
        stream.Close();
        printNodes(myData, "Return");
        
	}

    private void printNodes(Node node, string id)
    {
        int x = 0;
        Debug.Log(node);
        Node test = node;
        Debug.Log("ID: " + id + " Node number: " + x + " positionX: " + test.positionX);
        while (test.children.Count != 0 && x < 100)
        {
            Debug.Log("ID: " + id + " Node number: " + x + " positionX: " + test.positionX);
            x++;
            test = test.children[0];
        }
    }
}
