using UnityEngine;
using System.Collections.Generic;
using System.Threading;

namespace Assets.Scripts.Neuroevolution
{
    public class Controller
    {
        public readonly List<Creature> Creatures;
        public float CurrentTime;
        public bool IsTraining;
        public int CurrentGeneration;
        public int TotalGenerations;
        public float TrainStartTime;
        public static string DataPath = Application.dataPath;


        public Controller(List<Creature> creatures)
        {
            Creatures = creatures;
        }

        public Controller(Creature creature)
        {
            Creatures = new List<Creature>();
            Creatures.Add(creature);
        }


        private static void ThreadedJob(Creature c, int testDuration, AutoResetEvent waitHandle)
        {
            for (var k = 0; k < testDuration; k++)
            {
                c.Update(Globals.DeltaTime);
            }
            waitHandle.Set();
        }

        private static float GetVariation(int currentGeneration, int totalGenerations)
        {
            return 0.1f / currentGeneration;
        }

        private static void TrainThread(Controller controller, int generations, int testDuration, float variation, string fileName)
        {
            controller.IsTraining = true;
            controller.ResetCreatures();
            controller.TotalGenerations = generations;

            var scores = new List<List<float>>();
            var genomes = new List<List<int>>();
            var parents = new List<List<int>>();
            var fitnesses = new List<List<float>>();
            var powers = new List<List<float>>();
            var angles = new List<List<float>>();

            //Start training
            for (var k = 0; k < generations; k++)
            {

                //Update creatures
                controller.Update((int)(testDuration / Globals.DeltaTime));

                //Save the scores, genomes and parents
                var s = new List<float>();
                var g = new List<int>();
                var p = new List<int>();
                var f = new List<float>();
                var pw = new List<float>();
                var a = new List<float>();
                foreach (var c in controller.Creatures)
                {
                    s.Add(c.GetAveragePosition());
                    g.Add(c.GetGenome());
                    p.Add(c.GetParent());
                    f.Add(c.GetFitness());
                    pw.Add(c.GetPower());
                    a.Add(c.GetAngle());
                }
                scores.Add(s);
                genomes.Add(g);
                parents.Add(p);
                fitnesses.Add(f);
                powers.Add(pw);
                angles.Add(a);


                var v = (variation == -1) ? GetVariation(k + 1, generations) : variation;

                if (k != generations - 1)
                {
                    //Generate next generation
                    controller.Creatures.Sort();

                    if (controller.Creatures.Count % 2 != 0)
                    {
                        controller.Creatures.Add(controller.Creatures[0].Clone(v));
                    }
                    for (var i = 0; i < controller.Creatures.Count / 2; i++)
                    {
                        controller.Creatures[i] = controller.Creatures[i + controller.Creatures.Count / 2].Clone(v);
                    }
                    for (var i = controller.Creatures.Count / 2; i < controller.Creatures.Count; i++)
                    {
                        controller.Creatures[i] = controller.Creatures[i].Duplicate();
                    }
                    controller.CurrentGeneration = k + 1;
                }
                else
                {
                    for (var i = 0; i < controller.Creatures.Count; i++)
                    {
                        controller.Creatures[i] = controller.Creatures[i].Duplicate();
                    }
                }
            }

            //Score output in csv
            if (fileName != "")
            {

                using (System.IO.StreamWriter file = new System.IO.StreamWriter(Controller.DataPath + @"\" + fileName + ".csv", true))
                {
                    var s = "Variation; Generation; Genome; Parent; Score; Fitness; Power; Angle";
                    file.WriteLine(s);
                    for (int k = 0; k < scores.Count; k++)
                    {
                        for (var i = 0; i < scores[k].Count; i++)
                        {
                            var l = variation.ToString();
                            l += "; ";
                            l += k.ToString();
                            l += "; ";
                            l += genomes[k][i].ToString();
                            l += "; ";
                            l += parents[k][i].ToString();
                            l += "; ";
                            l += scores[k][i].ToString();
                            l += "; ";
                            l += fitnesses[k][i].ToString();
                            l += "; ";
                            l += powers[k][i].ToString();
                            l += "; ";
                            l += angles[k][i].ToString();
                            file.WriteLine(l);
                        }
                    }
                }
            }

            //Reset variables
            controller.CurrentTime = 0;
            controller.IsTraining = false;
        }


        public void Train(int generations, int testDuration, float variation, string fileName)
        {
            TrainStartTime = Time.time;
            var t = new Thread(() => TrainThread(this, generations, testDuration, variation, fileName));
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
            }

            // Update time
            CurrentTime += Globals.DeltaTime * testDuration;
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
            var max = GetBestCreature().GetAveragePosition();
            Creatures.RemoveAll(c => c.GetAveragePosition() < max - distance);
        }

        public void ResetCreatures()
        {
            for (var k = 0; k < Creatures.Count; k++)
            {
                Creatures[k] = Creatures[k].Duplicate();
            }
        }
    }
}