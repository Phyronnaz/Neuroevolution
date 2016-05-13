using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Assets.Scripts.Neuroevolution
{
    public class Controller
    {
        public List<Creature> Creatures;
        public float CurrentTime;
        public const float DeltaTime = 0.01f;
        public bool IsTraining;
        public int CurrentGeneration;
        public int TotalGenerations;
        public float TrainStartTime;
        public string DataPath = Application.dataPath;


        public Controller(List<Creature> creatures)
        {
            Creatures = creatures;
        }




        static void ThreadedJob(Creature c, int testDuration, AutoResetEvent waitHandle)
        {
            for (var k = 0; k < testDuration; k++)
            {
                c.Update(DeltaTime);
            }
            waitHandle.Set();
        }

        static void TrainThread(Controller controller, int generations, int testDuration, float variation)
        {
            controller.IsTraining = true;
            controller.ResetCreatures();
            controller.TotalGenerations = generations;

            var score = new List<List<float>>();

            //Start training
            for (var k = 0; k < generations; k++)
            {
                //Update creatures
                controller.Update((int)(testDuration / DeltaTime));

                //Save the scores
                var l = new List<float>();
                foreach (var c in controller.Creatures)
                {
                    l.Add(c.GetAveragePosition());
                }
                score.Add(l);

                //Generate next generation
                controller.Creatures.Sort();
                controller.ResetCreatures();
                if (controller.Creatures.Count % 2 != 0)
                {
                    controller.Creatures.Add(Creature.CloneCreature(controller.Creatures[0], variation));
                }
                for (var i = 0; i < controller.Creatures.Count / 2; i++)
                {
                    controller.Creatures[i] = Creature.CloneCreature(controller.Creatures[i + controller.Creatures.Count / 2], variation);
                }
                controller.CurrentGeneration = k + 1;
            }

            //Score output in csv
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(controller.DataPath + @"\score.csv", true))
            {
                var s = "Variation; Generation; ";
                for (int k = 0; k < controller.Creatures.Count; k++)
                {
                    s += "Creature " + k + ";";
                }
                file.WriteLine(s);
                for (int k = 0; k < score.Count; k++)
                {
                    var l = variation.ToString() + ";" + k.ToString() + ";";
                    foreach (var f in score[k])
                    {
                        l += f.ToString() + ";";
                    }
                    file.WriteLine(l);
                }
            }

            //Reset variables
            controller.CurrentTime = 0;
            controller.IsTraining = false;
        }


        public void Train(int generations, int testDuration, float variation)
        {
            TrainStartTime = Time.time;
            var t = new Thread(() => TrainThread(this, generations, testDuration, variation));
            t.Start();
        }

        public void Update(int testDuration)
        {
            var waitHandles = new AutoResetEvent[Creatures.Count];
            for (var k = 0; k < Creatures.Count; k++)
            {
                int x = k; //Because of shared data
                waitHandles[x] = new AutoResetEvent(false);
                ThreadPool.QueueUserWorkItem(state => ThreadedJob(Creatures[x], testDuration, waitHandles[x]));
            }

            foreach (var w in waitHandles)
            {
                w.WaitOne();
                if (!w.Set())
                {
                    Debug.LogError("Thread take too long");
                }
            }

            // Update time
            CurrentTime += DeltaTime * testDuration;
        }



        public float GetMaxPosition()
        {
            float max = Creatures[0].GetAveragePosition();
            foreach (var c in Creatures)
            {
                if (c.GetAveragePosition() > max)
                    max = c.GetAveragePosition();
            }
            return max;
        }

        public void RemoveCreaturesFartherThan(float distance)
        {
            var max = GetMaxPosition();
            Creatures.RemoveAll(c => c.GetAveragePosition() < max - distance);
        }

        public void ResetCreatures()
        {
            for (var k = 0; k < Creatures.Count; k++)
            {
                Creatures[k] = Creature.CloneCreature(Creatures[k], 0);
            }
        }
    }
}