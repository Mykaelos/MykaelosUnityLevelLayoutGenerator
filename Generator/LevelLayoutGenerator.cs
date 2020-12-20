using System;
using System.Collections;
using System.Collections.Generic;
using MykaelosUnityLevelLayoutGenerator.Utilities;
using UnityEngine;
using UnityEngine.UI;

public class LevelLayoutGenerator {
    private MonoBehaviour Owner;
    private YieldManager YieldManager;
    private Duration Duration = new Duration();
    private Text DebugText;

    private LevelLayoutData LevelLayoutData;
    private LevelRequirements LevelRequirements;
    private Coroutine CurrentCoroutine;
    private Action<LevelLayoutData> Callback;

    private List<IGeneratorPart> GeneratorParts = new List<IGeneratorPart>();
    private int CurrentGeneratorPartIndex = -1;

    private IGeneratorPart CurrentGeneratorPart {
        get {
            return CurrentGeneratorPartIndex < GeneratorParts.Count && CurrentGeneratorPartIndex > -1 ?
                GeneratorParts[CurrentGeneratorPartIndex] : null;
        }
    }


    public LevelLayoutGenerator(MonoBehaviour owner, Text debugTextBox = null) {
        Owner = owner;
        DebugText = debugTextBox;
        YieldManager = new YieldManager(Owner);
    }

    public void GenerateLevel(LevelRequirements levelRequirements, Action<LevelLayoutData> completeCallback = null) {
        if (CurrentCoroutine != null) {
            Owner.StopCoroutine(CurrentCoroutine);
        }

        Callback = completeCallback;
        Duration.Start();
        LevelLayoutData = new LevelLayoutData();
        LevelRequirements = levelRequirements;
        GeneratorParts = new List<IGeneratorPart> {
            new ClusteredRoomPlacer(),
            new PrepareRoomCells(),
            new PortalPlacer(),
            new SurroundRoomsWithWalls(),
            new GenerateCellMetaData(),
            new GenerateTreasureAndEnemies()
        };
        CurrentGeneratorPartIndex = -1;

        NextPart();
    }

    private void NextPart() {
        if (++CurrentGeneratorPartIndex < GeneratorParts.Count && CurrentGeneratorPart != null) {
            CurrentCoroutine = YieldManager.RunCoroutine(CurrentGeneratorPart.Start(LevelRequirements, LevelLayoutData, this, NextPart));
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

            if (CurrentGeneratorPart != null) {
                debugString +=
                    "Running: {0}".FormatWith(CurrentGeneratorPart.GetType().Name).NL()
                    + CurrentGeneratorPart.WriteDebug();
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
        }

        if (CurrentGeneratorPart != null) {
            CurrentGeneratorPart.DrawDebug();
        }
    }

    private static readonly Color LEVEL_RECT_GREEN = "adff2f".HexAsColor().SetA(0.5f);
    #endregion
}

public interface IGeneratorPart {
    IEnumerator Start(LevelRequirements levelRequirements, LevelLayoutData levelLayoutData, LevelLayoutGenerator levelLayoutGenerator, Action callback);
    void DrawDebug();
    string WriteDebug();
}
