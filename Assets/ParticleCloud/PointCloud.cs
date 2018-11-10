using UnityEngine;
using System.Collections;
//[ExecuteInEditMode]
[RequireComponent (typeof(ParticleSystem))]
public class PointCloud : MonoBehaviour {

	private ParticleSystem.Particle[] particles; //probably need to keep an own struct array of points in memory. scaling!

	[System.Serializable]
	public class CloudPoint{
		public Vector3 pos;
		public Color col;
	}

	[SerializeField]
	private CloudPoint[] points = new CloudPoint[0];
	public int nPoints{
		get { return points.Length; }
	}
	public float pointSize = 1f;

	public void ResetParticles(){
		particles = new ParticleSystem.Particle[points.Length];
		//particleSystem.Emit(points.Length);

		for(int i=0; i<points.Length; i++){
			if(i < 10){
				Debug.Log("point at "+points[i].pos);
			}
			particles[i].position = points[i].pos;
			particles[i].color = points[i].col;
			particles[i].size = pointSize;
			particles[i].remainingLifetime = float.PositiveInfinity;
			particles[i].velocity = Vector3.zero;
			//if(i%10==0) Debug.Log(points[i].pos);

			//particleSystem.Emit(points[i].pos, Vector3.zero, pointSize, float.PositiveInfinity, Color.red);

		}
		GetComponent<ParticleSystem>().SetParticles(particles, points.Length);
		GetComponent<ParticleSystem>().Pause();
		Debug.Log("reset "+points.Length+" particles");

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
	public Vector3 cloudDimensions;
	void OnDrawGizmos(){
		Gizmos.DrawWireCube(transform.position, cloudDimensions);
	}

	public void LoadPointsFromMesh(Mesh m){
		points = new CloudPoint[m.vertexCount];
		GetComponent<ParticleSystem>().maxParticles = m.vertexCount;
		Debug.Log(m.vertices.Length+" Verts;  "+m.colors.Length+" colors;  "+m.colors32.Length+" color32");
		for(int i=0; i<m.vertexCount; i++){
			points[i] = new CloudPoint();
			points[i].pos = m.vertices[i];

			if(m.colors.Length > i){
				points[i].col = m.colors[i];
			}
			else{
				points[i].col = Color.red;
			}
		}
	}

	public void LoadPointsFromXyz(string f){
		string[] lines = f.Split('\n');
		
		int startLine = 0;
		int numPoints = lines.Length-1; //expects last line to be empty
		
		points = new CloudPoint[numPoints];
		GetComponent<ParticleSystem>().maxParticles = numPoints;
		if(numPoints <= 0){
			Debug.LogWarning("no points in the file");
			return;
		}
		
		//find the bounds and set transform to the center
		float minX=float.PositiveInfinity, maxX=float.NegativeInfinity, minY=float.PositiveInfinity, maxY=float.NegativeInfinity, minZ=float.PositiveInfinity, maxZ=float.NegativeInfinity;
		
		for(int i=0; i<numPoints; i++){
			string [] values = lines[i+startLine].Split(' ');
			points[i]=new CloudPoint();

			if(values.Length >= 6){

				points[i].pos = new Vector3( float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]) );
				points[i].col = new Color32(byte.Parse(values[3]), byte.Parse(values[4]), byte.Parse(values[5]), byte.MaxValue);
				
				//set min max
				if(points[i].pos.x > maxX) maxX = points[i].pos.x;
				if(points[i].pos.x < minX) minX = points[i].pos.x;
				if(points[i].pos.y > maxY) maxY = points[i].pos.y;
				if(points[i].pos.y < minY) minY = points[i].pos.y;
				if(points[i].pos.z > maxZ) maxZ = points[i].pos.z;
				if(points[i].pos.z < minZ) minZ = points[i].pos.z;
			}
			else{
				Debug.LogWarning("couldn't parse "+lines[i+startLine]);
			}
		}
		cloudDimensions = new Vector3(maxX - minX, maxY - minY, maxZ - minZ);
		Vector3 centerPos = new Vector3((minX+maxX)/2f, (minY+maxY)/2f, (minZ+maxZ)/2f);
		//Debug.Log("center:"+centerPos);
		foreach(CloudPoint cp in points){
			cp.pos -= centerPos;
		}
		
