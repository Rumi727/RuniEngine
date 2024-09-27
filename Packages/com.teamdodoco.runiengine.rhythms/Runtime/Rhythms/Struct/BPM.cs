#nullable enable
using System;
using UnityEngine;

namespace RuniEngine.Rhythms
{
    [Serializable]
    public struct BPM
    {
        [FieldName("inspector.rhythmable.bpm"), Min(0)] public double bpm;
        [FieldName("inspector.rhythmable.bpm.timeSignatures"), Min(0)] public double timeSignatures;

        public BPM(double bpm, double timeSignatures = 4)
        {
            this.bpm = bpm;
            this.timeSignatures = timeSignatures;
        }
    }
}
