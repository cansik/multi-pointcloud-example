using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleCloud : MonoBehaviour
{
	public TextAsset pointCloudFile;

	[Range(0.1f, 5f)]
	public float pointSize = 0.1f;

	public Material pointsMaterial;

	private PointCloud _pointCloud;

	// Use this for initialization
	void Start () {
		_pointCloud = gameObject.AddComponent<PointCloud>();
		_pointCloud.GetComponent<ParticleSystem>().GetComponent<Renderer>().material = pointsMaterial;
		_pointCloud.pointSize = pointSize;
		
		_pointCloud.LoadPointsFromCloudComparePts(pointCloudFile.text);
	}
	
	// Update is called once per frame
	void Update ()
	{
		_pointCloud.pointSize = pointSize;
	}
	
	public void ResetParticlesLater(){
		_pointCloud.ResetParticles();
	}
}
