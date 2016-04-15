using UnityEngine;
using UnityEngine.UI;

namespace Evolution
{
    public class UI : MonoBehaviour
    {
        public Slider TimeMultiplier;
        public InputField FrictionAmplitude, NumberOfMuscles, NumberOfNodes, Generations;
        public Toggle RandomNumbers;


        public void Start()
        {
            ForceUIUpdate();
            TimeMultiplier.onValueChanged.AddListener(UpdateUI);
            FrictionAmplitude.onEndEdit.AddListener(UpdateUI);
            NumberOfMuscles.onEndEdit.AddListener(UpdateUI);
            NumberOfNodes.onEndEdit.AddListener(UpdateUI);
            RandomNumbers.onValueChanged.AddListener(UpdateUI);
            Generations.onValueChanged.AddListener(UpdateUI);
        }

        public void ForceUIUpdate()
        {
            TimeMultiplier.value = Mathf.Log10(Constants.TimeMultiplier);
            FrictionAmplitude.text = Constants.Friction.ToString();
            NumberOfMuscles.text = Constants.NumberOfMuscles.ToString();
            NumberOfNodes.text = Constants.NumberOfNodes.ToString();
            RandomNumbers.isOn = Constants.RandomNumbers;
            Generations.text = Constants.Generations.ToString();
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
            Constants.Friction = int.Parse(FrictionAmplitude.text);
            Constants.NumberOfMuscles = int.Parse(NumberOfMuscles.text);
            Constants.NumberOfNodes = int.Parse(NumberOfNodes.text);
            Constants.RandomNumbers = RandomNumbers.isOn;
            Constants.Generations = int.Parse(Generations.text);
        }
    }
}