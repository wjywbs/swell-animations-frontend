using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Grid))]
public class GridEditor : Editor
{
    static int BUTTON_HEIGHT = 24;

    Grid grid;
    Texture2D beginIcon;
    Texture2D rewIcon;
    Texture2D playIcon;
    Texture2D ffIcon;
    Texture2D endIcon;
    Texture2D pencilIcon;

    bool blockingMouseInput = false;
    static bool drawing = false;

    public void OnEnable()
    {
        grid = (Grid)target;
        beginIcon = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Editor/Images/begining.png", typeof(Texture2D));
        rewIcon = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Editor/Images/rew.png", typeof(Texture2D));
        playIcon = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Editor/Images/play.png", typeof(Texture2D));
        ffIcon = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Editor/Images/FF.png", typeof(Texture2D));
        endIcon = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Editor/Images/end.png", typeof(Texture2D));
        pencilIcon = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Editor/Images/pencil.png", typeof(Texture2D));
    }

    public override void OnInspectorGUI()
    {
        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.margin = new RectOffset(5, 5, 5, 5);
        buttonStyle.padding = new RectOffset(2, 2, 2, 2);
        buttonStyle.fixedHeight = BUTTON_HEIGHT;
        buttonStyle.fixedWidth = 64;

        GUIContent drawButtonContent = new GUIContent();
        drawButtonContent.image = pencilIcon;
        drawButtonContent.text = "Draw LOA";

        GUIContent editButtonContent = new GUIContent();
        editButtonContent.image = pencilIcon;
        editButtonContent.text = "Clear LOA";

        GUILayout.BeginVertical();
        GUILayout.Label(" Animation Preview Control ");
        GUILayout.HorizontalSlider(0, 0, 100);
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Button(beginIcon, buttonStyle);
        GUILayout.Button(rewIcon, buttonStyle);
        GUILayout.Button(playIcon, buttonStyle);
        GUILayout.Button(ffIcon, buttonStyle);
        GUILayout.Button(endIcon, buttonStyle);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        if(GUILayout.Button(drawButtonContent, GUILayout.Height(BUTTON_HEIGHT)))
        {
            Debug.Log("Draw LOA Clicked");
            drawing = true;
            Debug.Log("Drawing: " + drawing);
        }
        if (GUILayout.Button(editButtonContent, GUILayout.Height(BUTTON_HEIGHT)))
        {
            grid.clearPoints();
            SceneView.RepaintAll();
        }
        GUILayout.BeginHorizontal();
        GUILayout.Label("Mesh");
        GUILayout.TextField("mesh name");
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Animation Layers");
        GUILayout.FlexibleSpace();
        GUILayout.Button("+");
        GUILayout.Button("-");
        GUILayout.EndHorizontal();
        GUILayout.TextArea("", GUILayout.Height(300));
        GUILayout.EndVertical();
    }

    private void OnSceneGUI()
    {
        Event e = Event.current;
        if (drawing)
        {
            Debug.Log("Drawing: " + drawing);
        }
        if (e.type == EventType.MouseDown && drawing)
        {
            Debug.Log("Mouse Down");
            blockingMouseInput = true;

        }
        else if (e.type == EventType.MouseDrag)
        {
            Debug.Log("Dragging");
            if(drawing)
            {
                Vector3 point = e.mousePosition;
                Debug.Log("Mouse Point: " + point);
                point.z = SceneView.currentDrawingSceneView.camera.transform.position.z;
                Vector3 worldPos = SceneView.currentDrawingSceneView.camera.ScreenToWorldPoint(point);
                Debug.Log("World Position: " + worldPos);
                worldPos.y = worldPos.y * -1;
                grid.addPoint(worldPos);
                SceneView.RepaintAll();
            }
        }
        else if (e.type == EventType.MouseUp)
        {
            Debug.Log("Mouse Up");
            if (blockingMouseInput)
            {
                e.Use();
                drawing = false;             
            }
            blockingMouseInput = false;
        }
        if (e.type == EventType.Layout && drawing)
        {
            //somehow this allows e.Use() to actually function and block mouse input
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(GetHashCode(), FocusType.Passive));
        }
    }
}
