using System;
using System.Collections;

namespace MykaelosUnityLevelLayoutGenerator.Generator {
    public interface IGeneratorStep {
        IEnumerator Start(LevelRequirements levelRequirements, LevelLayoutData levelLayoutData, LevelLayoutGenerator levelLayoutGenerator, Action callback);
        void DrawDebug();
        string WriteDebug();
    }
}
