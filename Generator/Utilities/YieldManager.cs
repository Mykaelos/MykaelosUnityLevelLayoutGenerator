using System;
using System.Collections;
using UnityEngine;

namespace MykaelosUnityLevelLayoutGenerator.Utilities {
    public class YieldManager {
        public bool IsYielding = false;
        public float SpeedFraction = 1.1f;
        public float YieldTime {
            get {
                return Time.deltaTime * (1 / SpeedFraction);
            }
        }

        private MonoBehaviour Owner;

        public YieldManager(MonoBehaviour owner) {
            Owner = owner;
        }


        public Coroutine RunCoroutine(IEnumerator routine) {
            if (IsYielding) {
                return Owner.StartCoroutine(routine);
            }
            else {
                IEnumerator enumerator = routine;

                while (enumerator.MoveNext()) {
                    var current = enumerator.Current;
                }
            }

            return null;
        }

        public IEnumerator Yield() {
            if (SpeedFraction < 1f) {
                yield return new WaitForSeconds(YieldTime);
            }
            else {
                yield return null;
            }
        }

        public void IncreaseSpeed() {
            SpeedFraction = MathM.MaximumClamped(SpeedFraction + 0.1f, 1.1f);
        }

        public void DecreaseSpeed() {
            SpeedFraction = MathM.MinimumClamped(SpeedFraction - 0.1f, 0.1f);
        }
    }
}
