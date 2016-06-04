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
        public float Progression;
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


        private static void NextGenerationSpecies(List<Creature> creatures, float variation)
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
            for (var s = 0; s < Globals.SpeciesSizes.Count; s++)
            {
                if (s < groups.Count && !groups[s][0].IsDead)
                {
                    for (var i = 0; i < Globals.SpeciesSizes[s]; i++)
                    {
                        if (i < groups[s].Count)
                        {
                            newCreatures.Add(groups[s][i].GetCopy());
                            newCreatures.Add(groups[s][i].GetChild(variation));
                        }
                        else
                        {
                            newCreatures.Add(groups[s][0].GetChild(variation));
                            newCreatures.Add(groups[s][0].GetChild(variation));
                        }
                    }
                }
                else
                {
                    var c = groups[0][0].GetRandomClone();
                    newCreatures.Add(c);
                    newCreatures.Add(c.GetChild(variation));
                    for (var i = 0; i < Globals.SpeciesSizes[s] - 1; i++)
                    {
                        newCreatures.Add(c.GetChild(variation));
                        newCreatures.Add(c.GetChild(variation));
                    }
                }
            }
            for (var i = 0; i < Globals.RandomCount; i++)
            {
                newCreatures.Add(groups[0][0].GetRandomClone());
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
                //Update progression
                controller.Progression = (float)k / generations;

                //Update creatures
                controller.Update((int)(testDuration / Globals.DeltaTime));

                //Save the scores, genomes ...
                save.Add(variation, k, controller.Creatures);


                //Generate next generation
                if (k != generations - 1)
                {
                    if (Globals.UseSpecies)
                    {
                        NextGenerationSpecies(controller.Creatures, variation);
                    }
                    else
                    {
                        NextGenerationNormal(controller.Creatures, k);
                    }
                }
                else
                {
                    for (var i = 0; i < controller.Creatures.Count; i++)
                    {
                        controller.Creatures[i] = controller.Creatures[i].GetCopy();
                    }
                }
                Counters.AddGeneration();
            }

            //Score output in csv
            save.SaveToFile(filename, DataPath);

            //Reset variables
            controller.CurrentTime = 0;
            controller.Progression = 0;
            controller.IsTraining = false;
        }


        public void Train(int generations, int testDuration, float variation, string filename)
        {
            if (Globals.UseThreads)
            {
                var t = new Thread(() => TrainThread(this, generations, testDuration, variation, filename));
                t.Start();
            }
            else
            {
                TrainThread(this, generations, testDuration, variation, filename);
            }
        }

        public void Update(int testDuration)
        {
            // Update time
            CurrentTime += Globals.DeltaTime * testDuration;

            //Update creatures
            if (Globals.UseThreads)
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
            }
            else
            {
                foreach (var c in Creatures)
                {
                    ThreadedJob(c, testDuration, new AutoResetEvent(false));
                }
            }
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