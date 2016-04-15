using System;
using UnityEngine;

namespace Evolution
{
    public class DumbMuscle : Muscle
    {
        public DumbMuscle(Node left, Node right, Color color, Transform parent)
        {
            Left = left;
            Right = right;

            //Create muscle renderer
            muscleRenderer = (new GameObject()).AddComponent<MuscleRenderer>();
            muscleRenderer.gameObject.name = "Dumb Muscle from " + left.Id + " to " + right.Id;
            muscleRenderer.Color = color;
            muscleRenderer.transform.parent = parent;
            muscleRenderer.MaxLength = Vector2.Distance(left.Position, right.Position);
            muscleRenderer.Initialize();
            UpdateGraphics();
        }


        public override Muscle Clone(Node left, Node right, float variationAmplitude, Color color, Transform parent)
        {
            return new DumbMuscle(left, right, color, parent);
        }

        public override void Update()
        {
            var center = (Left.Position + Right.Position) / 2;

            Left.AddConstraint(-(center - Left.Position).normalized);
            Right.AddConstraint(-(center - Right.Position).normalized);

            Left.AddConstraint((center - Left.Position).normalized);
            Right.AddConstraint((center - Right.Position).normalized);
        }
    }
}