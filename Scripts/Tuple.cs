public struct Tuple {
	public int a;
	public int b;
	public Tuple (int a, int b) {
		this.a = a;
		this.b = b;
	}

	public Tuple Reverse () {
		return new Tuple (b, a);
	}

	public override bool Equals (object obj)
	{
		if (obj.GetType () == typeof(Tuple))
			return ((Tuple)obj) == this;
		else
			return false;
	}

	public override int GetHashCode () {
		return a.GetHashCode () + b.GetHashCode ();
	}

	public static bool operator ==(Tuple x, Tuple y) {
		return x.a == y.a && x.b == y.b;
	}

	public static bool operator !=(Tuple x, Tuple y) {
		return x.a != y.a || x.b != y.b;
	}
}
