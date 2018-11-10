using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(PointCloudLODGroup))]
[CanEditMultipleObjects]
public class PointCloudLODGroupEditor : Editor {

	SerializedProperty currentLOD;
	SerializedProperty pointLods;
	void OnEnable(){
		PointCloudLODGroup pc = target as PointCloudLODGroup;

		currentLOD = serializedObject.FindProperty("currentLOD");

		pointLods = serializedObject.FindProperty("pointLods");

		//pc.particleSystem.hideFlags = HideFlags.HideInInspector;
		pc.ResetParticles();
		//pc.particleSystem.Pause();
	}

	public override void OnInspectorGUI(){
		serializedObject.Update ();

		//GUILayout.Label("Custom "+serializedObject.GetType().ToString());

		EditorGUILayout.IntSlider(currentLOD, 0, 4);
		EditorGUILayout.Slider(pointLods.GetArrayElementAtIndex(currentLOD.intValue).FindPropertyRelative("pointSize"), 0.001f, 1f);

		serializedObject.ApplyModifiedProperties();

		int pointsTotal = 0;
		foreach(Object t in targets){
			if(t is PointCloudLODGroup){
				PointCloudLODGroup pc = t as PointCloudLODGroup;
				//GUILayout.Label(pc.transform.position.ToString());
				//GUILayout.Label(pc.cloudDimensions.ToString());
				GUILayout.Label(pc.lodStats);
				//GUILayout.Label(pc.nPoints+" Points");
				pointsTotal += pc.nPoints;
				//GUILayout.Label(pc.particleSystem.IsAlive()?"Alive":"Dead");
				pc.ResetParticles();
				//pc.particleSystem.Pause();
			}
		}
		GUILayout.Label(pointsTotal+" Points selected");
	}
}
