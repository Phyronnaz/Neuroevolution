public struct Matrix
{
    public readonly float[][] Array;

    public float Mean
    {
        get
        {
            var m = 0f;
            foreach (var k in Array)
            {
                foreach (var l in k)
                {
                    m += l;
                }
            }
            return m / Array.Length;
        }
    }

    public Matrix T
    {
        get
        {
            var m = new Matrix(N, M);
            for (var x = 0; x < N; x++)
            {
                for (var y = 0; y < M; y++)
                {
                    m[x][y] = this[y][x];
                }
            }
            return m;
        }
    }

    public int M
    {
        get
        {
            return Array.Length;
        }
    }

    public int N
    {
        get
        {
            return Array[0].Length;
        }
    }


    public Matrix(float[][] array)
    {
        Array = array;
    }

    public Matrix(int x, int y)
    {
        Array = new float[x][];
        for (var i = 0; i < x; i++)
        {
            Array[i] = new float[y];
        }
    }

    public static Matrix Random(int xLength, int yLength)
    {
        return Random(xLength, yLength, new System.Random());
    }

    public static Matrix Random(int xLength, int yLength, System.Random r)
    {
        return Random(xLength, yLength, 1, r);
    }

    public static Matrix Random(int xLength, int yLength, float variation, System.Random r)
    {
        var m = new Matrix(xLength, yLength);
        for (var x = 0; x < m.M; x++)
        {
            for (var y = 0; y < m.N; y++)
            {
                m[x][y] = variation * ((float)r.NextDouble() * 2 - 1);
            }
        }
        return m;
    }

    public static Matrix Dot(Matrix l, Matrix r)
    {
        if (l.N != r.M)
            throw new System.FormatException();

        var f = new float[l.M][];
        for (var x = 0; x < f.Length; x++)
        {
            f[x] = new float[r.N];
        }

        for (int i = 0; i < l.M; i++)
        {
            float[] iRowA = l[i];
            float[] iRowC = f[i];
            for (int k = 0; k < r.M; k++)
            {
                float[] kRowB = r[k];
                float ikA = iRowA[k];
                for (int j = 0; j < r.N; j++)
                {
                    iRowC[j] += ikA * kRowB[j];
                }
            }
        }
        return new Matrix(f);
    }

    public static Matrix operator +(Matrix l, Matrix r)
    {
        if (l.M != r.M || l.N != r.N)
            throw new System.FormatException();

        var f = new float[l.M][];
        for (var x = 0; x < f.Length; x++)
        {
            f[x] = new float[l.N];
        }

        for (int x = 0; x < f.Length; x++)
        {
            for (int y = 0; y < f[0].Length; y++)
            {
                f[x][y] = l[x][y] + r[x][y];
            }
        }
        return new Matrix(f);
    }

    public static Matrix operator -(Matrix l, Matrix r)
    {
        if (l.M != r.M || l.N != r.N)
            throw new System.FormatException();

        var f = new float[l.M][];
        for (var x = 0; x < f.Length; x++)
        {
            f[x] = new float[l.N];
        }

        for (int x = 0; x < f.Length; x++)
        {
            for (int y = 0; y < f[0].Length; y++)
            {
                f[x][y] = l[x][y] - r[x][y];
            }
        }
        return new Matrix(f);
    }

    public static Matrix operator +(Matrix l, float r)
    {
        var f = new float[l.M][];
        for (var x = 0; x < f.Length; x++)
        {
            f[x] = new float[l.N];
        }

        for (int x = 0; x < f.Length; x++)
        {
            for (int y = 0; y < f[0].Length; y++)
            {
                f[x][y] = l[x][y] + r;
            }
        }
        return new Matrix(f);
    }

    public static Matrix operator -(Matrix l, float r)
    {
        var f = new float[l.M][];
        for (var x = 0; x < f.Length; x++)
        {
            f[x] = new float[l.N];
        }

        for (int x = 0; x < f.Length; x++)
        {
            for (int y = 0; y < f[0].Length; y++)
            {
                f[x][y] = l[x][y] - r;
            }
        }
        return new Matrix(f);
    }

    public static Matrix operator -(float l, Matrix r)
    {
        var f = new float[r.M][];
        for (var x = 0; x < f.Length; x++)
        {
            f[x] = new float[r.N];
        }

        for (int x = 0; x < f.Length; x++)
        {
            for (int y = 0; y < f[0].Length; y++)
            {
                f[x][y] = l - r[x][y];
            }
        }
        return new Matrix(f);
    }

    public static Matrix operator *(Matrix l, float r)
    {
        var f = new float[l.M][];
        for (var x = 0; x < f.Length; x++)
        {
            f[x] = new float[l.N];
        }

        for (int x = 0; x < f.Length; x++)
        {
            for (int y = 0; y < f[0].Length; y++)
            {
                f[x][y] = l[x][y] * r;
            }
        }
        return new Matrix(f);
    }

    public static Matrix operator *(float l, Matrix r)
    {
        return r * l;
    }

    public static Matrix operator /(Matrix l, float r)
    {
        var f = new float[l.M][];
        for (var x = 0; x < f.Length; x++)
        {
            f[x] = new float[l.N];
        }

        for (int x = 0; x < f.Length; x++)
        {
            for (int y = 0; y < f[0].Length; y++)
            {
                f[x][y] = l[x][y] / r;
            }
        }
        return new Matrix(f);
    }

    public static Matrix operator *(Matrix l, Matrix r)
    {
        if (l.M != r.M || l.N != r.N)
            throw new System.FormatException();

        var f = new float[l.M][];
        for (var x = 0; x < f.Length; x++)
        {
            f[x] = new float[l.N];
        }

        for (int x = 0; x < f.Length; x++)
        {
            for (int y = 0; y < f[0].Length; y++)
            {
                f[x][y] += l[x][y] * r[x][y];
            }
        }
        return new Matrix(f);
    }

    public static bool operator ==(Matrix l, Matrix r)
    {
        return l.Array == r.Array;
    }

    public static bool operator !=(Matrix l, Matrix r)
    {
        return l.Array != r.Array;
    }

    public float[] this[int x]
    {
        get
        {
            return Array[x];
        }
        set
        {
            Array[x] = value;
        }
    }

    public override bool Equals(object obj)
    {
        return Array == (float[][])obj;
    }

    public override int GetHashCode()
    {
        return Array.GetHashCode();
    }

    public override string ToString()
    {
        var s = "[";
        for (var x = 0; x < M; x++)
        {
            if (x != 0)
                s += ",";
            s += "[";
            for (var y = 0; y < N; y++)
            {
                if (y != 0)
                    s += ",";
                s += Array[x][y].ToString();
            }
            s += "]";
        }
        s += "]";
        return s;
    }
}