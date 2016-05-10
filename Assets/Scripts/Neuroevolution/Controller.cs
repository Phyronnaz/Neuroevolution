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

        public void Update(int testDuration)
        {
            var waitHandles = new AutoResetEvent[Creatures.Count];
            for (var k = 0; k < Creatures.Count; k++)
            {
                int x = k; //Because of shared data
                waitHandles[x] = new AutoResetEvent(false);
                ThreadPool.QueueUserWorkItem(state => ThreadedJob(Creatures[x], testDuration, waitHandles[x]));
            }

            WaitHandle.WaitAll(waitHandles);

            // Update time
            CurrentTime += DeltaTime * testDuration;
        }

        public void GenerateNextGeneration(float variation)
        {
            Creatures.Sort();
            ResetCreatures();
            if (Creatures.Count % 2 != 0)
            {
                Creatures.Add(Creature.CloneCreature(Creatures[0], variation));
            }
            for (var k = 0; k < Creatures.Count / 2; k++)
            {
                Creatures[k] = Creature.CloneCreature(Creatures[k + Creatures.Count / 2], variation);
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