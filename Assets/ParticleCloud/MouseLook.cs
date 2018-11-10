using UnityEngine;
using System.Collections;

/// MouseLook rotates the transform based on the mouse delta.
/// Minimum and Maximum values can be used to constrain the possible rotation

/// To make an FPS style character:
/// - Create a capsule.
/// - Add the MouseLook script to the capsule.
///   -> Set the mouse look to use LookX. (You want to only turn character but not tilt it)
/// - Add FPSInputController script to the capsule
///   -> A CharacterMotor and a CharacterController component will be automatically added.

/// - Create a camera. Make the camera a child of the capsule. Reset it's transform.
/// - Add a MouseLook script to the camera.
///   -> Set the mouse look to use LookY. (You want the camera to tilt up and down like a head. The character already turns.)
[AddComponentMenu("Camera-Control/Mouse Look")]
public class MouseLook : MonoBehaviour {
	
	public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
	public RotationAxes axes = RotationAxes.MouseXAndY;
	public float sensitivityX = 15F;
	public float sensitivityY = 15F;
	
	public float minimumX = -360F;
	public float maximumX = 360F;
	
	public float minimumY = -60F;
	public float maximumY = 60F;
	
	float rotationY = 0F;
	
	void Update ()
	{
		if (axes == RotationAxes.MouseXAndY)
		{
			float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;
			
			rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
			rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
			
			transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
		}
		else if (axes == RotationAxes.MouseX)
		{
			transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityX, 0);
		}
		else
		{
			rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
			rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
			
			transform.localEulerAngles = new Vector3(-rotationY, transform.localEulerAngles.y, 0);
		}

		if(timeToDestination>0f){
			timeToDestination-=Time.deltaTime;

			Camera.main.transform.position = Vector3.Lerp(endPos, startPos, timeToDestination/totalTimeToDest);
		}
		else if(Input.GetMouseButtonDown(0)){

			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, Mathf.Infinity)){
				Screen.lockCursor = true;
				Screen.lockCursor = false;

				Vector3 pos = hit.point;
				endPos = pos + new Vector3(0f, 2f, 0f);
				//Camera.main.transform.position = endPos; //instant move

				startPos = transform.position;
				timeToDestination = totalTimeToDest;
			}
			else{
				//reset cursor to center anyways
				Screen.lockCursor = true;
				Screen.lockCursor = false;
				if(birdViewPos!=null){
					
					endPos = birdViewPos.position;
					
					startPos = transform.position;
					timeToDestination = totalTimeToDest;
				}
			}
		}
		else if(Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)){
			Camera.main.transform.position = Camera.main.transform.position + Camera.main.transform.forward * Time.deltaTime * arrowSpeed;
		}
		else if(Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)){
			Camera.main.transform.position = Camera.main.transform.position - Camera.main.transform.forward * Time.deltaTime * arrowSpeed;
		}
	}

	public float arrowSpeed = 30f;

	private float timeToDestination = 0f;
	private float totalTimeToDest = 0.5f;
	private Vector3 startPos;
	private Vector3 endPos;
	public Transform birdViewPos;
	
	void Start ()
	{
		//if(!networkView.isMine)
		//enabled = false;
		
		// Make the rigid body not change rotation
		//if (rigidbody)
		//rigidbody.freezeRotation = true;
	}
}