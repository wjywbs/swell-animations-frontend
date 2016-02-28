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
       // print("Message from libhello: " + hello());
        Debug.Log("Hello");
        Node node = new Node()
        {
            data = 0
        };
        for(int x = 1; x < 100; x++)
        {
            Node newNode = new Node()
            {
                data = x + x,
                childNode = node
            };
            node = newNode;
        }
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
        Node test = node;
        Debug.Log(node);
        Debug.Log(node.data);
        while(test.childNode != null && x < 100)
        {
            Debug.Log("ID: " + id + " Node number: " + x + " data: " + test.data);
            x++;
            test = test.childNode;
        }
    }
}
