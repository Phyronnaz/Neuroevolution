using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System.IO;
using System.Windows.Forms;
using System;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Neuroevolution
{
    public class Main : MonoBehaviour
    {
        private List<CreatureRenderer> creatureRenderers;
        private Controller controller;
        private Editor editor;
        private MainUI mainUI;
        private bool edit;
        private bool pause;


        public void Awake()
        {
            CancelInvoke();
            InvokeRepeating("ControllerUpdate", 0, Globals.DeltaTime);
            mainUI = GetComponent<MainUI>();
        }

        public void Start()
        {
            controller = null;
            edit = true;
            editor = new Editor();
            creatureRenderers = new List<CreatureRenderer>();
            var tmp = transform.position;
            tmp.x = 0;
            tmp.y = 12.5f;
            transform.position = tmp;
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
                CheckEditInputs();
            }
        }

        public void PlayUpdate()
        {
            if (controller.IsTraining)
            {
                mainUI.TrainUpdate(controller.CurrentGeneration);
            }
            else
            {
                //Update UI
                mainUI.NormalUpdate(controller.GetBestCreature(), controller.CurrentTime);

                //Remove slowest creatures
                controller.RemoveCreaturesFartherThan(100);

                //Render creatures
                RenderCreatures();

                //Input
                CheckPlayInputs();
            }
        }


        public void Train(int populationSize, int generations, int testDuration, float variation, string filename)
        {
            if (controller == null)
            {
                InitializeController();
            }

            //Adjust creatures number
            while (controller.Creatures.Count < populationSize)
            {
                controller.Creatures.Add(controller.Creatures[0].GetRandomClone());
            }
            while (controller.Creatures.Count > populationSize)
            {
                controller.Creatures.RemoveAt(CustomRandom.Range(0, controller.Creatures.Count));
            }

            //Begin train
            controller.Train(generations, testDuration, variation, filename);
        }

        private void LoadCreature()
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
                            XmlSerializer serializer = new XmlSerializer(typeof(CreatureStruct));
                            editor.Creature = (CreatureStruct)serializer.Deserialize(myStream);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        private void InitializeController()
        {
            Creature c;
            if (editor.Creature.Synapses.Count == 0)
            {
                c = CreatureFactory.CreateCreature(editor.Creature, Globals.HiddenSize, Globals.HiddenLayersCount);
            }
            else
            {
                c = CreatureFactory.CreateCreature(editor.Creature);
            }
            edit = false;
            editor.Destroy();
            controller = new Controller(c);
        }

        private void CheckEditInputs()
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
                LoadCreature();
            }
        }

        private void CheckPlayInputs()
        {
            if (GameObject.Find("EventSystem").GetComponent<EventSystem>().currentSelectedGameObject == null)
            {
                if (Input.GetKeyDown(KeyCode.A))
                {
                    controller.ResetCreatures();
                    controller.CurrentTime = 0;
                }
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    pause = !pause;
                }
                if (Input.GetKeyDown(KeyCode.R))
                {
                    Restart();
                }
                if(Input.GetKeyDown(KeyCode.E))
                {
                    var c = controller.GetBestCreature().Save;
                    Restart();
                    editor.Creature = c;
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
                            var save = controller.GetBestCreature().Save;
                            XmlSerializer serializer = new XmlSerializer(typeof(CreatureStruct));
                            serializer.Serialize(myStream, save);
                            myStream.Close();
                        }
                    }
                }
            }
        }

        private void Restart()
        {
            foreach (var c in creatureRenderers)
            {
                c.Destroy();
            }
            Start();
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
                creatureRenderers[k].Update(c.Save, c.GetBodies());
            }
        }

        private void ControllerUpdate()
        {
            if (controller != null && !controller.IsTraining && !pause)
            {
                controller.Update(mainUI.GetTimeMultiplier());
                //Pause at 20s
                if (controller.CurrentTime <= 20 && controller.CurrentTime + mainUI.GetTimeMultiplier() * Globals.DeltaTime >= 20)
                {
                    pause = true;
                }
            }
        }

    }
}
