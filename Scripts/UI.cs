using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{

	[SerializeField]
	Slider timeMultiplier;
	[SerializeField]
	InputField gravityMultiplier, cycleDurationMultiplier, strengthAmplitude, frictionAmplitude, numberOfMuscles, numberOfNodes, fluidFriction;
	[SerializeField]
	Toggle randomNumbers, generate;

	void Start ()
	{
		timeMultiplier.value = Constants.TimeMultiplier;
		gravityMultiplier.text = Constants.GravityMultiplier.ToString ();
		cycleDurationMultiplier.text = Constants.CycleDurationMultiplier.ToString ();
		strengthAmplitude.text = (Constants.StrengthAmplitude * 10000).ToString ();
		frictionAmplitude.text = Constants.FrictionAmplitude.ToString ();
		numberOfMuscles.text = Constants.NumberOfMuscles.ToString ();
		numberOfNodes.text = Constants.NumberOfNodes.ToString ();
		randomNumbers.isOn = Constants.RandomNumbers;
		fluidFriction.text = Constants.FluidFriction.ToString ();
		generate.isOn = Constants.Generate;

		timeMultiplier.onValueChanged.AddListener (UpdateUI);
		gravityMultiplier.onEndEdit.AddListener (UpdateUI);
		cycleDurationMultiplier.onEndEdit.AddListener (UpdateUI);
		strengthAmplitude.onEndEdit.AddListener (UpdateUI);
		frictionAmplitude.onEndEdit.AddListener (UpdateUI);
		numberOfMuscles.onEndEdit.AddListener (UpdateUI);
		numberOfNodes.onEndEdit.AddListener (UpdateUI);
		randomNumbers.onValueChanged.AddListener (UpdateUI);
		fluidFriction.onEndEdit.AddListener (UpdateUI);
		generate.onValueChanged.AddListener (UpdateUI);
	}

	public void UpdateUI (string s)
	{
		UpdateUI ();
	}

	public void UpdateUI (float value)
	{
		UpdateUI ();
	}

	public void UpdateUI (bool value)
	{
		UpdateUI ();
	}

	public void UpdateUI ()
	{
		Constants.TimeMultiplier = timeMultiplier.value;
		Constants.GravityMultiplier = int.Parse (gravityMultiplier.text);
		Constants.CycleDurationMultiplier = int.Parse (cycleDurationMultiplier.text);
		Constants.StrengthAmplitude = int.Parse (strengthAmplitude.text) / 10000;
		Constants.FrictionAmplitude = int.Parse (frictionAmplitude.text);
		Constants.NumberOfMuscles = int.Parse (numberOfMuscles.text);
		Constants.NumberOfNodes = int.Parse (numberOfNodes.text);
		Constants.RandomNumbers = randomNumbers.isOn;
		Constants.FluidFriction = float.Parse (fluidFriction.text);
		Constants.Generate = generate.isOn;
	}
}

public static class Constants
{
	public static float TimeMultiplier = 1;
	public static float GravityMultiplier = 50;
	public static float CycleDurationMultiplier = 10f;
	public static float Tolerance = 0.001f;
	public static float StrengthAmplitude = 10f / 10000f;
	//TODO: remove
	public static float FrictionAmplitude = 1000;
	public static int NumberOfMuscles = 3;
	public static int NumberOfNodes = 3;
	public static bool RandomNumbers = true;
	public static bool Generate;
	public static float FluidFriction = 0.1f;
	public static float ContractedDistanceMultiplier = 2;
	public static float ExtendedDistanceMultiplier = 2;
	public static float Bounciness = 0.6f;
	public static float MinRandom = 0.00001f;
	public static float MinMass = 1;
	public static float MaxMass = 10;
	public static float MinStrength = 1;
	public static bool MuscleDebug = true;
	public static bool NeuralNetwork = false;
}
