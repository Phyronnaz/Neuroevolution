using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Neuroevolution
{
    public static class CreatureFactory
    {
        public static Creature CreateCreature(CreatureStruct creature)
        {
            return new Creature(creature, 0, Counters.GenomeCount, Counters.SpeciesCount, -1);
        }

        public static Creature CreateCreature(CreatureStruct creature, int hiddenSize, int hiddenLayersCount)
        {
            var revoluteCount = creature.RevoluteJoints.Count;
            hiddenSize = Mathf.Max(hiddenSize, revoluteCount * 2 + 1);
            var synapses = new List<Matrix>();
            synapses.Add(Matrix.Random(revoluteCount * 2 + 1, hiddenSize));
            for (var k = 1; k < hiddenLayersCount; k++)
            {
                synapses.Add(Matrix.Random(hiddenSize, hiddenSize));
            }
            synapses.Add(Matrix.Random(hiddenSize, revoluteCount));
            creature.Synapses = synapses;
            return new Creature(creature, 0, Counters.GenomeCount, Counters.SpeciesCount, -1);
        }
    }
}
