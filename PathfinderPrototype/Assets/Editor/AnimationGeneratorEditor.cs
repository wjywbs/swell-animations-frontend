using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AnimationGenerator))]
public class AnimationGeneratorEditor : Editor
{
    static int BUTTON_HEIGHT = 24;

    AnimationGenerator generator;
    Texture2D playIcon;
    Texture2D pauseIcon;
    Texture2D eraserIcon;
    Texture2D pencilIcon;
    Texture2D pencilEditIcon;
    Texture2D stopIcon;
    Texture2D rotationIcon;
    Texture2D clearIcon;
    Texture2D deleteIcon;
    Texture2D eyeIcon;

    bool blockingMouseInputForDrawing = false;
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
        pencilEditIcon = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Editor/Images/pencil-edit.png", typeof(Texture2D));
        eraserIcon = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Editor/Images/eraser.png", typeof(Texture2D));
        rotationIcon = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Editor/Images/rotation.png", typeof(Texture2D));
        clearIcon = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Editor/Images/clear-rotation.png", typeof(Texture2D));
        deleteIcon = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Editor/Images/delete-rotation.png", typeof(Texture2D));
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
        drawButtonContent.text = "Draw LOA";

        GUIContent editButtonContent = new GUIContent();
        editButtonContent.image = pencilEditIcon;
        editButtonContent.text = "Edit LOA";

        GUIContent clearButtonContent = new GUIContent();
        clearButtonContent.image = eraserIcon;
        clearButtonContent.text = "Clear LOA";

        GUIContent rotationButtonContent = new GUIContent();
        rotationButtonContent.image = rotationIcon;
        rotationButtonContent.text = "Add Rotation Point";

        GUIContent rotationButtonDelete = new GUIContent();
        rotationButtonDelete.image = deleteIcon;
        rotationButtonDelete.text = "Delete Rotation Point";

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
            generator.drawingLOA = true;
            generator.addingRotationPoint = false;
            generator.editingLOA = false;
            generator.deletingRotationPoint = false;
            blockingMouseInputForDrawing = true;
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button(editButtonContent, middleButtonStyle))
        {
            //edit line            
            generator.editingLOA = true;
            generator.drawingLOA = false;
            generator.addingRotationPoint = false;
            generator.deletingRotationPoint = false;
            blockingMouseInputForDrawing = true;
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button(clearButtonContent, middleButtonStyle))
        {
            Undo.RecordObject(generator, "Clearing LOA");
            //generator.StopAnimation();
            generator.ClearPoints();
            generator.ClearRotationPoints();
            SceneView.RepaintAll();
            EditorUtility.SetDirty(generator);
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
        if (GUILayout.Button(rotationButtonContent, middleButtonStyle))
        {
            generator.addingRotationPoint = true;
            generator.deletingRotationPoint = false;
            generator.editingLOA = false;
            generator.drawingLOA = false;
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button(rotationButtonDelete, middleButtonStyle))
        {
            generator.deletingRotationPoint = true;
            generator.addingRotationPoint = false;
            generator.editingLOA = false;
            generator.drawingLOA = false;
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button(rotationButtonClear, middleButtonStyle))
        {
            Undo.RecordObject(generator, "Clearing Rotation Points");
            generator.ClearRotationPoints();
            SceneView.RepaintAll();
            EditorUtility.SetDirty(generator);
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button(hideLOAButtonContent, middleButtonStyle))
        {
            generator.renderLOA = !generator.renderLOA;
            SceneView.RepaintAll();
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button(hideDrawingPlaneButtonContent, middleButtonStyle))
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
        if (e.type == EventType.MouseDown)
        {
            Vector3 point = getWorldPointFromMousePoint(e.mousePosition);
            generator.mouseLocation = point;

            if (generator.addingRotationPoint)
            {
                Undo.RecordObject(generator, "Add Rotation Point");
                generator.AddRotationPoint(point);
                EditorUtility.SetDirty(generator);
            }
            else if (generator.editingLOA)
            {
                Undo.RecordObject(generator, "Edit LOA");
                generator.EditStart(point);
            }
            else if (generator.drawingLOA)
            {
                Undo.RecordObject(generator, "Draw LOA");
            }
            else if (generator.deletingRotationPoint)
            {
                generator.rotationPointToDelete = generator.getClosetestRotationPoint(point);
            }
        }
        else if (e.type == EventType.MouseDrag && e.mousePosition != lastPoint)
        {
            if (blockingMouseInputForDrawing)
            {
                lastPoint = e.mousePosition;
                Vector3 point = getWorldPointFromMousePoint(e.mousePosition);
                if (generator.drawingLOA)
                {
                    generator.AddPoint(point);
                }
                else if (generator.editingLOA)
                {
                    generator.AddEditPoint(point);
                }
                else
                {
                    blockingMouseInputForDrawing = false;
                }
                SceneView.RepaintAll();
            }
        }
        else if (e.type == EventType.MouseUp)
        {
            if (blockingMouseInputForDrawing)
            {
                e.Use();
                generator.drawingLOA = false;
                if (generator.editingLOA)
                {
                    Vector3 point = getWorldPointFromMousePoint(e.mousePosition);
                    generator.EditEnd(point);
                }                     
                EditorUtility.SetDirty(generator);
                generator.GenerateAnimation();
            }
            blockingMouseInputForDrawing = false;
            if(generator.deletingRotationPoint)
            {
                Vector3 point = getWorldPointFromMousePoint(e.mousePosition);
                if(generator.rotationPointToDelete == generator.getClosetestRotationPoint(point))
                {
                    Undo.RecordObject(generator, "Rotate Rotation Point");
                    generator.rotationPoints.Remove(generator.rotationPointToDelete);
                    EditorUtility.SetDirty(generator);
                }
            }
        }
        if (e.type == EventType.Layout && blockingMouseInputForDrawing)
        {
            //somehow this allows e.Use() to actually function and block mouse input
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(GetHashCode(), FocusType.Passive));
        }
        RotationPointHandles();
    }    

    public void RotationPointHandles()
    {
        if (generator.rotationPoints != null && generator.rotationPoints.Count > 0)
        {
            foreach (RotationPoint rotPoint in generator.rotationPoints)
            {
                if (generator.selectedRotationPointIndex != rotPoint.index
                    && Vector3.Distance(rotPoint.position, generator.mouseLocation) <= AnimationGenerator.ROTATION_POINT_RADIUS + AnimationGenerator.SELECT_RANGE)
                {
                    generator.selectedRotationPointIndex = rotPoint.index;
                }

                if (generator.selectedRotationPointIndex != rotPoint.index)
                {
                    continue;
                }

                EditorGUI.BeginChangeCheck();
                Quaternion rot = Handles.RotationHandle(rotPoint.rotation, rotPoint.position);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(generator, "Rotate Rotation Point");
                    EditorUtility.SetDirty(generator);
                    rotPoint.rotation = rot;
                }
            }
        }
    }

    private Vector3 getWorldPointFromMousePoint(Vector3 mousePoint)
    {
        Ray ray = HandleUtility.GUIPointToWorldRay(mousePoint);
        float rayDistance;
        generator.editorPlane.Raycast(ray, out rayDistance);
        return ray.GetPoint(rayDistance);
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
