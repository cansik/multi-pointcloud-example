using UnityEngine;
using UnityEditor;
using System.Collections;
using System;
using System.IO;

public class PtsImporter : AssetPostprocessor {

	static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPath){
		foreach(string s in importedAssets){
			if(s.EndsWith(".pts") || s.EndsWith(".ply")){

				string prefabPath = s.Substring(0, s.Length-4) + ".prefab";
				GameObject go = new GameObject("tempObject");
				go.hideFlags = HideFlags.NotEditable;


				PointCloud pc = go.AddComponent<PointCloud>();

				ParticleSystem ps = go.GetComponent<ParticleSystem>();
				ps.loop=true;
				Debug.LogWarning("To get frustum culling working you need to set Prewarm option to true manually in "+prefabPath+" and if it's still not ok set the shape to box ahd it's size to match your pointcloud dimension");
				ps.enableEmission=true;
				ps.playOnAwake=true;
				ps.GetComponent<Renderer>().castShadows=false;
				ps.GetComponent<Renderer>().receiveShadows=false;

				go.GetComponent<ParticleSystem>().GetComponent<Renderer>().material = AssetDatabase.LoadAssetAtPath("Assets/Point.mat", typeof(Material)) as Material;

				//prefab.hideFlags = HideFlags.NotEditable;
				try{
					using(StreamReader sr = new StreamReader(s)){
						Debug.Log("loading points!!!");
						if(s.EndsWith(".pts")){
							pc.LoadPointsFromPts(sr.ReadToEnd());
						}
						else if(s.EndsWith(".ply")){
							pc.LoadPointsFromPly(sr.ReadToEnd());
						}
					}
				}
				catch(Exception e){
					Debug.LogError(e);
				}

				PrefabUtility.CreatePrefab(prefabPath, go);
				GameObject.DestroyImmediate(go);

				Debug.Log("imported asset "+s);



				//AssetDatabase.CreateAsset(prefab, prefabPath);
				//AssetDatabase.AddObjectToAsset(createPointcloudFromPts(""), s);
			}
		}
		foreach(string s in deletedAssets){
			if(s.EndsWith(".pts")){
				Debug.Log("deleted asset "+s);
			}
		}
		for(int i=0; i<movedAssets.Length; i++){
			if(movedAssets[i].EndsWith(".pts")){
				Debug.Log("moved asset from "+movedFromAssetPath[i]+" to "+movedAssets[i]);
			}
		}

	}
}
