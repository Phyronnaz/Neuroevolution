using UnityEngine;
using UnityEngine.UI;
using ProgressBar;

namespace Assets.Scripts.Neuroevolution
{
    public class MainUI : MonoBehaviour
    {
        public Slider TimeMultiplierSlider;
        public InputField GenerationsField;
        public InputField TestDurationField;
        public InputField InitialPopulationSizeField;
        public InputField VariationField;
        public InputField FileNameField;
        public Text DistanceText;
        public Text TimeText;
        public Text SpeedText;
        public Text EnergyText;
        public Text PowerText;
        public Text TimeRemainingText;
        public ProgressBarBehaviour ProgressBar;
        private Main main;
        //Progress bar
        private float remainingTime;
        private float startTrainTime;
        private int totalGenerations;

        private void Awake()
        {
            main = GetComponent<Main>();
        }

        private void ResetRemainingTime()
        {
            remainingTime = Mathf.Infinity;
        }

        public int GetTimeMultiplier()
        {
            return (int)Mathf.Exp(TimeMultiplierSlider.value);
        }

        public void TrainUpdate(float progression)
        {
            ProgressBar.gameObject.SetActive(true);
            if (Input.GetKeyDown(KeyCode.Return))
            {
                remainingTime = Mathf.Infinity;
            }
            //Update progress bar
            ProgressBar.SetFillerSizeAsPercentage(progression * 100f);
            //Time text
            var speed = (Time.time - startTrainTime) / (progression * totalGenerations);
            remainingTime = Mathf.Min(speed * (totalGenerations - progression * totalGenerations), remainingTime);
            TimeRemainingText.text = (int)remainingTime + "s remaining";
        }

        public void NormalUpdate(Creature bestCreature, float currentTime)
        {
            ProgressBar.gameObject.SetActive(false);

            // Update camera position
            var tmp = transform.position;
            tmp.x = Mathf.Lerp(tmp.x, bestCreature.GetAveragePosition().X + 5, Time.deltaTime * Mathf.Exp(TimeMultiplierSlider.value) + 0.01f);
            tmp.y = Mathf.Lerp(tmp.y, bestCreature.GetAveragePosition().Y + 5, Time.deltaTime * Mathf.Exp(TimeMultiplierSlider.value) + 0.01f);
            transform.position = tmp;

            //Update UI
            var m = bestCreature.GetAveragePosition().X.ToString();
            if (m.Length > 5)
                m = m.Substring(0, 5);

            var t = currentTime.ToString();
            if (t.Length > 7)
                t = t.Substring(0, 7);

            var s = (bestCreature.GetAveragePosition().X / currentTime).ToString();
            if (s.Length > 6)
                s = s.Substring(0, 6);

            var e = bestCreature.GetEnergy().ToString();
            if (e.Length > 5)
                e = e.Substring(0, 5);

            var p = bestCreature.GetPower().ToString();
            if (p.Length > 5)
                p = p.Substring(0, 5);

            DistanceText.text = "Distance : " + m;
            TimeText.text = "Time : " + t;
            SpeedText.text = "Speed : " + s;
            EnergyText.text = "Energy : " + e;
            PowerText.text = "Power : " + p;
        }


        /*
         * UI
         */
        public void OnPopulationSizeChange()
        {
            if (int.Parse(InitialPopulationSizeField.text) % 5 != 0)
            {
                InitialPopulationSizeField.text = (int.Parse(InitialPopulationSizeField.text) + 5 - int.Parse(InitialPopulationSizeField.text) % 5).ToString();
            }
        }

        public void Train()
        {
            startTrainTime = Time.time;
            totalGenerations = int.Parse(GenerationsField.text);
            ResetRemainingTime();
            main.Train(int.Parse(InitialPopulationSizeField.text), int.Parse(GenerationsField.text), int.Parse(TestDurationField.text), float.Parse(VariationField.text), FileNameField.text);
        }
    }
}
