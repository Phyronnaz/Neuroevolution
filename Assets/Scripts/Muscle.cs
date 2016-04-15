using UnityEngine;

namespace Evolution
{
    public abstract class Muscle
    {
        public Node Left;
        public Node Right;
        public MuscleRenderer muscleRenderer;


        public abstract Muscle Clone(Node left, Node right, float variationAmplitude, Color color, Transform parent);
        public abstract void Update();
        public virtual void UpdateGraphics()
        {
            muscleRenderer.SetPosition(Left.Position, Right.Position);
        }
        public void Destroy()
        {
            Object.Destroy(muscleRenderer.gameObject);
        }
        public override bool Equals(object obj)
        {
            if (obj is Tuple)
            {
                var t = (Tuple)obj;
                return (Left.Id == t.a && Right.Id == t.b) || (Left.Id == t.b && Right.Id == t.a);
            }
            else
            {
                var m = (IntelligentMuscle)obj;
                if (m != null)
                {
                    return m.Left == Left && m.Right == Right;
                }
                else
                {
                    return false;
                }
            }
        }
        public override int GetHashCode()
        {
            return Left.GetHashCode() + Right.GetHashCode();
        }
    }
}
