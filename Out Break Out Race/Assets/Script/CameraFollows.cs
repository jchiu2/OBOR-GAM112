using UnityEngine;
using System.Collections;

public class CameraFollows : MonoBehaviour {
	public Transform target;

	private Vector3 tarPosi, camPosi;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void LateUpdate () {
		tarPosi = target.position; //trrack the player
		camPosi = new Vector3 (tarPosi.x, tarPosi.y, -10); //camera new position without interrupting the Z axis of the camera.
		transform.position = camPosi;
	}
}
