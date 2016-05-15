import matplotlib.pyplot as plt
from io import StringIO
import numpy as np
import csv

scsv = open("score_adaptive_0.001_100.csv", "r").read()

f = StringIO(scsv)
reader = csv.reader(f, delimiter=';')

genomes = [[]]
for row in reader:
    if row[0] != "Variation":
        generation = int(row[1])
        genome = int(row[2])
        parent = int(row[3])
        score = float(row[4])

        while len(genomes) <= genome:
            genomes.append([])

        genomes[genome].append([parent, generation, score])

colors = [np.random.rand(3, 1) for k in genomes]

for i in range(len(genomes)):
    for g in genomes[i]:
        if g[0] != -1:
            for p in genomes[g[0]]:
                if p[1] == g[1] - 1:
                    plt.plot([g[1], p[1]], [g[2], p[2]], color=colors[i], marker='o')
                    break
            else:
                plt.plot([g[1], g[1] - 1], [g[2], g[2]], color=colors[i], marker='o')

        else:
            if g[1] != 0:
                plt.plot([g[1], g[1] - 1], [g[2], g[2]], color=colors[i], marker='o')
            else:
                plt.plot(g[1], g[2], color=colors[i], marker='o')
    for k in range(len(genomes[i])):
        g = genomes[i][k]
        if k == 0 or genomes[i][k][2] != genomes[i][k - 1][2]:
            if g[0] != -1:
                for p in genomes[g[0]]:
                    if p[1] == g[1] - 1:
                        plt.plot([g[1], p[1]], [g[2], p[2]], color=colors[i], marker='o')
                        break
            else:
                plt.plot(g[1], g[2], color=colors[i], marker='o')

        else:
            plt.plot([g[1], g[1] - 1], [g[2], g[2]], color=colors[i], marker='o')

plt.xlabel('Generation')
plt.ylabel('Score')
plt.show()
