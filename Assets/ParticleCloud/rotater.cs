using UnityEngine;
using System.Collections;

public class rotater : MonoBehaviour {
	public float speed=1f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(Vector3.up * speed * Time.deltaTime);
	}
	
	void OnGUI() {
		speed = GUI.HorizontalSlider(new Rect(50, Screen.height-100, 220, 50), speed, 0.0f, 20f);
	}
}
