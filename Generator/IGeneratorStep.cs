using System;
using System.Collections;

namespace MykaelosUnityLevelLayoutGenerator.Generator {
    public interface IGeneratorStep {
        IEnumerator Start(LevelLayoutGenerator levelLayoutGenerator, Action callback);

        void DrawDebug();

        string WriteDebug();
    }
}
