using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor (typeof(Grid))]
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
		Texture2D rotationIcon;
		Texture2D eyeIcon;

		bool blockingMouseInput = false;
		static bool drawing = false;
		private Vector2 lastPoint;

		public void OnEnable ()
		{
				grid = (Grid)target;
				beginIcon = (Texture2D)AssetDatabase.LoadAssetAtPath ("Assets/Editor/Images/begining.png", typeof(Texture2D));
				rewIcon = (Texture2D)AssetDatabase.LoadAssetAtPath ("Assets/Editor/Images/rew.png", typeof(Texture2D));
				playIcon = (Texture2D)AssetDatabase.LoadAssetAtPath ("Assets/Editor/Images/play.png", typeof(Texture2D));
				ffIcon = (Texture2D)AssetDatabase.LoadAssetAtPath ("Assets/Editor/Images/FF.png", typeof(Texture2D));
				endIcon = (Texture2D)AssetDatabase.LoadAssetAtPath ("Assets/Editor/Images/end.png", typeof(Texture2D));
				pencilIcon = (Texture2D)AssetDatabase.LoadAssetAtPath ("Assets/Editor/Images/pencil.png", typeof(Texture2D));
				rotationIcon = (Texture2D)AssetDatabase.LoadAssetAtPath ("Assets/Editor/Images/rotation.png", typeof(Texture2D));
				eyeIcon = (Texture2D)AssetDatabase.LoadAssetAtPath ("Assets/Editor/Images/eye_open.png", typeof(Texture2D));
		}

		public override void OnInspectorGUI ()
		{
				GUIStyle buttonStyle = new GUIStyle (GUI.skin.button);
				buttonStyle.margin = new RectOffset (5, 5, 5, 5);
				buttonStyle.padding = new RectOffset (2, 2, 2, 2);
				buttonStyle.fixedHeight = BUTTON_HEIGHT;
				buttonStyle.fixedWidth = 64;

				GUIContent drawButtonContent = new GUIContent ();
				drawButtonContent.image = pencilIcon;
				drawButtonContent.text = "Edit LOA";

				GUIContent editButtonContent = new GUIContent ();
				editButtonContent.image = pencilIcon;
				editButtonContent.text = "Clear LOA";

				GUIContent rotationButtonContent = new GUIContent ();
				rotationButtonContent.image = rotationIcon;
				rotationButtonContent.text = "Add Rotation Point"; 
        
				GUIContent hideLOAButtonContent = new GUIContent ();
				hideLOAButtonContent.image = eyeIcon;
				hideLOAButtonContent.text = "Hide LOA";

				GUIContent hideRotationButtonContent = new GUIContent ();
				hideRotationButtonContent.image = eyeIcon;
				hideRotationButtonContent.text = "Hide Rotation Points";

				GUILayout.BeginVertical ();
				GUILayout.Label (" Animation Preview Control ");
				GUILayout.HorizontalSlider (0, 0, 100);
				GUILayout.BeginHorizontal ();
				GUILayout.FlexibleSpace ();
				GUILayout.Button (beginIcon, buttonStyle);
				GUILayout.Button (rewIcon, buttonStyle);
				GUILayout.Button (playIcon, buttonStyle);
				GUILayout.Button (ffIcon, buttonStyle);
				GUILayout.Button (endIcon, buttonStyle);
				GUILayout.FlexibleSpace ();
				GUILayout.EndHorizontal ();
				if (GUILayout.Button (drawButtonContent, GUILayout.Height (BUTTON_HEIGHT))) {
						drawing = true;
				}
				if (GUILayout.Button (editButtonContent, GUILayout.Height (BUTTON_HEIGHT))) {
						grid.clearPoints ();
						SceneView.RepaintAll ();
				}
				GUILayout.Button (rotationButtonContent, GUILayout.Height (BUTTON_HEIGHT));
				GUILayout.Button (hideLOAButtonContent, GUILayout.Height (BUTTON_HEIGHT));
				GUILayout.Button (hideRotationButtonContent, GUILayout.Height (BUTTON_HEIGHT));
				GUILayout.BeginHorizontal ();
				GUILayout.Label ("Mesh");
				GUILayout.TextField ("mesh name");     
				GUILayout.EndHorizontal ();

				GUILayout.Label ("Editor Plane");

				grid.planePoint1 = EditorGUILayout.ObjectField ("Plane Point 1", grid.planePoint1, typeof(Transform), true) as Transform;
				grid.planePoint2 = EditorGUILayout.ObjectField ("Plane Point 2", grid.planePoint2, typeof(Transform), true) as Transform;
				grid.planePoint3 = EditorGUILayout.ObjectField ("Plane Point 3", grid.planePoint3, typeof(Transform), true) as Transform;

				GUILayout.EndVertical ();
		}

		private void OnSceneGUI ()
		{
				Event e = Event.current;
				if (e.type == EventType.MouseDown && drawing) {
						blockingMouseInput = true;

				} else if (e.type == EventType.MouseDrag && e.mousePosition != lastPoint) {
						if (drawing) {
								getWorldPointFromMousePoint (e.mousePosition);
								SceneView.RepaintAll ();
						}
				} else if (e.type == EventType.MouseUp) {
						if (blockingMouseInput) {
								e.Use ();
								drawing = false;             
						}
						blockingMouseInput = false;
				}
				if (e.type == EventType.Layout && drawing) {
						//somehow this allows e.Use() to actually function and block mouse input
						HandleUtility.AddDefaultControl (GUIUtility.GetControlID (GetHashCode (), FocusType.Passive));
				}
		}

		private void getWorldPointFromMousePoint (Vector3 mousePoint)
		{
				Ray ray = HandleUtility.GUIPointToWorldRay (mousePoint);
				float rayDistance;
				if (grid.editorPlane.Raycast (ray, out rayDistance)) {
						Vector3 point = ray.GetPoint (rayDistance);            
						grid.addPoint (point);
				}        
		}


}
