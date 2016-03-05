using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using swellanimations;
using System.IO;
using System.Runtime.InteropServices;
using System;


public class BackendAdapter {

    [DllImport("swell-integration")]
    private static extern IntPtr generateAnimation(byte[] a, int size, ref uint outputSize);

    public static Node[] GenerateFromBackend(ModelData modelData)
    {
        ModeldataSerializer serializer = new ModeldataSerializer();
        MemoryStream memStream = new MemoryStream();
        serializer.Serialize(memStream, modelData);
        byte[] arr = memStream.ToArray();
        Debug.Log(Convert.ToBase64String(arr));
        int size = arr.Length; 
        uint outputSize = 0;
        IntPtr retData = generateAnimation(arr, size, ref outputSize);
        var bytes = new byte[(int)outputSize];
        Marshal.Copy(retData, bytes, 0, (int)outputSize);
        Debug.Log(Convert.ToBase64String(bytes));
        MemoryStream stream = new MemoryStream(bytes, false);
        swellanimations.Animation animation = null;
        animation = (swellanimations.Animation)serializer.Deserialize(stream, null, typeof(swellanimations.Animation));
        memStream.Close();
        stream.Close();
        return animation.frames.ToArray();
    }
}
