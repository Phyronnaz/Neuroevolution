using UnityEngine;

namespace Evolution
{
    public class IntelligentMuscle : Muscle
    {
        public readonly float Strength;
        public readonly float ExtendedLength;
        public readonly float ContractedLength;
        public readonly float ChangeTime;
        public readonly bool BeginWithContraction;

        bool contract;


        public IntelligentMuscle(Node left, Node right, float strength, float extendedLength, float contractedLength, float changeTime, bool beginWithContraction, Color color, Transform parent)
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
            muscleRenderer.gameObject.name = "Intelligent Muscle from " + left.Id + " to " + right.Id;
            muscleRenderer.MaxLength = ExtendedLength;
            muscleRenderer.MinLength = ContractedLength;
            muscleRenderer.Color = color;
            muscleRenderer.transform.parent = parent;
            muscleRenderer.Initialize();
            UpdateGraphics();
        }


        public static IntelligentMuscle RandomMuscle(Node left, Node right, float cycleDuration, Color color, Transform parent)
        {
            var distance = Vector2.Distance(left.Position, right.Position);
            var contractedLength = distance - Random.Range(Constants.MinRandom, Constants.ContractedDistanceMultiplier);
            var extendedLength = distance + Random.Range(Constants.MinRandom, Constants.ExtendedDistanceMultiplier);

            var strength = Random.Range(Constants.MinStrength, Constants.StrengthAmplitude);
            var muscleCycleDuration = Random.Range(Constants.MinRandom, cycleDuration);
            var beginWithContraction = (Random.value > 0.5f);

            return new IntelligentMuscle(left, right, strength, extendedLength, contractedLength, muscleCycleDuration, beginWithContraction, color, parent);
        }

        public override Muscle Clone(Node left, Node right, float variationAmplitude, Color color, Transform parent)
        {
            return new IntelligentMuscle(
                left,
                right,
                Strength * (1 + Random.Range(-variationAmplitude, variationAmplitude)),
                ExtendedLength * (1 + Random.Range(-variationAmplitude, variationAmplitude)),
                ContractedLength * (1 + Random.Range(-variationAmplitude, variationAmplitude)),
                ChangeTime * (1 + Random.Range(-variationAmplitude, variationAmplitude)),
                (Random.value > variationAmplitude) ? BeginWithContraction : !BeginWithContraction,
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

        public override void Update()
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

        public override void UpdateGraphics()
        {
            //Position
            base.UpdateGraphics();

            //Width
            var distance = Vector2.Distance(Left.Position, Right.Position);
            var width = Mathf.Lerp(0.1f, 1, ContractedLength / distance);
            muscleRenderer.SetWidthAndColor(width, contract);
        }
    }
}