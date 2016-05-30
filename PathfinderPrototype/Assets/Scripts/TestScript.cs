using UnityEngine;
using System.Collections;

[AddComponentMenu("Animation/Animation Tester")]
public class TestScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
        AnimationPlayer animationPlayer = gameObject.GetComponent<AnimationPlayer>();
        animationPlayer.ToggleAnimation();
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
