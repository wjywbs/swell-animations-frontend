using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(AnimationGenerator))]
public class AnimationGeneratorEditor : Editor
{
    static int BUTTON_HEIGHT = 24;

    AnimationGenerator generator;
    Texture2D beginIcon;
    Texture2D rewIcon;
    Texture2D playIcon;
    Texture2D ffIcon;
    Texture2D endIcon;
    Texture2D pencilIcon;
    Texture2D rotationIcon;
    Texture2D eyeIcon;

    bool blockingMouseInput = false;
    static bool drawing = false;
    private Vector2 lastPoint;

    public void OnEnable()
    {
#if UNITY_EDITOR
        m_LastEditorUpdateTime = Time.realtimeSinceStartup;
        EditorApplication.update += OnEditorUpdate;
#endif
        generator = (AnimationGenerator)target;
        beginIcon = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Editor/Images/begining.png", typeof(Texture2D));
        rewIcon = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Editor/Images/rew.png", typeof(Texture2D));
        playIcon = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Editor/Images/play.png", typeof(Texture2D));
        ffIcon = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Editor/Images/FF.png", typeof(Texture2D));
        endIcon = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Editor/Images/end.png", typeof(Texture2D));
        pencilIcon = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Editor/Images/pencil.png", typeof(Texture2D));
        rotationIcon = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Editor/Images/rotation.png", typeof(Texture2D));
        eyeIcon = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Editor/Images/eye_open.png", typeof(Texture2D));
    }

    public override void OnInspectorGUI()
    {
        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.margin = new RectOffset(5, 5, 5, 5);
        buttonStyle.padding = new RectOffset(2, 2, 2, 2);
        buttonStyle.fixedHeight = BUTTON_HEIGHT;
        buttonStyle.fixedWidth = 32;

        GUIStyle middleButtonStyle = new GUIStyle(GUI.skin.button);
        middleButtonStyle.margin = new RectOffset(5, 5, 5, 5);
        middleButtonStyle.padding = new RectOffset(2, 2, 2, 2);
        middleButtonStyle.fixedHeight = BUTTON_HEIGHT;
        middleButtonStyle.fixedWidth = 200;

        GUIContent drawButtonContent = new GUIContent();
        drawButtonContent.image = pencilIcon;
        drawButtonContent.text = "Edit LOA";

        GUIContent editButtonContent = new GUIContent();
        editButtonContent.image = pencilIcon;
        editButtonContent.text = "Clear LOA";

        GUIContent rotationButtonContent = new GUIContent();
        rotationButtonContent.image = rotationIcon;
        rotationButtonContent.text = "Add Rotation Point";

        GUIContent hideLOAButtonContent = new GUIContent();
        hideLOAButtonContent.image = eyeIcon;
        hideLOAButtonContent.text = "Hide LOA";

        GUIContent hideRotationButtonContent = new GUIContent();
        hideRotationButtonContent.image = eyeIcon;
        hideRotationButtonContent.text = "Hide Rotation Points";

        GUILayout.BeginVertical();
        GUILayout.Label(" Animation Preview Control ");
        GUILayout.HorizontalSlider(0, 0, 100);
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Button(beginIcon, buttonStyle);
        GUILayout.Button(rewIcon, buttonStyle);
        if (GUILayout.Button(playIcon, buttonStyle))
        {
            generator.ToggleAnimation();
        }
        GUILayout.Button(ffIcon, buttonStyle);
        GUILayout.Button(endIcon, buttonStyle);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Time Between Frames");
        generator.timeBetweenFrames = CustomGUILayout.FloatField(generator.timeBetweenFrames);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button(drawButtonContent, middleButtonStyle))
        {
            drawing = true;
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button(editButtonContent, middleButtonStyle))
        {
            generator.clearPoints();
            SceneView.RepaintAll();
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Center On Drawing Plane", middleButtonStyle))
        {
            SceneView scenView = SceneView.lastActiveSceneView;
            Quaternion rotation = Quaternion.LookRotation(generator.editorPlane.normal, generator.planeVector1 * -1);
            scenView.LookAt(generator.planePoint1.position, rotation);
            scenView.Repaint();
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Button(rotationButtonContent, middleButtonStyle);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Button(hideLOAButtonContent, middleButtonStyle);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Button(hideRotationButtonContent, middleButtonStyle);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        generator.model = EditorGUILayout.ObjectField("Model", generator.model, typeof(Transform), true) as Transform;
        GUILayout.EndHorizontal();

        GUILayout.Label("Editor Plane");

        generator.planePoint1 = EditorGUILayout.ObjectField("Plane Point 1", generator.planePoint1, typeof(Transform), true) as Transform;
        generator.planePoint2 = EditorGUILayout.ObjectField("Plane Point 2", generator.planePoint2, typeof(Transform), true) as Transform;
        generator.planePoint3 = EditorGUILayout.ObjectField("Plane Point 3", generator.planePoint3, typeof(Transform), true) as Transform;

        GUILayout.EndVertical();
    }

    private void OnSceneGUI()
    {
        Event e = Event.current;
        if (e.type == EventType.MouseDown && drawing)
        {
            blockingMouseInput = true;

        }
        else if (e.type == EventType.MouseDrag && e.mousePosition != lastPoint)
        {
            if (drawing)
            {
                lastPoint = e.mousePosition;
                getWorldPointFromMousePoint(e.mousePosition);
                SceneView.RepaintAll();
            }
        }
        else if (e.type == EventType.MouseUp)
        {
            if (blockingMouseInput)
            {
                e.Use();
                drawing = false;
                generator.GenerateAnimation();
            }
            blockingMouseInput = false;
        }
        if (e.type == EventType.Layout && drawing)
        {
            //somehow this allows e.Use() to actually function and block mouse input
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(GetHashCode(), FocusType.Passive));
        }
    }

    private void getWorldPointFromMousePoint(Vector3 mousePoint)
    {
        Ray ray = HandleUtility.GUIPointToWorldRay(mousePoint);
        float rayDistance;
        if (generator.editorPlane.Raycast(ray, out rayDistance))
        {
            Vector3 point = ray.GetPoint(rayDistance);
            generator.addPoint(point);
        }
    }

    private float m_LastEditorUpdateTime;

    protected virtual void OnDisable()
    {
#if UNITY_EDITOR
        EditorApplication.update -= OnEditorUpdate;
#endif
    }

    protected virtual void OnEditorUpdate()
    {
        generator.UpdateAnimation(m_LastEditorUpdateTime);
    }

}
