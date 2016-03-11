using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UI : MonoBehaviour {

	public Slider timeMultiplier;
	public InputField gravityMultiplier;
	public InputField cycleDurationMultiplier;
	public InputField strengthAmplitude;
	public InputField frictionAmplitude;
	public InputField numberOfMuscles;
	public InputField numberOfNodes;
	public Toggle randomNumbers;
	public Toggle generate;
	public InputField muscleReaction;

	void Start () {
		timeMultiplier.value = Constants.timeMultiplier;
		gravityMultiplier.text = Constants.gravityMultiplier.ToString ();
		cycleDurationMultiplier.text = Constants.cycleDurationMultiplier.ToString ();
		strengthAmplitude.text = Constants.strengthAmplitude.ToString ();
		frictionAmplitude.text = Constants.frictionAmplitude.ToString ();
		numberOfMuscles.text = Constants.numberOfMuscles.ToString ();
		numberOfNodes.text = Constants.numberOfNodes.ToString ();
		randomNumbers.isOn = Constants.randomNumbers;
		muscleReaction.text = Constants.musclesReaction.ToString ();
		generate.isOn = Constants.generate;

		timeMultiplier.onValueChanged.AddListener(UpdateUI);
		gravityMultiplier.onEndEdit.AddListener(UpdateUI);
		cycleDurationMultiplier.onEndEdit.AddListener(UpdateUI);
		strengthAmplitude.onEndEdit.AddListener(UpdateUI);
		frictionAmplitude.onEndEdit.AddListener(UpdateUI);
		numberOfMuscles.onEndEdit.AddListener(UpdateUI);
		numberOfNodes.onEndEdit.AddListener(UpdateUI);
		randomNumbers.onValueChanged.AddListener(UpdateUI);
		muscleReaction.onEndEdit.AddListener (UpdateUI);
		generate.onValueChanged.AddListener (UpdateUI);
	}

	public void UpdateUI(string s) {
		UpdateUI ();
	}

	public void UpdateUI (float value) {
		UpdateUI ();
	}

	public void UpdateUI (bool value) {
		UpdateUI ();
	}

	public void UpdateUI () {
		Constants.timeMultiplier = timeMultiplier.value;
		Constants.gravityMultiplier = int.Parse (gravityMultiplier.text);
		Constants.cycleDurationMultiplier = int.Parse (cycleDurationMultiplier.text);
		Constants.strengthAmplitude = int.Parse (strengthAmplitude.text);
		Constants.frictionAmplitude = int.Parse (frictionAmplitude.text);
		Constants.numberOfMuscles = int.Parse (numberOfMuscles.text);
		Constants.numberOfNodes = int.Parse (numberOfNodes.text);
		Constants.randomNumbers = randomNumbers.isOn;
		Constants.musclesReaction = float.Parse (muscleReaction.text);
		Constants.generate = generate.isOn;
	}
}

public static class Constants {
	public static float timeMultiplier = 1;
	public static float gravityMultiplier = 10;
	public static float cycleDurationMultiplier = 10;
	public static float tolerance = 0.001f;
	public static float strengthAmplitude = 20;
	public static float frictionAmplitude = 1;
	public static int numberOfMuscles = 3;
	public static int numberOfNodes = 1;
	public static bool randomNumbers = false;
	public static float musclesReaction = 2;
	public static bool generate = false;
	public static float contractedDistanceMultiplier = 3;
	public static float extendedDistanceMultiplier = 1;
	public static float bounciness = 0.1f;
	public static float minRandom = 0.01f;
	public static float minMass = 1;
	public static float maxMass = 10;
	public static float minStrength = 1;
}
