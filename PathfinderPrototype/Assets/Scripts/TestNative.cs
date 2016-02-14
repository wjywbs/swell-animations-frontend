using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;


public class TestNative : MonoBehaviour {

    [DllImport("RandomNumberDLL")]



    private static extern int GetRandom();

	void Start () {
        print("Native Random Number: " + GetRandom());
	}
}
