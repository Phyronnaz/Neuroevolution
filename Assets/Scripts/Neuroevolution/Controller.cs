﻿using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Assets.Scripts.Neuroevolution
{
    public class Controller
    {
        public List<Creature> Creatures;
        public float CurrentTime;
        public const float DeltaTime = 0.001f;
        public const int NumberOfThreads = 32;


        public Controller(List<Creature> creatures)
        {
            Creatures = creatures;
        }

        public Controller(Creature creature)
        {
            Creatures = new List<Creature>();
            Creatures.Add(creature);
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
            Debug.Log(Creatures[0].GetBodies()[0].Position.Y);
            // Update creatures
            var numberOfThreads = Mathf.Min(NumberOfThreads, Creatures.Count);
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

            // Update time
            CurrentTime += DeltaTime * testDuration;
        }

        public void GenerateNextGeneration(float variation)
        {
            Creatures.Sort();
            ResetCreatures();
            var count = Creatures.Count;
            if (count % 2 != 0)
            {
                Creatures.Add(Creature.CloneCreature(Creatures[count - 1], variation));
                count -= 1;
            }
            for (var k = 0; k < count / 2; k++)
            {
                Creatures[k] = Creature.CloneCreature(Creatures[k + count / 2], variation);
            }

        }

        public void Train(int generations, int testDuration, float variation)
        {
            for (var k = 0; k < generations; k++)
            {
                Update(testDuration);
                GenerateNextGeneration(variation);
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


        public void RemoveCreaturesFartherThan(float distance)
        {
            var max = GetMaxPosition();
            Creatures.RemoveAll(c => c.GetAveragePosition() < max - distance);
        }

        public void ResetCreatures()
        {
            foreach (var c in Creatures)
            {
                c.Reset();
            }
        }
    }
}