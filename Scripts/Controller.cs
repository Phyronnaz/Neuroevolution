using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Controller : MonoBehaviour {

	#region variables
	public List<Node> nodes;
	public List<Muscle> muscles;

	public Text cycleText;
	public Text distanceText;
	public Text timeText;

	public float cycleDuration;

	private float currentTime = 0;
	private float time = 0;
	private float deltaTime = 0.001f;
	#endregion


	#region Start && Update && CustomUpdate
	void Start () {
		InvokeRepeating ("CustomUpdate", 0, deltaTime);
	}

	void CustomUpdate () {
		/*
		 * time modulo cycle duration
		 */
		time = (currentTime - cycleDuration * (Mathf.FloorToInt (currentTime / cycleDuration)));

		for (var k = 0; k < Constants.timeMultiplier; k++) {
			/*
			 * Update muscles and nodes
			 */
			foreach (var m in muscles) {
				m.Update (time);
			}
			foreach (var n in nodes) {
				n.Update (deltaTime);
			}
			foreach (var m in muscles) {
				m.LateUpdate ();
			}
			foreach (var n in nodes) {
				n.LateUpdate ();
			}
		
			/*
			* Update current time
			*/
			currentTime += deltaTime;
		}
	}

	void Update () {
		/*
		 * Update average position
		 */
		float avPosition = 0;
		foreach(var n in nodes) {
			avPosition += n.position.x;
		}
		avPosition /= nodes.Count;

		var tmp = transform.position;
		tmp.x = avPosition;
		transform.position = tmp;

		/*
		 * Update UI
		 */
		distanceText.text = "Distance : " + avPosition.ToString ();
		timeText.text = "Time : " + currentTime.ToString ();
		cycleText.text = (Mathf.Ceil (time / cycleDuration * 100)).ToString () + " %";
	}
	#endregion
}