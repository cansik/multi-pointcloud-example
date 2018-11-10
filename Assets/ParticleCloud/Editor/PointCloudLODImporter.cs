using UnityEngine;
using UnityEditor;
using System.Collections;
using System;
using System.IO;

public class PointCloudLODImporter : AssetPostprocessor {
	
	static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPath){
		foreach(string s in importedAssets){
			if(s.EndsWith(".xyz")){
				string prefabPath = s.Substring(0, s.Length-4) + ".prefab";
				int LOD = 0;
				if(s.Substring(s.Length-9,4).Equals("_LOD")){
					prefabPath = s.Substring(0, s.Length-9) + ".prefab";
					LOD = int.Parse(s.Substring(s.Length-5, 1));
				}
				Debug.Log("importing to "+prefabPath+" LOD:"+LOD);

				GameObject pcPrefabInstance;// = AssetDatabase.LoadMainAssetAtPath(prefabPath) as GameObject;
				UnityEngine.Object asset = AssetDatabase.LoadMainAssetAtPath(prefabPath);
				//return;
				PointCloudLODGroup pc = null;

				if(asset == null){
					Debug.Log("Creating new asset at "+prefabPath);
					pcPrefabInstance = new GameObject("newPointCloudPrefab");
					pcPrefabInstance.hideFlags = HideFlags.NotEditable;
					pc = pcPrefabInstance.AddComponent<PointCloudLODGroup>();
				}
				else{
					pcPrefabInstance = PrefabUtility.InstantiatePrefab(asset) as GameObject;
					pc = pcPrefabInstance.GetComponent<PointCloudLODGroup>();
				}

				ParticleSystem ps = pcPrefabInstance.GetComponent<ParticleSystem>();
				ps.loop=true;
				ps.enableEmission=false;
				ps.playOnAwake=true;
				ps.GetComponent<Renderer>().castShadows=false;
				ps.GetComponent<Renderer>().receiveShadows=false;
				ps.GetComponent<Renderer>().material = AssetDatabase.LoadAssetAtPath("Assets/Point.mat", typeof(Material)) as Material;
				Debug.LogWarning("To get frustum culling working you need to set Prewarm option to true manually in "+prefabPath+" and if it's still not ok set the shape to box ahd it's size to match your pointcloud dimension");

				
				//prefab.hideFlags = HideFlags.NotEditable;
				try{
					using(StreamReader sr = new StreamReader(s)){
						if(s.EndsWith(".xyz")){
							pc.LoadPointsFromXyz(LOD, sr.ReadToEnd());
							pc.currentLOD = LOD;
						}
					}
				}
				catch(Exception e){
					Debug.LogError(e);
				}
				
				CreateNew(pcPrefabInstance, prefabPath);
				//PrefabUtility.CreatePrefab(prefabPath, pcPrefab);


				//PrefabUtility.ReplacePrefab(prefabPath, pcPrefab);

				GameObject.DestroyImmediate(pcPrefabInstance);

				AssetDatabase.Refresh();

				Debug.Log("imported asset "+s);
				
				
				
				//AssetDatabase.CreateAsset(prefab, prefabPath);
				//AssetDatabase.AddObjectToAsset(createPointcloudFromPts(""), s);
			}
		}
	}

	static void CreateNew(GameObject obj, string localPath) {
		UnityEngine.Object prefab = PrefabUtility.CreateEmptyPrefab(localPath);
		PrefabUtility.ReplacePrefab(obj, prefab, ReplacePrefabOptions.ConnectToPrefab);
	}
}
