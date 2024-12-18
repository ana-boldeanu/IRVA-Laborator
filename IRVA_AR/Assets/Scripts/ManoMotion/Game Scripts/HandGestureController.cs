using UnityEngine;

namespace AR_ManoMotion
{
    public class HandGestureController : MonoBehaviour
    {
        private FruitSpawner _fruitSpawner;

        void Start() => _fruitSpawner = GetComponentInChildren<FruitSpawner>();

        void Update()
        {
            /* TODO 3.1, 4.1, 5.1 Get the hand info */
            HandInfo handInfo = ManomotionManager.Instance.Hand_infos[0].hand_info;

            /* TODO 3.2, 4.2, 5.2 Get the gesture info */
            GestureInfo gestInfo = handInfo.gesture_info;

            /* TODO 3.3. Get the hand side information */
            HandSide handSide = gestInfo.hand_side;

            /* TODO 4.3. Get the continuous gestures */
            ManoGestureContinuous gestCont = gestInfo.mano_gesture_continuous;  // Continuous gesture


            /* TODO 5.3. Get the trigger gestures */
            ManoGestureTrigger gestTrigger = gestInfo.mano_gesture_trigger;     // Trigger gesture 


            /* TODO 3.5. When the hand side is 'HandSide.Palmside', disable the fruit spawning altogether.
             * If it's 'HandSide.Backside' spawning should be active (default behavior)
             */
            _fruitSpawner.SpawnerActive = handSide != HandSide.Palmside;

            /* TODO 4.5. For each frame in which the continuous gesture 'CLOSED_HAND_GESTURE' is active, 
             * increase the spawn rate by 20 (or some number to see a difference)
             * For any other continuous gesture the spawn rate should be kept as default
             * Hint! Use the spawn rate variable found in the fruit spawner script
             * You need to figure out where to use it, changing it's value won't do anything as it's not used in code
             */
            _fruitSpawner.SpawnMultiplier = handInfo.gesture_info.mano_gesture_continuous == ManoGestureContinuous.CLOSED_HAND_GESTURE ? 20f : 1f;

            /* TODO 5.4. When the trigger gesture 'PICK' is detected, destroy all fruit instances which are on-screen.
             * Hint! Use 'DestroyFruitInstance()' function found on fruit controller script,
             * as it plays particle and sound effect as well
             */
            if (handInfo.gesture_info.mano_gesture_trigger == ManoGestureTrigger.PICK)
            {
                FruitController[] allFruit = FindObjectsOfType<FruitController>();

                foreach (FruitController fruit in allFruit)
                {
                    fruit.DestroyFruitInstance();
                }
            }

        }
    }
}