		// do not change center!
		//transform.position = centerPos;
	}

	
	public void LoadPointsFromPly(string f){
		string[] lines = f.Split('\n');
		
		int startLine = 0;
		int numPoints = 0;
		for(int i = 0; !lines[i].Equals("end_header"); i++){
			//read header
			if( lines[i].StartsWith("element vertex") ){
				numPoints = int.Parse(lines[i].Substring(15, lines[i].Length-15));
			}
			
			startLine = i+2;
		}

		points = new CloudPoint[numPoints];
		GetComponent<ParticleSystem>().maxParticles = numPoints;
		if(numPoints <= 0){
			Debug.LogWarning("no points in the file");
			return;
		}

		//find the bounds and set transform to the center
		float minX=float.PositiveInfinity, maxX=float.NegativeInfinity, minY=float.PositiveInfinity, maxY=float.NegativeInfinity, minZ=float.PositiveInfinity, maxZ=float.NegativeInfinity;
		
		for(int i=0; i<numPoints; i++){
			string [] values = lines[i+startLine].Split(' ');
			points[i]=new CloudPoint();

			points[i].pos = new Vector3( -float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]) );
			points[i].col = new Color32(byte.Parse(values[3]), byte.Parse(values[4]), byte.Parse(values[5]), byte.MaxValue);

			//set min max
			if(points[i].pos.x > maxX) maxX = points[i].pos.x;
			if(points[i].pos.x < minX) minX = points[i].pos.x;
			if(points[i].pos.y > maxY) maxY = points[i].pos.y;
			if(points[i].pos.y < minY) minY = points[i].pos.y;
			if(points[i].pos.z > maxZ) maxZ = points[i].pos.z;
			if(points[i].pos.z < minZ) minZ = points[i].pos.z;
		}
		cloudDimensions = new Vector3(maxX - minX, maxY - minY, maxZ - minZ);
		Vector3 centerPos = new Vector3((minX+maxX)/2f, (minY+maxY)/2f, (minZ+maxZ)/2f);
		//Debug.Log("center:"+centerPos);
		foreach(CloudPoint cp in points){
			cp.pos -= centerPos;
		}
		// do not change center!
		//transform.position = centerPos;
	}

	public void LoadPointsFromPts(string f){
		string[] lines = f.Split('\n');
		
		int startLine = 0;
		int numPoints = lines.Length - startLine;
		
		if(lines[0].Trim().Split(' ').Length == 1){
			//parse numPoints from first line
			numPoints = int.Parse(lines[0].Trim());
			startLine = 1;
		}

		//Debug.Log("num points: "+numPoints);
		points = new CloudPoint[numPoints];
		GetComponent<ParticleSystem>().maxParticles = numPoints;
		if(numPoints <= 0){
			Debug.LogWarning("no points in the file");
			return;
		}


		//find the bounds and set transform to the center

		float minX=float.PositiveInfinity, maxX=float.NegativeInfinity, minY=float.PositiveInfinity, maxY=float.NegativeInfinity, minZ=float.PositiveInfinity, maxZ=float.NegativeInfinity;
		
		for(int i=0; i<numPoints; i++){
		//for(int i=startLine; i<lines.Length; i++){
			string [] values = lines[i+startLine].Split(' ');
			//Debug.Log("values = "+lines[i+startLine]);
			points[i]=new CloudPoint();
			//rotate 90
			points[i].pos = new Vector3( float.Parse(values[0]), float.Parse(values[2]), -float.Parse(values[1]) );
			points[i].col = new Color32(byte.Parse(values[values.Length-3]), byte.Parse(values[values.Length-2]), byte.Parse(values[values.Length-1]), byte.MaxValue);

		    //aha! http://www.las-vegas.uni-osnabrueck.de/index.php/tutorials2/8-understanding-file-formats-pts-and-3d-files

			//set min max
			if(points[i].pos.x > maxX) maxX = points[i].pos.x;
			if(points[i].pos.x < minX) minX = points[i].pos.x;
			if(points[i].pos.y > maxY) maxY = points[i].pos.y;
			if(points[i].pos.y < minY) minY = points[i].pos.y;
			if(points[i].pos.z > maxZ) maxZ = points[i].pos.z;
			if(points[i].pos.z < minZ) minZ = points[i].pos.z;
		}
		cloudDimensions = new Vector3(maxX - minX, maxY - minY, maxZ - minZ);
		Vector3 centerPos = new Vector3((minX+maxX)/2f, (minY+maxY)/2f, (minZ+maxZ)/2f);
		//Debug.Log("center:"+centerPos);
		foreach(CloudPoint cp in points){
			cp.pos -= centerPos;
		}
		// do not change center!
		//transform.position = centerPos;

	}
	
	public void LoadPointsFromCloudComparePts(string f){
		string[] lines = f.Split('\n');
		
		int startLine = 0;
		int numPoints = lines.Length - startLine;
		
		if(lines[0].Trim().Split(' ').Length == 1){
			//parse numPoints from first line
			numPoints = int.Parse(lines[0].Trim());
			startLine = 1;
		}

		//Debug.Log("num points: "+numPoints);
		points = new CloudPoint[numPoints];
		GetComponent<ParticleSystem>().maxParticles = numPoints;
		if(numPoints <= 0){
			Debug.LogWarning("no points in the file");
			return;
		}


		//find the bounds and set transform to the center

		float minX=float.PositiveInfinity, maxX=float.NegativeInfinity, minY=float.PositiveInfinity, maxY=float.NegativeInfinity, minZ=float.PositiveInfinity, maxZ=float.NegativeInfinity;
		
		for(int i=0; i<numPoints; i++){
		//for(int i=startLine; i<lines.Length; i++){
			string [] values = lines[i+startLine].Split(' ');
			//Debug.Log("values = "+lines[i+startLine]);
			points[i]=new CloudPoint();
			//rotate 90
			points[i].pos = new Vector3( float.Parse(values[0]), float.Parse(values[2]), -float.Parse(values[1]) );
			points[i].col = new Color32(byte.Parse(values[3]), byte.Parse(values[4]), byte.Parse(values[5]), byte.MaxValue);

		    //aha! http://www.las-vegas.uni-osnabrueck.de/index.php/tutorials2/8-understanding-file-formats-pts-and-3d-files

			//set min max
			if(points[i].pos.x > maxX) maxX = points[i].pos.x;
			if(points[i].pos.x < minX) minX = points[i].pos.x;
			if(points[i].pos.y > maxY) maxY = points[i].pos.y;
			if(points[i].pos.y < minY) minY = points[i].pos.y;
			if(points[i].pos.z > maxZ) maxZ = points[i].pos.z;
			if(points[i].pos.z < minZ) minZ = points[i].pos.z;
		}
		cloudDimensions = new Vector3(maxX - minX, maxY - minY, maxZ - minZ);
		Vector3 centerPos = new Vector3((minX+maxX)/2f, (minY+maxY)/2f, (minZ+maxZ)/2f);
		//Debug.Log("center:"+centerPos);
		foreach(CloudPoint cp in points){
			cp.pos -= centerPos;
		}
		// do not change center!
		//transform.position = centerPos;

	}
	
	
}
