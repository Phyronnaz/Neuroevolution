using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Assets.Scripts.Neuroevolution
{
    public struct CreatureUpdateStruct
    {
        public float testDuration;
        public Creature creature;

        public CreatureUpdateStruct(float testDuration, Creature creature)
        {
            this.testDuration = testDuration;
            this.creature = creature;
        }
    }
    public class Controller
    {
        public List<Creature> Creatures;
        public float CurrentTime;
        public const float DeltaTime = 0.01f;
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


        static void ThreadedJob(System.Object data)
        {
            var creature = ((CreatureUpdateStruct)data).creature;
            var testDuration = ((CreatureUpdateStruct)data).testDuration;
            for (var k = 0; k < testDuration; k++)
            {
                creature.Update(DeltaTime);
            }

        }

        public void Update(int testDuration)
        {
            // Update creatures
            //var numberOfThreads = Mathf.Min(NumberOfThreads, Creatures.Count);
            //var threads = new List<Thread>();
            //var count = Creatures.Count - Creatures.Count % numberOfThreads;
            //int h = count / numberOfThreads;
            //for (var k = 0; k < numberOfThreads; k++)
            //{
            //    var creaturesToProcess = Creatures.GetRange(k * h, h);
            //    threads.Add(new Thread(() => ThreadedJob(creaturesToProcess, testDuration)));
            //    threads[threads.Count - 1].Start();
            //    //ThreadedJob(creaturesToProcess, testDuration);
            //}
            //if (count != Creatures.Count)
            //{
            //    var endCreaturesToProcess = Creatures.GetRange(count, Creatures.Count - count);
            //    threads.Add(new Thread(() => ThreadedJob(endCreaturesToProcess, testDuration)));
            //    threads[threads.Count - 1].Start();
            //    //ThreadedJob(endCreaturesToProcess, testDuration);
            //}
            //// Wait for threads to end
            //foreach (var t in threads)
            //{
            //    t.Join();
            //}

            foreach (var creature in Creatures)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(ThreadedJob), new CreatureUpdateStruct(testDuration, creature));
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