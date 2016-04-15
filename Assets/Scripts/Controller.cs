using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Evolution
{
    public class Controller
    {
        public List<Creature> Creatures;
        public float CurrentTime;
        public const float DeltaTime = 0.001f;


        public Controller(List<Creature> creatures)
        {
            Creatures = creatures;
        }


        static void ThreadedJob(List<Creature> creatures, int time)
        {
            foreach (var c in creatures)
            {
                for (var k = 0; k < time; k++)
                {
                    c.Update(DeltaTime);
                }
            }
        }

        public void Update(int testDuration)
        {
            // Update creatures
            var numberOfThreads = Mathf.Min(Constants.NumberOfThreads, Creatures.Count);
            var threads = new List<Thread>();
            var count = Creatures.Count - Creatures.Count % numberOfThreads;
            int h = count / numberOfThreads;
            for (var k = 0; k < numberOfThreads; k++)
            {
                var creaturesToProcess = Creatures.GetRange(k * h, h);
                threads.Add(new Thread(() => ThreadedJob(creaturesToProcess, testDuration)));
                threads[threads.Count - 1].Start();
            }
            if (count != Creatures.Count)
            {
                var endCreaturesToProcess = Creatures.GetRange(count, Creatures.Count - count);
                threads.Add(new Thread(() => ThreadedJob(endCreaturesToProcess, testDuration)));
                threads[threads.Count - 1].Start();
            }
            // Wait for threads to end
            foreach (var t in threads)
            {
                t.Join();
            }

            // Update graphics
            foreach (var c in Creatures)
            {
                c.UpdateGraphics();
            }

            // Update time
            CurrentTime += DeltaTime * testDuration;
        }

        public void GenerateNextGeneration(float generationProgress)
        {
            var genColor = new Color(generationProgress, generationProgress, generationProgress);
            Creatures.Sort();
            ResetCreatures();
            var count = Creatures.Count;
            if (count % 2 != 0)
            {
                Creatures.Add(Creatures[count - 1].Clone(Constants.Variation, genColor));
                count -= 1;
            }
            for (var k = 0; k < count / 2; k++)
            {
                Creatures[k].Destroy();
                Creatures[k] = Creatures[k + count / 2].Clone(Constants.Variation, genColor);
            }

        }

        public void Train(int generations, int testDuration)
        {
            for (var k = 0; k < generations; k++)
            {
                Update(testDuration);
                GenerateNextGeneration((float)k / generations);
            }
            CurrentTime = 0;
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


        public void RemoveCreaturesFartherThan(float distance, float max)
        {
            foreach (var c in Creatures)
            {
                if (c.GetAveragePosition() < max - distance)
                    c.Destroy();
            }
            Creatures.RemoveAll(c => c.GetAveragePosition() < max - distance);
        }

        public void ResetCreatures()
        {
            foreach (var c in Creatures)
            {
                c.Reset();
            }
        }

        public int GetCyclePercentageOfTheFarthestCreature()
        {
            var max = Creatures[0].GetAveragePosition();
            var bestCreature = Creatures[0];
            foreach (var c in Creatures)
            {
                if (c.GetAveragePosition() > max)
                {
                    max = c.GetAveragePosition();
                    bestCreature = c;
                }
            }
            return bestCreature.GetCyclePercentage();
        }

        public void Destroy()
        {
            foreach (var c in Creatures)
            {
                c.Destroy();
            }
            Creatures.Clear();
            GC.Collect();
        }
    }
}