using UnityEngine;
using UnityEngine.UI;
using ProgressBar;
using System.Collections.Generic;

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
        public List<InputField> SpeciesSizesFields = new List<InputField>();
        public Text DistanceText;
        public Text TimeText;
        public Text SpeedText;
        public Text EnergyText;
        public Text PowerText;
        public Text TimeRemainingText;
        public Text AngleText;
        public ProgressBarBehaviour ProgressBar;
        public GameObject GlobalsPanel;
        public GameObject TrainPanel;
        public GameObject SpeciesPanel;
        private Main main;
        //Progress bar
        private float remainingTime;
        private float startTrainTime;
        private int totalGenerations;
        private float counter;

        public void Awake()
        {
            main = GetComponent<Main>();
            for (var i = 0; i < Globals.SpeciesSizes.Count; i++)
            {
                if (i < SpeciesSizesFields.Count)
                {
                    SpeciesSizesFields[i].text = Globals.SpeciesSizes[i].ToString();
                }
            }
        }

        public int GetTimeMultiplier()
        {
            counter += Mathf.Exp(TimeMultiplierSlider.value);
            if (counter > 1)
            {
                var x = counter;
                counter = 0;
                return (int)x;
            }
            else
            {
                return 0;
            }
        }

        public void TrainUpdate(float progression)
        {
            if (Input.GetKey(KeyCode.Return))
            {
                ResetRemainingTime();
            }
            ProgressBar.gameObject.SetActive(true);
            //Update progress bar
            ProgressBar.SetFillerSizeAsPercentage(progression * 100f);
            //Time text
            var speed = (Time.time - startTrainTime) / (progression * totalGenerations);
            remainingTime = Mathf.Min(speed * (totalGenerations - progression * totalGenerations), remainingTime);
            TimeRemainingText.text = (int)remainingTime + "s remaining";
        }

        public void Update()
        {
            OnPopulationSizeChange();
        }

        public void NormalUpdate(Creature bestCreature, float currentTime)
        {
            ProgressBar.gameObject.SetActive(false);

            // Update camera position
            var tmp = transform.position;
            tmp.x = Mathf.Lerp(tmp.x, bestCreature.GetAveragePosition().X, Mathf.Abs(tmp.x - bestCreature.GetAveragePosition().X) * 0.1f);
            tmp.y = Mathf.Lerp(tmp.y, bestCreature.GetAveragePosition().Y, Mathf.Abs(tmp.y - bestCreature.GetAveragePosition().Y) * 0.1f);
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

            var a = bestCreature.GetAngle().ToString();
            if (a.Length > 5)
                a = a.Substring(0, 5);

            DistanceText.text = "Distance : " + m;
            TimeText.text = "Time : " + t;
            SpeedText.text = "Speed : " + s;
            EnergyText.text = "Energy : " + e;
            PowerText.text = "Power : " + p;
            AngleText.text = "Angle : " + a;
        }

        public void HideUI()
        {
            GlobalsPanel.SetActive(!GlobalsPanel.activeSelf);
            TrainPanel.SetActive(!TrainPanel.activeSelf);
            SpeciesPanel.SetActive(!SpeciesPanel.activeSelf);
        }

        public void OnPopulationSizeChange()
        {
            if (Globals.UseSpecies)
            {
                var x = 0;
                foreach (var i in SpeciesSizesFields)
                {
                    int a;
                    if (int.TryParse(i.text, out a))
                    {
                        x += a;
                    }
                }
                InitialPopulationSizeField.text = (2 * x + Globals.RandomCount).ToString();
            }
        }

        public void StartTrain()
        {
            var l = new List<int>();
            foreach (var i in SpeciesSizesFields)
            {
                var x = int.Parse(i.text);
                if (x != 0)
                {
                    l.Add(x);
                }
            }
            Globals.SpeciesSizes = l;

            startTrainTime = Time.time;
            totalGenerations = int.Parse(GenerationsField.text);
            ResetRemainingTime();
            main.Train(int.Parse(InitialPopulationSizeField.text), int.Parse(GenerationsField.text), int.Parse(TestDurationField.text), float.Parse(VariationField.text), FileNameField.text);
        }

        private void ResetRemainingTime()
        {
            remainingTime = Mathf.Infinity;
        }
    }
}
