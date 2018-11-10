using UnityEngine;
using System.Collections;

[RequireComponent (typeof(ParticleSystem))]
public class PointCloudLODGroup : MonoBehaviour {


	[System.Serializable]
	public class PointCloudLOD{

		public PointCloud.CloudPoint[] points = new PointCloud.CloudPoint[0];

		public int Length{
			get{
				return points.Length;
			}
		}

		public float pointSize = 1f;
	}

	[SerializeField]
	public PointCloudLOD[] pointLods = new PointCloudLOD[0]; // was jaggedArray [lod][points]

	public int nPoints{
		get {
			int n = 0;
			for(int i=0; i<pointLods.Length; i++){
				n += pointLods[i].Length;
			}
			return n;
		}
	}

	public string lodStats{
		get{
			string s = pointLods.Length+" LODs: ";
			for(int i=0; i<pointLods.Length; i++){
				s+="  "+i+":"+pointLods[i].Length;
			}
			return s;
		}
	}

	public int currentLOD = 0;
	public void ResetParticles(){
		ResetParticles(currentLOD);
	}
	public void ResetParticles(int lod){
		if(lod<0 || lod>=pointLods.Length){
			GetComponent<ParticleSystem>().Pause();
			return;
		}
		ParticleSystem.Particle[] particles = new ParticleSystem.Particle[pointLods[lod].Length];
		//particleSystem.Emit(points.Length);
		
		for(int i=0; i<pointLods[lod].Length; i++){
			if(i < 10){
				//Debug.Log("point at "+points[i].pos);
			}
			particles[i].position = pointLods[lod].points[i].pos;
			particles[i].color = pointLods[lod].points[i].col;
			particles[i].size = pointLods[lod].pointSize;
			particles[i].remainingLifetime = float.PositiveInfinity;
			particles[i].velocity = Vector3.zero;
			//if(i%10==0) Debug.Log(points[i].pos);
			
			//particleSystem.Emit(points[i].pos, Vector3.zero, pointSize, float.PositiveInfinity, Color.red);
			
		}
		GetComponent<ParticleSystem>().SetParticles(particles, pointLods[lod].Length);
		GetComponent<ParticleSystem>().Pause();
		Debug.Log("reset "+pointLods[lod].Length+" particles");
		
	}

	void Awake(){
		GetComponent<ParticleSystem>().loop=true;
		GetComponent<ParticleSystem>().enableEmission=true;
		GetComponent<ParticleSystem>().playOnAwake=true;
		GetComponent<ParticleSystem>().GetComponent<Renderer>().castShadows=false;
		GetComponent<ParticleSystem>().GetComponent<Renderer>().receiveShadows=false;
		Debug.Log("Awakened");
	}
	void Start(){
		ResetParticles();
	}
	void OnEnable(){
		ResetParticles();
		Debug.Log("on enable");
	}

	[SerializeField]
	private Vector3 cloudDimensions;
	void OnDrawGizmos(){
		Gizmos.DrawWireCube(transform.position, cloudDimensions);
	}

	public void LoadPointsFromXyz(int destLOD, string f){
		string[] lines = f.Split('\n');
		
		int startLine = 0;
		int numPoints = lines.Length-1; //expects last line to be empty

		//Create the destLOD!!
		Debug.Log("trying to load points to LOD "+destLOD+" pointLods current length:"+pointLods.Length);
		if(destLOD >= pointLods.Length){

			int len = pointLods.Length;

			// increase array size to destLOD + 1
			PointCloudLOD[] oldLods = new PointCloudLOD[len];

			// copy to oldLods
			pointLods.CopyTo(oldLods,0);

			// increase size to destLOD + 1
			pointLods = new PointCloudLOD[destLOD+1];

			// initialize with oldLods if exist, or a new PointCloudLOD if not
			for(int x=0; x<pointLods.Length; x++){
				if(x < oldLods.Length){
					pointLods[x] = oldLods[x];
				}
				else{
					pointLods[x] = new PointCloudLOD();

					//set default point size values
					if(x == 1){
						pointLods[x].pointSize = 0.004f;
					}
					else if(x == 2){
						pointLods[x].pointSize = 0.241f;
					}

				}
			}

			Debug.Log(" there are now "+pointLods.Length+" lods");
		}

		pointLods[destLOD].points = new PointCloud.CloudPoint[numPoints];

		GetComponent<ParticleSystem>().maxParticles = Mathf.Max(GetComponent<ParticleSystem>().maxParticles, numPoints);
		if(numPoints <= 0){
			Debug.LogWarning("no points in the file");
			return;
		}
		
		//find the bounds and set transform to the center
		//todo: use values from previous pointclouds
		float minX=float.PositiveInfinity, maxX=float.NegativeInfinity, minY=float.PositiveInfinity, maxY=float.NegativeInfinity, minZ=float.PositiveInfinity, maxZ=float.NegativeInfinity;
		
		for(int i=0; i<numPoints; i++){
			string [] values = lines[i+startLine].Split(' ');
			pointLods[destLOD].points[i]=new PointCloud.CloudPoint();
			
			if(values.Length >= 6){
				
				pointLods[destLOD].points[i].pos = new Vector3( float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]) );
				pointLods[destLOD].points[i].col = new Color32(byte.Parse(values[3]), byte.Parse(values[4]), byte.Parse(values[5]), byte.MaxValue);
				
				//set min max
				if(pointLods[destLOD].points[i].pos.x > maxX) maxX = pointLods[destLOD].points[i].pos.x;
				if(pointLods[destLOD].points[i].pos.x < minX) minX = pointLods[destLOD].points[i].pos.x;
				if(pointLods[destLOD].points[i].pos.y > maxY) maxY = pointLods[destLOD].points[i].pos.y;
				if(pointLods[destLOD].points[i].pos.y < minY) minY = pointLods[destLOD].points[i].pos.y;
				if(pointLods[destLOD].points[i].pos.z > maxZ) maxZ = pointLods[destLOD].points[i].pos.z;
				if(pointLods[destLOD].points[i].pos.z < minZ) minZ = pointLods[destLOD].points[i].pos.z;
			}
			else{
				Debug.LogWarning("couldn't parse "+lines[i+startLine]);
			}
		}
		cloudDimensions = new Vector3(maxX - minX, maxY - minY, maxZ - minZ);
		Vector3 centerPos = new Vector3((minX+maxX)/2f, (minY+maxY)/2f, (minZ+maxZ)/2f);
		//Debug.Log("center:"+centerPos);
		foreach(PointCloud.CloudPoint cp in pointLods[destLOD].points){
			cp.pos -= centerPos;
		}

		//todo: this is now wrong for LODs!!!
		transform.position = centerPos;
		Debug.Log(" and now there are "+pointLods.Length+" lods");
	}

	// Update is called once per frame
	void Update () {
		Vector3 nearClipCenter = Camera.main.transform.position + Camera.main.transform.forward * Camera.main.nearClipPlane;
		float dist =  Vector3.Distance(transform.position, nearClipCenter);

		float lodSwitchAt = 3f;

		if(currentLOD == 2 && dist < lodSwitchAt){
			currentLOD = 1;
			ResetParticles();
			Debug.Log(gameObject.name + " LOD set to "+currentLOD);
		}
		else if(currentLOD == 1 && dist >= lodSwitchAt){
			currentLOD = 2;
			ResetParticles();
			Debug.Log(gameObject.name + " LOD set to "+currentLOD);
		}

	}
}
