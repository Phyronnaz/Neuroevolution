import matplotlib.pyplot as plt
from io import StringIO
import numpy as np
import csv

import tkinter as tk
from tkinter import filedialog

root = tk.Tk()
root.withdraw()
file_path = filedialog.askopenfilename()

scsv = open(file_path, "r").read()

f = StringIO(scsv)
reader = csv.reader(f, delimiter=';')

genomes = [[]]
for row in reader:
    if row[0] != "Generation":
        generation = int(row[0])
        genome = int(row[2])
        parent = int(row[4])
        score = float(row[5])
        fitness = float(row[6])
        power = float(row[7])
        species = int(row[3])

        while len(genomes) <= genome:
            genomes.append([])

        genomes[genome].append([parent, generation, score, fitness, power, species])

colors = [np.random.rand(3, 1) for k in genomes]


for j in range(2, 5):
    plt.figure()
    for i in range(len(genomes)):
        for g in genomes[i]:
            if g[0] != -1:
                for p in genomes[g[0]]:
                    if p[1] == g[1] - 1:
                        plt.plot([g[1], p[1]], [g[j], p[j]], color=colors[g[5]], marker='o')
                        break
                else:
                    plt.plot([g[1], g[1] - 1], [g[j], g[j]], color=colors[g[5]], marker='o')

            else:
                if g[1] != 0:
                    plt.plot([g[1], g[1] - 1], [g[j], g[j]], color=colors[g[5]], marker='o')
                else:
                    plt.plot(g[1], g[j], color=colors[g[5]], marker='o')
        for k in range(len(genomes[i])):
            g = genomes[i][k]
            if k == 0 or genomes[i][k][j] != genomes[i][k - 1][j]:
                if g[0] != -1:
                    for p in genomes[g[0]]:
                        if p[1] == g[1] - 1:
                            plt.plot([g[1], p[1]], [g[j], p[j]], color=colors[g[5]], marker='o')
                            break
                else:
                    plt.plot(g[1], g[j], color=colors[g[5]], marker='o')

            else:
                plt.plot([g[1], g[1] - 1], [g[j], g[j]], color=colors[g[5]], marker='o')

    plt.xlabel('Generation')
    if j == 2:
        plt.ylabel('Score')
    if j == 3:
        plt.ylabel('Fitness')
    if j == 4:
        plt.ylabel('Power')
plt.show()
