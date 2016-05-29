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

        private static void NextGenerationSpecies(List<Creature> creatures, float currentVariation)
        {
            //Sort by species
            creatures.Sort((a, b) =>
            {
                if (a.Species == b.Species)
                {
                    return b.GetFitness().CompareTo((a.GetFitness()));
                }
                else
                {
                    return b.Species.CompareTo(a.Species);
                }
            });

            var groups = new List<List<Creature>>();

            //Create groups
            var currentSpecies = creatures[0].Species;
            groups.Add(new List<Creature>());
            foreach (var c in creatures)
            {
                if (c.Species == currentSpecies)
                {
                    groups[groups.Count - 1].Add(c);
                }
                else
                {
                    currentSpecies = c.Species;
                    groups.Add(new List<Creature>());
                    groups[groups.Count - 1].Add(c);
                }
            }

            //Sort species
            groups.Sort((y, x) => x[0].GetFitness().CompareTo(y[0].GetFitness()));

            //Generate new creatures
            var newCreatures = new List<Creature>();

            //Kept groups
            for (var i = 0; i < (creatures.Count - Globals.RandomCount) / 5; i++)
            {
                var g = groups[i];

                //Best of the species
                newCreatures.Add(g[0].GetCopy());
                newCreatures.Add(g[0].GetChild(currentVariation));

                //Second best
                if (groups[i].Count > 1)
                {
                    newCreatures.Add(g[1].GetCopy());
                    newCreatures.Add(g[1].GetChild(currentVariation));
                }
                else
                {
                    newCreatures.Add(g[0].GetRandomClone());
                    newCreatures.Add(g[0].GetRandomClone());
                }

                //Random one
                newCreatures.Add(g[0].GetRandomClone());
            }

            while (newCreatures.Count < creatures.Count)
            {
                newCreatures.Add(newCreatures[0].GetRandomClone());
            }

            creatures.Clear();
            creatures.AddRange(newCreatures);
        }

        private static void NextGenerationNormal(List<Creature> creatures, float currentVariation)
        {
            creatures.Sort((a, b) => a.GetFitness().CompareTo(b.GetFitness()));

            if (creatures.Count % 2 != 0)
            {
                creatures.Add(creatures[0].GetChild(currentVariation));
            }
            for (var i = 0; i < creatures.Count / 2; i++)
            {
                creatures[i] = creatures[i + creatures.Count / 2].GetChild(currentVariation);
            }
            for (var i = creatures.Count / 2; i < creatures.Count; i++)
            {
                creatures[i] = creatures[i].GetCopy();
            }
        }

        private static void TrainThread(Controller controller, int generations, int testDuration, float variation, string filename)
        {
            controller.IsTraining = true;
            controller.ResetCreatures();

            var save = new CSVSave(generations);

            //Start training
            for (var k = 0; k < generations; k++)
            {
                //Variation
                var currentVariation = (variation == -1) ? GetVariation(k + 1, generations) : variation;

                //Update creatures
                controller.Update((int)(testDuration / Globals.DeltaTime));

                //Save the scores, genomes ...
                save.Add(currentVariation, k, controller.Creatures);


                //Generate next generation
                if (k != generations - 1)
                {
                    if (Globals.UseSpecies)
                    {
                        NextGenerationSpecies(controller.Creatures, currentVariation);
                    }
                    else
                    {
                        NextGenerationNormal(controller.Creatures, currentVariation);
                    }
                    controller.CurrentGeneration = k + 1;
                }
                else
                {
                    for (var i = 0; i < controller.Creatures.Count; i++)
                    {
                        controller.Creatures[i] = controller.Creatures[i].GetCopy();
                    }
                }
            }

            //Score output in csv
            save.SaveToFile(filename, DataPath);

            //Reset variables
            controller.CurrentTime = 0;
            controller.IsTraining = false;
        }


        public void Train(int generations, int testDuration, float variation, string filename)
        {
            var t = new Thread(() => TrainThread(this, generations, testDuration, variation, filename));
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
                if (c.GetAveragePosition().X > max.GetAveragePosition().X)
                    max = c;
            }
            return max;
        }

        public void RemoveCreaturesFartherThan(float distance)
        {
            var max = GetBestCreature().GetAveragePosition().X;
            Creatures.RemoveAll(c => c.GetAveragePosition().X < max - distance);
        }

        public void ResetCreatures()
        {
            for (var k = 0; k < Creatures.Count; k++)
            {
                Creatures[k] = Creatures[k].GetCopy();
            }
        }
    }
}