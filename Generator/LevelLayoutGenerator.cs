using System;
using System.Collections;
using System.Collections.Generic;
using MykaelosUnityLevelLayoutGenerator.Utilities;
using UnityEngine;

namespace MykaelosUnityLevelLayoutGenerator.Generator {
    public class LevelLayoutGenerator {
        private MonoBehaviour Owner;
        private YieldManager YieldManager;
        private Duration Duration = new Duration();

        private Coroutine CurrentCoroutine;
        private Action Callback;

        private List<IGeneratorStep> GeneratorSteps = new List<IGeneratorStep>();
        private int CurrentGeneratorStepIndex = -1;

        private IGeneratorStep CurrentGeneratorStep {
            get {
                return CurrentGeneratorStepIndex < GeneratorSteps.Count && CurrentGeneratorStepIndex > -1 ?
                    GeneratorSteps[CurrentGeneratorStepIndex] : null;
            }
        }


        public LevelLayoutGenerator(MonoBehaviour owner) {
            Owner = owner;
            YieldManager = new YieldManager(Owner);
        }

        public void GenerateLevel(List<IGeneratorStep> generatorSteps, Action completeCallback = null) {
            if (CurrentCoroutine != null) {
                Owner.StopCoroutine(CurrentCoroutine);
            }

            Callback = completeCallback;
            Duration.Start();
            GeneratorSteps = generatorSteps.IsNotEmpty() ? generatorSteps : new List<IGeneratorStep>();
            CurrentGeneratorStepIndex = -1;

            NextPart();
        }

        private void NextPart() {
            if (++CurrentGeneratorStepIndex < GeneratorSteps.Count && CurrentGeneratorStep != null) {
                CurrentCoroutine = YieldManager.RunCoroutine(CurrentGeneratorStep.Start(this, NextPart));
            }
            else {
                Debug.Log("Finished Generating the Level!");
                CurrentCoroutine = null;
                Duration.Stop();

                Callback?.Invoke();
            }
        }

        public void IncreaseSpeed() {
            YieldManager.IncreaseSpeed();
        }

        public void DecreaseSpeed() {
            YieldManager.DecreaseSpeed();
        }

        public void ToggleYield() {
            YieldManager.IsYielding = !YieldManager.IsYielding;
        }

        public void SetYield(bool isYielding) {
            YieldManager.IsYielding = isYielding;
        }

        #region YieldManager Interface
        public IEnumerator Yield() {
            return YieldManager.Yield();
        }

        public Coroutine StartCoroutine(IEnumerator routine) {
            return YieldManager.RunCoroutine(routine);
        }
        #endregion

        #region DrawDebug - Debug visuals while generating the level
        public string GetDebugText() {
            string debugString = "";

            debugString +=
                Duration.GetDurationTimeStamp().NL() +
                "YieldTime: {0:N3}\nSpeed: {1:N1}%".FormatWith(YieldManager.YieldTime, YieldManager.SpeedFraction * 100).NL();

            if (CurrentGeneratorStep != null) {
                debugString +=
                    "Running: {0}".FormatWith(CurrentGeneratorStep.GetType().Name).NL()
                    + CurrentGeneratorStep.WriteDebug();
            }

            return debugString;
        }

        public void DrawDebug() {
            if (CurrentGeneratorStep != null) {
                CurrentGeneratorStep.DrawDebug();
            }
        }

        private static readonly Color LEVEL_RECT_GREEN = "adff2f".HexAsColor().SetA(0.5f);
        private static readonly Color BOSS_RECT_ORANGE = "FF7000".HexAsColor().SetA(1f);
        #endregion
    }
}
