using UnityEngine;
using System.Collections;

[AddComponentMenu("Animation/Animation Tester")]
public class TestScript : MonoBehaviour {

    public float delay = 0f;
    private bool started = false;

	// Use this for initialization
	void Start () {
    }
	
	// Update is called once per frame
	void Update () {
        if (Time.fixedTime > delay && !started)
        {
            started = true;
            AnimationPlayer animationPlayer = gameObject.GetComponent<AnimationPlayer>();
            animationPlayer.ToggleAnimation();
        }
    }
}
