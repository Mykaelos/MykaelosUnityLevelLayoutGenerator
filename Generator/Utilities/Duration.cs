using System;
using UnityEngine;

namespace MykaelosUnityLevelLayoutGenerator.Utilities {
    public class Duration {
        private float StartTime;
        private float StopTime;
        private bool IsTiming = false;


        public void Start() {
            StartTime = Time.time;
            IsTiming = true;
        }

        public void Stop() {
            StopTime = Time.time;
            IsTiming = false;
        }

        public float GetDuration() {
            return IsTiming ? Time.time - StartTime : StopTime - StartTime;
        }

        public string GetDurationTimeStamp() {
            var timeSpan = TimeSpan.FromSeconds(GetDuration());
            return "{0:D2}:{1:D2}:{2:D3}".FormatWith(timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);
        }
    }
}
