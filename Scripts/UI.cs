using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
	public Slider TimeMultiplier;
	public InputField GravityMultiplier, CycleDurationMultiplier, StrengthAmplitude, FrictionAmplitude, NumberOfMuscles, NumberOfNodes;
	public Toggle RandomNumbers, Generate;

	void Start ()
	{
		TimeMultiplier.onValueChanged.AddListener (UpdateUI);
		GravityMultiplier.onEndEdit.AddListener (UpdateUI);
		CycleDurationMultiplier.onEndEdit.AddListener (UpdateUI);
		StrengthAmplitude.onEndEdit.AddListener (UpdateUI);
		FrictionAmplitude.onEndEdit.AddListener (UpdateUI);
		NumberOfMuscles.onEndEdit.AddListener (UpdateUI);
		NumberOfNodes.onEndEdit.AddListener (UpdateUI);
		RandomNumbers.onValueChanged.AddListener (UpdateUI);
		Generate.onValueChanged.AddListener (UpdateUI);

		ForceUIUpdate ();
	}

	public void ForceUIUpdate ()
	{
		TimeMultiplier.value = Mathf.Log10 (Constants.TimeMultiplier);
		GravityMultiplier.text = Constants.GravityMultiplier.ToString ();
		CycleDurationMultiplier.text = Constants.CycleDurationMultiplier.ToString ();
		StrengthAmplitude.text = (Constants.StrengthAmplitude * 10000).ToString ();
		FrictionAmplitude.text = Constants.Friction.ToString ();
		NumberOfMuscles.text = Constants.NumberOfMuscles.ToString ();
		NumberOfNodes.text = Constants.NumberOfNodes.ToString ();
		RandomNumbers.isOn = Constants.RandomNumbers;
		Generate.isOn = Constants.Generate;
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
		Constants.TimeMultiplier = (int)Mathf.Pow (10, TimeMultiplier.value);
		Constants.GravityMultiplier = int.Parse (GravityMultiplier.text);
		Constants.CycleDurationMultiplier = int.Parse (CycleDurationMultiplier.text);
		Constants.StrengthAmplitude = int.Parse (StrengthAmplitude.text) / 10000;
		Constants.Friction = int.Parse (FrictionAmplitude.text);
		Constants.NumberOfMuscles = int.Parse (NumberOfMuscles.text);
		Constants.NumberOfNodes = int.Parse (NumberOfNodes.text);
		Constants.RandomNumbers = RandomNumbers.isOn;
		Constants.Generate = Generate.isOn;
	}
}

public static class Constants
{
	public static int TimeMultiplier = 1;
	public static float GravityMultiplier = 50;
	public static float CycleDurationMultiplier = 10f;
	public static float Tolerance = 0.001f;
	public static float StrengthAmplitude = 10f / 10000f;
	public static float Friction = 1000;
	public static int NumberOfMuscles = 3;
	public static int NumberOfNodes = 3;
	public static bool RandomNumbers = true;
	public static bool Generate;
	public static float ContractedDistanceMultiplier = 2;
	public static float ExtendedDistanceMultiplier = 2;
	public static float Bounciness = 0.6f;
	public static float MinRandom = 0.00001f;
	public static float MinMass = 1;
	public static float MaxMass = 10;
	public static float MinStrength = 1;
	public static bool MuscleDebug = true;
	public static bool NeuralNetwork;
	public static int NumberOfThreads = 32;
}
