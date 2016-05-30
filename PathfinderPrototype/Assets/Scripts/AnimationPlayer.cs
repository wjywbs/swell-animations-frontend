using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;
using System;

[AddComponentMenu("Animation/Animation Player")]
public class AnimationPlayer : MonoBehaviour
{

    private AnimationGenerator animationGenerator = new AnimationGenerator();
    public TextAsset animationFile;

    // Use this for initialization
    void Start()
    {
        ParseFile();
    }

    // Update is called once per frame
    void Update()
    {
        animationGenerator.UpdateAnimation(Time.deltaTime);
    }

    public void StopAnimation()
    {
        animationGenerator.StopAnimation();
    }

    public void ToggleAnimation()
    {
        animationGenerator.ToggleAnimation();
    }

    void ParseFile()
    {
        var rx = new System.Text.RegularExpressions.Regex(AnimationGenerator.FILE_LINE_SEPARATOR);
        String[] splitText = rx.Split(animationFile.text);
        animationGenerator.serializedAnimation = splitText[0];
        String[] planeVector1Text = splitText[1].Split(',');
        Vector3 planeVector1 = new Vector3(float.Parse(planeVector1Text[0]), float.Parse(planeVector1Text[1]), float.Parse(planeVector1Text[2]));
        animationGenerator.model = transform;
        animationGenerator.planeVector1 = planeVector1;
        animationGenerator.forceRestore = true;
        animationGenerator.RestoreAnimation();
    }
}