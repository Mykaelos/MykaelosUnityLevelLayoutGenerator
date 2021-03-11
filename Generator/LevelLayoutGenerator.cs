using System;
using System.Collections;
using System.Collections.Generic;
using MykaelosUnityLevelLayoutGenerator.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace MykaelosUnityLevelLayoutGenerator.Generator {
    public class LevelLayoutGenerator {
        private MonoBehaviour Owner;
        private YieldManager YieldManager;
        private Duration Duration = new Duration();
        private Text DebugText;

        private LevelLayoutData LevelLayoutData;
        private LevelRequirements LevelRequirements;
        private Coroutine CurrentCoroutine;
        private Action<LevelLayoutData> Callback;

        private List<IGeneratorStep> GeneratorSteps = new List<IGeneratorStep>();
        private int CurrentGeneratorStepIndex = -1;

        private IGeneratorStep CurrentGeneratorStep {
            get {
                return CurrentGeneratorStepIndex < GeneratorSteps.Count && CurrentGeneratorStepIndex > -1 ?
                    GeneratorSteps[CurrentGeneratorStepIndex] : null;
            }
        }


        public LevelLayoutGenerator(MonoBehaviour owner, Text debugTextBox = null) {
            Owner = owner;
            DebugText = debugTextBox;
            YieldManager = new YieldManager(Owner);
        }

        public void GenerateLevel(LevelRequirements levelRequirements, List<IGeneratorStep> generatorSteps, Action<LevelLayoutData> completeCallback = null) {
            if (CurrentCoroutine != null) {
                Owner.StopCoroutine(CurrentCoroutine);
            }

            Callback = completeCallback;
            Duration.Start();
            LevelLayoutData = new LevelLayoutData();
            LevelRequirements = levelRequirements;
            GeneratorSteps = generatorSteps.IsNotEmpty() ? generatorSteps : new List<IGeneratorStep>();
            CurrentGeneratorStepIndex = -1;

            NextPart();
        }

        private void NextPart() {
            if (++CurrentGeneratorStepIndex < GeneratorSteps.Count && CurrentGeneratorStep != null) {
                CurrentCoroutine = YieldManager.RunCoroutine(CurrentGeneratorStep.Start(LevelRequirements, LevelLayoutData, this, NextPart));
            }
            else {
                Debug.Log("Finished Generating the Level!");
                CurrentCoroutine = null;
                Duration.Stop();

                Callback?.Invoke(LevelLayoutData);
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
        public void DrawDebugText() {
            if (DebugText != null) {
                string debugString = "";

                debugString +=
                    Duration.GetDurationTimeStamp().NL() +
                    "YieldTime: {0:N3}\nSpeed: {1:N1}%".FormatWith(YieldManager.YieldTime, YieldManager.SpeedFraction * 100).NL();

                if (LevelLayoutData != null) {
                    if (!LevelLayoutData.Rooms.IsNullOrEmpty()) {
                        debugString += "Total Rooms: {0}".FormatWith(LevelLayoutData.Rooms.Count).NL();
                    }
                    if (!LevelLayoutData.Cells.IsNullOrEmpty()) {
                        debugString += "Total Cells: {0}".FormatWith(LevelLayoutData.Cells.Count).NL();
                    }
                    debugString = debugString.NL();
                }

                if (CurrentGeneratorStep != null) {
                    debugString +=
                        "Running: {0}".FormatWith(CurrentGeneratorStep.GetType().Name).NL()
                        + CurrentGeneratorStep.WriteDebug();
                }

                DebugText.text = debugString;
            }
        }

        public void DrawDebug() {
            if (LevelLayoutData != null) {
                foreach (var room in LevelLayoutData.Rooms) {
                    room.DrawDebug();
                }

                if (LevelLayoutData.LevelRect != Rect.zero) {
                    GizmosM.DrawRect(LevelLayoutData.LevelRect, LEVEL_RECT_GREEN);
                }

                if (LevelLayoutData.BossRoom != null) {
                    GizmosM.DrawRect(LevelLayoutData.BossRoom.Rect, Color.red);
                    //GizmosM.DrawRect(new Rect(LevelLayoutData.BossRoom.Rect.position - (Vector2.one * 0.5f), LevelLayoutData.BossRoom.Rect.size), BOSS_RECT_ORANGE); //TODO - Fix this for real in the generator.
                }

                if (LevelLayoutData.StartingRoom != null) {
                    GizmosM.DrawRect(LevelLayoutData.StartingRoom.Rect, Color.green);
                    //GizmosM.DrawRect(new Rect(LevelLayoutData.BossRoom.Rect.position - (Vector2.one * 0.5f), LevelLayoutData.BossRoom.Rect.size), BOSS_RECT_ORANGE); //TODO - Fix this for real in the generator.
                }
            }

            if (CurrentGeneratorStep != null) {
                CurrentGeneratorStep.DrawDebug();
            }
        }

        private static readonly Color LEVEL_RECT_GREEN = "adff2f".HexAsColor().SetA(0.5f);
        private static readonly Color BOSS_RECT_ORANGE = "FF7000".HexAsColor().SetA(1f);
        #endregion
    }
}
