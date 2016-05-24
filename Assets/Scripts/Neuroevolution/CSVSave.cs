using System.Collections.Generic;

namespace Assets.Scripts.Neuroevolution
{
    public struct CSVSave
    {
        public List<float> Variations;
        public List<int> Generations;
        public List<List<int>> Genomes;
        public List<List<int>> Species;
        public List<List<int>> Parents;
        public List<List<float>> Scores;
        public List<List<float>> Fitnesses;
        public List<List<float>> Powers;
        public List<List<float>> Angles;

        public CSVSave(int generations)
        {
            Variations = new List<float>(generations);
            Generations = new List<int>(generations);
            Genomes = new List<List<int>>(generations);
            Species = new List<List<int>>(generations);
            Parents = new List<List<int>>(generations);
            Scores = new List<List<float>>(generations);
            Fitnesses = new List<List<float>>(generations);
            Powers = new List<List<float>>(generations);
            Angles = new List<List<float>>(generations);
        }


        public void Add(float variation, int generation, List<Creature> creatures)
        {
            var genomes = new List<int>(); ;
            var species = new List<int>();
            var parents = new List<int>();
            var scores = new List<float>();
            var fitnesses = new List<float>();
            var powers = new List<float>();
            var angles = new List<float>();
            foreach (var c in creatures)
            {
                genomes.Add(c.GetGenome());
                species.Add(c.GetSpecies());
                parents.Add(c.GetParent());
                scores.Add(c.GetAveragePosition().X);
                fitnesses.Add(c.GetFitness());
                powers.Add(c.GetPower());
                angles.Add(c.GetAngle());
            }
            Variations.Add(variation);
            Generations.Add(generation);
            Genomes.Add(genomes);
            Species.Add(species);
            Parents.Add(parents);
            Scores.Add(scores);
            Fitnesses.Add(fitnesses);
            Powers.Add(powers);
            Angles.Add(angles);
        }

        public void SaveToFile(string fileName, string path)
        {
            if (fileName != "")
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(path + @"\" + fileName + ".csv", true))
                {
                    var s = "Generation; Variation; Genome; Species; Parent; Score; Fitness; Power; Angle";
                    file.WriteLine(s);
                    for (int i = 0; i < Scores.Count; i++)
                    {
                        for (var j = 0; j < Scores[i].Count; j++)
                        {
                            s = Generations[i].ToString();
                            s += "; ";
                            s += Variations[i];
                            s += "; ";
                            s += Genomes[i][j];
                            s += "; ";
                            s += Species[i][j];
                            s += "; ";
                            s += Parents[i][j];
                            s += "; ";
                            s += Scores[i][j];
                            s += "; ";
                            s += Fitnesses[i][j];
                            s += "; ";
                            s += Powers[i][j];
                            s += "; ";
                            s += Angles[i][j];
                            file.WriteLine(s);
                        }
                    }
                }
            }
        }
    }
}
