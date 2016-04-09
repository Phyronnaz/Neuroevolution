using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class ControllerScript : MonoBehaviour
{

	public Text CycleText;
	public Text DistanceText;
	public Text TimeText;

	Controller controller;

	public void Initialize (List<Creature> creatures)
	{
		controller = new Controller (creatures);
		InvokeRepeating ("ControllerUpdate", 0, Controller.DeltaTime);
	}

	void ControllerUpdate ()
	{
		controller.Update ();
	}

	void Update ()
	{
		var max = controller.GetMaxPosition ();

		// Update average position
		var tmp = transform.position;
		tmp.x = Mathf.Lerp (tmp.x, max, Time.deltaTime * Constants.TimeMultiplier / 10 + 0.01f);
		transform.position = tmp;

		//Remove slowest creatures
		controller.RemoveCreaturesFartherThan (50, max);

		//Update UI
		DistanceText.text = "Distance : " + max;
		CycleText.text = string.Format ("{0} %", controller.GetCyclePercentageOfTheFarthestCreatures ());
		TimeText.text = "Time : " + controller.CurrentTime;

		if (Input.GetKeyDown (KeyCode.S)) {
			Constants.TimeMultiplier = 100;
			GetComponent<UI> ().ForceUIUpdate ();
		}

		if (Input.GetKeyDown (KeyCode.A)) {
			CancelInvoke ();
			controller.ResetCreatures ();
			InvokeRepeating ("ControllerUpdate", 0, Controller.DeltaTime);
		}


		if (Input.GetKeyDown (KeyCode.R)) {
			CancelInvoke ();
			controller.Destroy ();
			Destroy (this);
			GetComponent<Generate> ().Restart ();
		}
	}
}
