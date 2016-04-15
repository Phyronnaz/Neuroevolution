using UnityEngine;

namespace Evolution
{
    public class Muscle
    {
        public readonly float Strength;
        public readonly float ExtendedLength;
        public readonly float ContractedLength;
        public readonly float ChangeTime;
        public readonly bool BeginWithContraction;
        public readonly Node Right;
        public readonly Node Left;
        public readonly MuscleRenderer muscleRenderer;

        bool contract;


        public Muscle(Node left, Node right, float strength, float extendedLength, float contractedLength, float changeTime, bool beginWithContraction, Color color, Transform parent)
        {
            Left = left;
            Right = right;
            Strength = strength;
            ExtendedLength = Mathf.Max(extendedLength, Constants.MinRandom);
            ContractedLength = Mathf.Max(contractedLength, Constants.MinRandom);
            ChangeTime = changeTime;
            BeginWithContraction = beginWithContraction;

            //Create muscle renderer
            muscleRenderer = (new GameObject()).AddComponent<MuscleRenderer>();
            muscleRenderer.gameObject.name = "Muscle from " + left.Id + " to " + right.Id;
            muscleRenderer.MaxLength = ExtendedLength;
            muscleRenderer.MinLength = ContractedLength;
            muscleRenderer.Color = color;
            muscleRenderer.transform.parent = parent;
            muscleRenderer.Initialize();
            UpdateGraphics();
        }


        public static Muscle RandomMuscle(Node left, Node right, float cycleDuration, bool dumbMuscle, Color color, Transform parent)
        {
            var distance = Vector2.Distance(left.Position, right.Position);
            float contractedLength, extendedLength;
            if (dumbMuscle)
            {
                contractedLength = distance;
                extendedLength = distance;
            }
            else
            {
                contractedLength = distance - Random.Range(Constants.MinRandom, Constants.ContractedDistanceMultiplier);
                extendedLength = distance + Random.Range(Constants.MinRandom, Constants.ExtendedDistanceMultiplier);
            }
            var strength = Random.Range(Constants.MinStrength, Constants.StrengthAmplitude);
            var muscleCycleDuration = Random.Range(Constants.MinRandom, cycleDuration);
            var beginWithContraction = (Random.value > 0.5f);

            return new Muscle(left, right, strength, extendedLength, contractedLength, muscleCycleDuration, beginWithContraction, color, parent);
        }

        public static Muscle CloneMuscle(Muscle muscle, Node left, Node right, float variationAmplitude, Color color, Transform parent)
        {
            var extendedLength = muscle.ExtendedLength * ((muscle.ExtendedLength == muscle.ContractedLength) ? 1 : (1 + Random.Range(-variationAmplitude, variationAmplitude)));
            var contractedLength = muscle.ContractedLength * ((muscle.ExtendedLength == muscle.ContractedLength) ? 1 : (1 + Random.Range(-variationAmplitude, variationAmplitude)));
            return new Muscle(
                left,
                right,
                muscle.Strength * (1 + Random.Range(-variationAmplitude, variationAmplitude)),
                extendedLength,
                contractedLength,
                muscle.ChangeTime * (1 + Random.Range(-variationAmplitude, variationAmplitude)),
                (Random.value > variationAmplitude) ? muscle.BeginWithContraction : !muscle.BeginWithContraction,
                color,
                parent);
        }

        public void Contract()
        {
            contract = true;
        }

        public void Extend()
        {
            contract = false;
        }

        public void Update()
        {
            var l = Vector2.Distance(Left.Position, Right.Position);
            var center = (Left.Position + Right.Position) / 2;

            float force;

            if (contract)
            {
                //Contract time
                if (ContractedLength > l)
                {
                    Left.AddConstraint(-(center - Left.Position).normalized);
                    Right.AddConstraint(-(center - Right.Position).normalized);
                    //Force extend
                    force = Mathf.Clamp(l - ExtendedLength, 0, 0);
                }
                else
                {
                    //Contract
                    force = Mathf.Clamp(l - ContractedLength, 0, 1);
                }
            }
            else
            {
                //Extend time
                if (l > ExtendedLength)
                {
                    Left.AddConstraint((center - Left.Position).normalized);
                    Right.AddConstraint((center - Right.Position).normalized);
                    //Force contract
                    force = Mathf.Clamp(l - ContractedLength, 0, 0);
                }
                else
                {
                    //Extend
                    force = Mathf.Clamp(l - ExtendedLength, -1, 0);
                }
            }

            Left.AddVelocity(Strength * force * (center - Left.Position).normalized);
            Right.AddVelocity(Strength * force * (center - Right.Position).normalized);
        }

        public void UpdateGraphics()
        {
            var distance = Vector2.Distance(Left.Position, Right.Position);

            //Position
            muscleRenderer.SetPosition(Left.Position, Right.Position);

            //Width
            var width = Mathf.Lerp(0.1f, 1, ContractedLength / distance);
            muscleRenderer.SetWidthAndColor(width, contract);
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
                var m = (Muscle)obj;
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