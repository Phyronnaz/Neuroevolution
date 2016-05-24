using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProgressBar;
using System.Xml.Serialization;
using System.IO;
using System.Windows.Forms;
using System;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Neuroevolution
{
    public class Main : MonoBehaviour
    {
        //UI
        public Slider TimeMultiplierSlider;
        public InputField GenerationsField;
        public InputField TestDurationField;
        public InputField InitialPopulationSizeField;
        public InputField VariationField;
        public InputField HiddenLayersCountField;
        public InputField FileNameField;
        public Text DistanceText;
        public Text TimeText;
        public Text SpeedText;
        public Text EnergyText;
        public Text PowerText;
        public Text TimeRemainingText;
        public ProgressBarBehaviour ProgressBar;
        private List<CreatureRenderer> creatureRenderers;
        //Others
        private Controller controller;
        private Editor editor;
        private bool edit;
        private const int hiddenSize = 4; //Auto increased if needed
        private float remainingTime = Mathf.Infinity;
        private bool isPaused;


        public void Awake()
        {
            InvokeRepeating("ControllerUpdate", 0, Globals.DeltaTime);
        }
        private void ControllerUpdate()
        {
            if (controller != null && !controller.IsTraining && !isPaused)
            {
                controller.Update((int)Mathf.Exp(TimeMultiplierSlider.value));
                //Pause at 20s
                if (controller.CurrentTime <= 20 && controller.CurrentTime + (int)Mathf.Exp(TimeMultiplierSlider.value) * Globals.DeltaTime >= 20)
                {
                    isPaused = true;
                }
            }
        }

        public void Start()
        {
            ProgressBar.gameObject.SetActive(false);
            edit = true;
            editor = new Editor();
            creatureRenderers = new List<CreatureRenderer>();
            var tmp = transform.position;
            tmp.x = 0;
            transform.position = tmp;
        }


        public void Train()
        {
            //Called by UI


            remainingTime = Mathf.Infinity;
            if (controller == null)
            {
                InitializeController();
            }

            //Adjust creatures number
            while (controller.Creatures.Count < int.Parse(InitialPopulationSizeField.text))
            {
                var r = controller.Creatures[CustomRandom.Range(0, controller.Creatures.Count)];
                controller.Creatures.Add(r.RandomClone());
            }
            while (controller.Creatures.Count > int.Parse(InitialPopulationSizeField.text))
            {
                controller.Creatures.RemoveAt(CustomRandom.Range(0, controller.Creatures.Count));
            }

            //Begin train
            controller.Train(int.Parse(GenerationsField.text), int.Parse(TestDurationField.text), float.Parse(VariationField.text), FileNameField.text);
        }

        public void OnPopulationSizeChange()
        {
            if (int.Parse(InitialPopulationSizeField.text) % 5 != 0)
            {
                InitialPopulationSizeField.text = (int.Parse(InitialPopulationSizeField.text) + 5 - int.Parse(InitialPopulationSizeField.text) % 5).ToString();
            }
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.F11))
            {
                UnityEngine.Screen.fullScreen = !UnityEngine.Screen.fullScreen;
            }
            if (edit)
            {
                EditUpdate();
            }
            else
            {
                PlayUpdate();
            }
        }

        public void EditUpdate()
        {
            editor.Update();
            if (!GameObject.Find("EventSystem").GetComponent<EventSystem>().IsPointerOverGameObject())
            {
                //Start
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    InitializeController();
                }
                //Restart
                if (Input.GetKeyDown(KeyCode.R))
                {
                    editor.Destroy();
                    editor = new Editor();
                }
                //Load file
                if (Input.GetKeyDown(KeyCode.L))
                {
                    Stream myStream = null;
                    OpenFileDialog openFileDialog1 = new OpenFileDialog();

                    openFileDialog1.InitialDirectory = "c:\\";
                    openFileDialog1.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
                    openFileDialog1.FilterIndex = 1;
                    openFileDialog1.RestoreDirectory = true;

                    if (openFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            if ((myStream = openFileDialog1.OpenFile()) != null)
                            {
                                using (myStream)
                                {
                                    XmlSerializer serializer = new XmlSerializer(typeof(CreatureSaveStruct));
                                    var c = (CreatureSaveStruct)serializer.Deserialize(myStream);
                                    controller = new Controller(c.ToCreature());

                                    edit = false;
                                    editor.Destroy();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                        }
                    }
                }
            }
        }
        public void InitializeController()
        {
            var ct = new CreatureStruct(editor.GetPositions(), editor.GetDistanceJoints(), editor.GetRevoluteJoints(), editor.GetRotationNode());
            var c = new Creature(ct, hiddenSize, int.Parse(HiddenLayersCountField.text));
            edit = false;
            editor.Destroy();
            controller = new Controller(c);
            RenderCreatures();
        }

        public void PlayUpdate()
        {
            if (controller.IsTraining)
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    remainingTime = Mathf.Infinity;
                }
                //Progress bar
                ProgressBar.gameObject.SetActive(true);
                ProgressBar.ProgressSpeed = 10000;
                ProgressBar.SetFillerSizeAsPercentage((float)controller.CurrentGeneration / controller.TotalGenerations * 100f);
                //Time
                var speed = (Time.time - controller.TrainStartTime) / controller.CurrentGeneration;
                var x = Mathf.Min(speed * (controller.TotalGenerations - controller.CurrentGeneration), remainingTime);
                if (x > 1)
                {
                    remainingTime = x;
                }
                TimeRemainingText.text = (int)remainingTime + "s remaining";
            }
            else
            {
                ProgressBar.gameObject.SetActive(false);

                var max = controller.GetBestCreature();

                // Update camera position
                var tmp = transform.position;
                tmp.x = Mathf.Lerp(tmp.x, max.GetAveragePosition().X + 5, Time.deltaTime * Mathf.Exp(TimeMultiplierSlider.value) + 0.01f);
                tmp.y = Mathf.Lerp(tmp.y, max.GetAveragePosition().Y + 5, Time.deltaTime * Mathf.Exp(TimeMultiplierSlider.value) + 0.01f);
                transform.position = tmp;

                //Remove slowest creatures
                controller.RemoveCreaturesFartherThan(100);

                //Update UI
                var m = max.GetAveragePosition().X.ToString();
                if (m.Length > 5)
                    m = m.Substring(0, 5);

                var t = controller.CurrentTime.ToString();
                if (t.Length > 7)
                    t = t.Substring(0, 7);

                var s = (max.GetAveragePosition().X / controller.CurrentTime).ToString();
                if (s.Length > 6)
                    s = s.Substring(0, 6);

                var e = max.GetEnergy().ToString();
                if (e.Length > 5)
                    e = e.Substring(0, 5);

                var p = max.GetPower().ToString();
                if (p.Length > 5)
                    p = p.Substring(0, 5);

                DistanceText.text = "Distance : " + m;
                TimeText.text = "Time : " + t;
                SpeedText.text = "Speed : " + s;
                EnergyText.text = "Energy : " + e;
                PowerText.text = "Power : " + p;

                //Render creatures
                RenderCreatures();


                if (!GameObject.Find("EventSystem").GetComponent<EventSystem>().IsPointerOverGameObject())
                {
                    //Input
                    if (Input.GetKeyDown(KeyCode.A))
                    {
                        controller.ResetCreatures();
                        controller.CurrentTime = 0;
                    }
                    if (Input.GetKeyDown(KeyCode.P))
                    {
                        isPaused = !isPaused;
                    }
                    if (Input.GetKeyDown(KeyCode.R))
                    {
                        foreach (var c in creatureRenderers)
                        {
                            c.Destroy();
                        }
                        Start();
                    }
                    if (Input.GetKeyDown(KeyCode.S))
                    {
                        Stream myStream;
                        SaveFileDialog saveFileDialog1 = new SaveFileDialog();

                        saveFileDialog1.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
                        saveFileDialog1.FilterIndex = 1;
                        saveFileDialog1.RestoreDirectory = true;

                        if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                        {
                            if ((myStream = saveFileDialog1.OpenFile()) != null)
                            {
                                var save = controller.GetBestCreature().GetSave();
                                XmlSerializer serializer = new XmlSerializer(typeof(CreatureSaveStruct));
                                serializer.Serialize(myStream, save);
                                myStream.Close();
                            }
                        }
                    }
                }
            }
        }
        private void RenderCreatures()
        {
            while (creatureRenderers.Count < controller.Creatures.Count)
            {
                creatureRenderers.Add(new CreatureRenderer());
            }
            while (creatureRenderers.Count > controller.Creatures.Count)
            {
                creatureRenderers[0].Destroy();
                creatureRenderers.RemoveAt(0);
            }
            for (var k = 0; k < creatureRenderers.Count; k++)
            {
                var c = controller.Creatures[k];
                float x;
                int g;
                if (int.TryParse(GenerationsField.text, out g))
                {
                    x = (float)c.GetGeneration() / g;
                }
                else
                {
                    x = 0;
                }
                creatureRenderers[k].Update(c.GetBodies(), c.GetJoints(), new Color(x, x, x));
            }
        }

    }
}
