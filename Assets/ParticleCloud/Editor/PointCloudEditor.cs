using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(PointCloud))]
[CanEditMultipleObjects]
public class PointCloudEditor : Editor {

	SerializedProperty pointSize;

	void OnEnable(){
		PointCloud pc = target as PointCloud;

		pointSize = serializedObject.FindProperty("pointSize");

		//pc.particleSystem.hideFlags = HideFlags.HideInInspector;
		pc.ResetParticles();
		//pc.particleSystem.Pause();
	}

	public override void OnInspectorGUI(){
		serializedObject.Update ();

		//GUILayout.Label("Custom "+serializedObject.GetType().ToString());

		EditorGUILayout.Slider(pointSize, 0.001f, 2f);

		serializedObject.ApplyModifiedProperties();
		int pointsTotal = 0;
		foreach(Object t in targets){
			if(t is PointCloud){
				PointCloud pc = t as PointCloud;
				//GUILayout.Label(pc.transform.position.ToString());
				GUILayout.Label(pc.cloudDimensions.ToString());
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
