using UnityEngine;
using System.Collections;

public class TestManaged : MonoBehaviour {

	// Use this for initialization
	void Start () {
        RandomNumber.RandomNumber obj = new RandomNumber.RandomNumber();
        print("Managed Random Number: " + obj.GetRandom());
	}
	
}
