using System;
using UnityEngine;
using System.Collections.Generic;

public class Creature {
	#region private variables
	private List<Muscle> muscles;
	private List<Node> nodes;

	private float cycleDuration;
	private float time;
	#endregion

	public Creature (List<Muscle> muscles, List<Node> nodes, float cycleDuration) {
		this.muscles = muscles;
		this.nodes = nodes;
		this.cycleDuration = cycleDuration;
		time = 0;
	}

	public void Update (float deltaTime) {
		/*
		 * Time modulo cycle duration
		 */
		time = (time - cycleDuration * (Mathf.FloorToInt (time / cycleDuration)));

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
			time += deltaTime;
		}
	}
}