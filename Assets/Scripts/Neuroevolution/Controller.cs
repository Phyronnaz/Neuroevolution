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

        public Controller(Creature creature)
        {
            Creatures = new List<Creature>();
            Creatures.Add(creature);
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

            var scores = new List<List<float>>();
            var genomes = new List<List<int>>();
            var parents = new List<List<int>>();

            //Start training
            for (var k = 0; k < generations; k++)
            {
                //Update creatures
                controller.Update((int)(testDuration / DeltaTime));

                //Save the scores, genomes and parents
                var s = new List<float>();
                var g = new List<int>();
                var p = new List<int>();
                foreach (var c in controller.Creatures)
                {
                    s.Add(c.GetAveragePosition());
                    g.Add(c.Genome);
                    p.Add(c.Parent);
                }
                scores.Add(s);
                genomes.Add(g);
                parents.Add(p);

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
                var s = "Variation; Generation; Genome; Parent; Score";
                file.WriteLine(s);
                for (int k = 0; k < scores.Count; k++)
                {
                    for (var i = 0; i < scores[k].Count; i++)
                    {
                        var l = variation.ToString() + ";" + k.ToString() + ";";
                        l += genomes[k][i].ToString() + "; " + parents[k][i].ToString() + "; " + scores[k][i].ToString();
                        file.WriteLine(l);
                    }
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

        public Creature GetBestCreature()
        {
            Creature max = Creatures[0];
            foreach (var c in Creatures)
            {
                if (c.GetAveragePosition() > max.GetAveragePosition())
                    max = c;
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
                Creatures[k] = Creature.DuplicateCreature(Creatures[k]);
            }
        }
    }
}