using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(AnimationGenerator))]
public class AnimationGeneratorEditor : Editor
{
    static int BUTTON_HEIGHT = 24;

    AnimationGenerator generator;
    Texture2D playIcon;
    Texture2D pauseIcon;
    Texture2D pencilIcon;
    Texture2D stopIcon;
    Texture2D rotationIcon;
    Texture2D clearIcon;
    Texture2D eyeIcon;

    bool blockingMouseInput = false;
    private Vector2 lastPoint;

    public void OnEnable()
    {
#if UNITY_EDITOR
        m_LastEditorUpdateTime = Time.realtimeSinceStartup;
        EditorApplication.update += OnEditorUpdate;
#endif
        generator = (AnimationGenerator)target;
        playIcon = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Editor/Images/play.png", typeof(Texture2D));
        pauseIcon = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Editor/Images/pause.png", typeof(Texture2D));
        stopIcon = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Editor/Images/stop.png", typeof(Texture2D));
        pencilIcon = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Editor/Images/pencil.png", typeof(Texture2D));
        rotationIcon = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Editor/Images/rotation.png", typeof(Texture2D));
        clearIcon = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Editor/Images/clear-rotation.png", typeof(Texture2D));
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

        GUIContent rotationButtonClear = new GUIContent();
        rotationButtonClear.image = clearIcon;
        rotationButtonClear.text = "Clear Rotation Points";

        GUIContent hideLOAButtonContent = new GUIContent();
        hideLOAButtonContent.image = eyeIcon;
        hideLOAButtonContent.text = "Toggle LOA";

        GUIContent hideDrawingPlaneButtonContent = new GUIContent();
        hideDrawingPlaneButtonContent.image = eyeIcon;
        hideDrawingPlaneButtonContent.text = "Toggle Drawing Plane";

        GUILayout.BeginVertical();
        GUILayout.Label(" Animation Preview Control ");
        float frame = GUILayout.HorizontalSlider(generator.currentFrame, 0, (float)generator.GetNumFrames());
        if (frame != generator.currentFrame)
        {
            generator.currentFrame = (int)frame;
            generator.AnimateFrame();
        }
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button(generator.animationPlaying ? pauseIcon : playIcon, buttonStyle))
        {
            generator.ToggleAnimation();
        }
        if (GUILayout.Button(stopIcon, buttonStyle))
        {
            generator.StopAnimation();
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button(drawButtonContent, middleButtonStyle))
        {
            //generator.StopAnimation();
            generator.drawing = true;
            generator.rotating = false;
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button(editButtonContent, middleButtonStyle))
        {
            //generator.StopAnimation();
            generator.ClearPoints();
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
        if(GUILayout.Button(rotationButtonContent, middleButtonStyle))
        {
            Debug.Log("hello world, from roation point code.!");
            generator.rotating = true;
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if(GUILayout.Button(rotationButtonClear, middleButtonStyle))
        {
            Debug.Log("CLEARNING.!");
            generator.ClearRotations();
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if(GUILayout.Button(hideLOAButtonContent, middleButtonStyle))
        {
            generator.drawLOA = !generator.drawLOA;
            SceneView.RepaintAll();
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if(GUILayout.Button(hideDrawingPlaneButtonContent, middleButtonStyle))
        {
            generator.drawPlane = !generator.drawPlane;
            SceneView.RepaintAll();
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Frames of Animation");
        generator.framesOfAnimation = (int)CustomGUILayout.FloatField((float)generator.framesOfAnimation);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();

        GUILayout.Label("Max Frame Rate");
        generator.timeBetweenFrames = 1 / CustomGUILayout.FloatField(generator.GetFrameRate());
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
        if (e.type == EventType.MouseDown && generator.rotating) {
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            float rayDistance;
            if (generator.editorPlane.Raycast(ray, out rayDistance))
            {
                Vector3 point = ray.GetPoint(rayDistance);
                generator.addRotation(point);
            }
        }
        if (e.type == EventType.MouseDown && generator.drawing)
        {
            blockingMouseInput = true;

        }
        else if (e.type == EventType.MouseDrag && e.mousePosition != lastPoint)
        {
            if (generator.drawing)
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
                generator.drawing = false;
                generator.GenerateAnimation();
            }
            blockingMouseInput = false;
        }
        if (e.type == EventType.Layout && generator.drawing)
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
        generator.RestoreAnimation();
        generator.UpdateAnimation(Time.realtimeSinceStartup - m_LastEditorUpdateTime);
        m_LastEditorUpdateTime = Time.realtimeSinceStartup;
    }

}
