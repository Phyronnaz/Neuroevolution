using UnityEngine;
using UnityEngine.UI;

namespace Evolution
{
    public class UI : MonoBehaviour
    {
        public Slider TimeMultiplier;
        public InputField GravityMultiplier, CycleDurationMultiplier, StrengthAmplitude, FrictionAmplitude, NumberOfMuscles, NumberOfNodes;
        public Toggle RandomNumbers;


        public void Start()
        {
            ForceUIUpdate();
            TimeMultiplier.onValueChanged.AddListener(UpdateUI);
            GravityMultiplier.onEndEdit.AddListener(UpdateUI);
            CycleDurationMultiplier.onEndEdit.AddListener(UpdateUI);
            StrengthAmplitude.onEndEdit.AddListener(UpdateUI);
            FrictionAmplitude.onEndEdit.AddListener(UpdateUI);
            NumberOfMuscles.onEndEdit.AddListener(UpdateUI);
            NumberOfNodes.onEndEdit.AddListener(UpdateUI);
            RandomNumbers.onValueChanged.AddListener(UpdateUI);
        }

        public void ForceUIUpdate()
        {
            TimeMultiplier.value = Mathf.Log10(Constants.TimeMultiplier);
            GravityMultiplier.text = Constants.GravityMultiplier.ToString();
            CycleDurationMultiplier.text = Constants.CycleDurationMultiplier.ToString();
            StrengthAmplitude.text = (Constants.StrengthAmplitude * 10000).ToString();
            FrictionAmplitude.text = Constants.Friction.ToString();
            NumberOfMuscles.text = Constants.NumberOfMuscles.ToString();
            NumberOfNodes.text = Constants.NumberOfNodes.ToString();
            RandomNumbers.isOn = Constants.RandomNumbers;
        }

        public void UpdateUI(string s)
        {
            UpdateUI();
        }

        public void UpdateUI(float value)
        {
            UpdateUI();
        }

        public void UpdateUI(bool value)
        {
            UpdateUI();
        }

        public void UpdateUI()
        {
            Constants.TimeMultiplier = (int)Mathf.Pow(10, TimeMultiplier.value);
            Constants.GravityMultiplier = int.Parse(GravityMultiplier.text);
            Constants.CycleDurationMultiplier = int.Parse(CycleDurationMultiplier.text);
            Constants.StrengthAmplitude = int.Parse(StrengthAmplitude.text) / 10000;
            Constants.Friction = int.Parse(FrictionAmplitude.text);
            Constants.NumberOfMuscles = int.Parse(NumberOfMuscles.text);
            Constants.NumberOfNodes = int.Parse(NumberOfNodes.text);
            Constants.RandomNumbers = RandomNumbers.isOn;
        }
    }
}