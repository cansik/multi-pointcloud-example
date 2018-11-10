using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FileLoader : MonoBehaviour {


	private List<PointCloud> clouds = new List<PointCloud>();


	public Mesh cloudMesh;
	// Use this for initialization
	void Start () {
		dbgString = "started";

		TextAsset ptsFile = Resources.Load("examplePts") as TextAsset;

		LoadFile(ptsFile.text);
		// if(cloudMesh != null){
		// 	LoadMesh(cloudMesh);
		// }


		//LoadFile("7     \n0.5955505 0.8973999 0.2449951 -1934 125 118 102\n35.5955505 -179.8973999 1861.2449951 -1934 125 118 102\n35.5770302 -179.9205170 1861.1866455 -1913 181 173 160\n35.5856400 -179.9092102 1861.1351318 -1921 161 155 141\n35.5833511 -179.9119873 1861.0806885 -1917 194 188 174\n35.5838661 -179.9111328 1861.0270996 -1912 195 189 173\n35.5815773 -179.9139099 1860.9726563 -1914 193 187 171");

	}
	
//	private Vector3 offset = new Vector3(10, 0, 0);
	private int num = 1;
	public Material pointsMaterial;
	public Transform cameraRotater;
	private PointCloud pc;
	public void LoadFile(string f){
		//http://answers.unity3d.com/questions/9960/how-do-i-let-the-user-select-a-local-file-for-my-a.html
		//dbgString = "should load "+f;
		
		GameObject go = new GameObject("PointCloud");
		pc = go.AddComponent<PointCloud>();
		pc.GetComponent<ParticleSystem>().GetComponent<Renderer>().material = pointsMaterial;
		pc.pointSize = 0.1f;
		
		//GameObject p = new GameObject("pointcloud");
		//PointCloud pc = Instantiate(pointCloudPrefab) as PointCloud; //p.AddComponent<ParticleSystem>();
		//PointCloud pc = ps.GetComponent<PointCloud>();
		pc.LoadPointsFromPts(f);

		cameraRotater.position = pc.transform.position;

		//Debug.Log("loaded file, now reseting particles");
		//pc.ResetParticles();//why not????
		CancelInvoke("RestartParticlesLater");
		Invoke("ResetParticlesLater", 6f);//why?????    what a hack!

		clouds.Add(pc);

		dbgString+="\nloaded file "+num;
		num++;
		
	}
	public void LoadMesh(Mesh m){
		GameObject go = new GameObject("PointCloudMesh");
		pc = go.AddComponent<PointCloud>();
		pc.GetComponent<ParticleSystem>().GetComponent<Renderer>().material = pointsMaterial;

		pc.LoadPointsFromMesh(m);
		pc.ResetParticles();
		cameraRotater.position = pc.transform.position;

		clouds.Add(pc);
	}

	string dbgString = "nothing jet";
	void OnGUI(){
		GUI.Label(new Rect(20, Screen.height/2, 220, 80), dbgString);
		if(pc != null){
			float newPointSize = GUI.HorizontalSlider(new Rect(50, 50, 220, 50), pc.pointSize, 0.001f, 2f);
			if(newPointSize != pc.pointSize){
				foreach(PointCloud c in clouds){
					c.pointSize = newPointSize;
					c.ResetParticles();
				}
			}
		}
	}
	public void ResetParticlesLater(){
		foreach(PointCloud c in clouds){
			c.ResetParticles();
		}
	}
}
