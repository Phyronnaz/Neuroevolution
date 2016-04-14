using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Evolution
{
    public class ControllerScript : MonoBehaviour
    {

        public Text CycleText;
        public Text DistanceText;
        public Text TimeText;

        Controller controller;
        bool hasBreak;


        public void Initialize(List<Creature> creatures)
        {
            controller = new Controller(creatures);
            InvokeRepeating("ControllerUpdate", 0, Controller.DeltaTime);

            //HACK
            controller.Train(10, (int)(100 / Controller.DeltaTime));
        }

        public void Update()
        {
            var max = controller.GetMaxPosition();

            // Update average position
            var tmp = transform.position;
            tmp.x = Mathf.Lerp(tmp.x, max, Time.deltaTime * Constants.TimeMultiplier / 10 + 0.01f);
            transform.position = tmp;

            //Remove slowest creatures
            controller.RemoveCreaturesFartherThan(50, max);

            //Update UI
            DistanceText.text = "Distance : " + max;
            CycleText.text = string.Format("{0} %", controller.GetCyclePercentageOfTheFarthestCreatures());
            TimeText.text = "Time : " + controller.CurrentTime;

            if (controller.CurrentTime > 200 && !hasBreak)
            {
                hasBreak = true;
                print(max / controller.CurrentTime);
                Debug.Break();
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                Constants.TimeMultiplier = 100;
                GetComponent<UI>().ForceUIUpdate();
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                CancelInvoke();
                controller.ResetCreatures();
                controller.CurrentTime = 0;
                InvokeRepeating("ControllerUpdate", 0, Controller.DeltaTime);
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                CancelInvoke();
                controller.Destroy();
                Destroy(this);
                GetComponent<Generate>().Restart();
            }
        }

        void ControllerUpdate()
        {
            controller.Update();
        }
    }
}