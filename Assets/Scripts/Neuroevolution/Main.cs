using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Neuroevolution
{
    public class Main : MonoBehaviour
    {
        //UI
        public Slider TimeMultiplierSlider;
        public InputField GenerationsField;
        public InputField TestDurationField;
        public InputField InitialPopulationSizeField;
        public InputField InitialPopulationVariationField;
        public InputField VariationField;
        public InputField HiddenLayersCountField;
        public Text DistanceText;
        public Text TimeText;
        public Text SpeedText;
        List<CreatureRenderer> creatureRenderers;
        //Others
        Controller controller;
        Editor editor;
        bool edit;
        const int hiddenSize = 4; //Auto increased if needed

        public void Awake()
        {
            InvokeRepeating("ControllerUpdate", 0, Controller.DeltaTime);
        }
        public void Start()
        {
            edit = true;
            editor = new Editor();
            creatureRenderers = new List<CreatureRenderer>();
            var tmp = transform.position;
            tmp.x = 0;
            transform.position = tmp;
        }

        public void InitializeController()
        {
            var h = hiddenSize;
            h = Mathf.Max(h, editor.GetRevoluteJoints().Count * 2 + 1);
            var s = new List<Matrix>();
            s.Add(Matrix.Random(editor.GetRevoluteJoints().Count * 2 + 1, h));
            for (var k = 1; k < int.Parse(HiddenLayersCountField.text); k++)
            {
                s.Add(Matrix.Random(h, h));
            }
            s.Add(Matrix.Random(h, editor.GetRevoluteJoints().Count));

            var c = new Creature(editor.GetPositions(), editor.GetDistanceJoints(), editor.GetRevoluteJoints(), s, 0);
            edit = false;
            editor.Destroy();
            var l = new List<Creature>();
            l.Add(c);
            //l.Add(Creature.CloneCreature(c, 0));
            controller = new Controller(l);
            RenderCreatures();
        }

        /// <summary>
        /// Called by UI
        /// </summary>
        public void Train()
        {
            if (controller == null)
            {
                InitializeController();
            }

            //Adjust creatures number
            var initialVariation = float.Parse(InitialPopulationVariationField.text);
            while (controller.Creatures.Count < int.Parse(InitialPopulationSizeField.text))
            {
                var randomCreature = controller.Creatures[Random.Range(0, controller.Creatures.Count)];
                controller.Creatures.Add(Creature.CloneCreature(randomCreature, initialVariation));
            }
            while (controller.Creatures.Count > int.Parse(InitialPopulationSizeField.text))
            {
                controller.Creatures.RemoveAt(Random.Range(0, controller.Creatures.Count));
            }

            //Begin train
            controller.Train(int.Parse(GenerationsField.text), int.Parse(TestDurationField.text), float.Parse(VariationField.text));
        }

        public void EditUpdate()
        {
            editor.Update();
            if (Input.GetKeyDown(KeyCode.Space))
            {
                InitializeController();
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                editor.Destroy();
                editor = new Editor();
            }
        }

        public void PlayUpdate()
        {
            var max = controller.GetMaxPosition();

            // Update camera position
            var tmp = transform.position;
            tmp.x = Mathf.Lerp(tmp.x, max + 5, Time.deltaTime * Mathf.Exp(TimeMultiplierSlider.value) + 0.01f);
            transform.position = tmp;

            //Remove slowest creatures
            controller.RemoveCreaturesFartherThan(100);

            //Update UI
            var m = max.ToString();
            if (m.Length > 5)
                m = m.Substring(0, 5);
            var t = controller.CurrentTime.ToString();
            if (t.Length > 7)
                t = t.Substring(0, 7);
            var s = (max / controller.CurrentTime).ToString();
            if (s.Length > 6)
                s = s.Substring(0, 6);
            DistanceText.text = "Distance : " + m;
            TimeText.text = "Time : " + t;
            SpeedText.text = "Speed:" + s;

            //Render creatures
            RenderCreatures();

            //Input
            if (Input.GetKeyDown(KeyCode.A))
            {
                controller.ResetCreatures();
                controller.CurrentTime = 0;
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                foreach (var c in creatureRenderers)
                {
                    c.Destroy();
                }
                Start();
            }
        }

        void RenderCreatures()
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
                    x = c.Generation / g;
                }
                else
                {
                    x = 0;
                }
                creatureRenderers[k].Update(c.GetBodies(), c.GetJoints(), new Color(x, x, x));
            }
        }

        public void Update()
        {
            if (edit)
            {
                EditUpdate();
            }
            else
            {
                PlayUpdate();
            }
        }

        void ControllerUpdate()
        {
            if (controller != null)
            {
                controller.Update((int)Mathf.Exp(TimeMultiplierSlider.value));
            }
        }
    }
}
