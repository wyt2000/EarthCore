using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

namespace GUIs.Animations.Lerp {
// 线性插值
public sealed class PathLinearLerp : PathLerp {
    // PreSum[i] 表示 Node[0] 到 Node[i] 的距离
    private readonly float[] m_preSum;

    public PathLinearLerp(Vector3 start, IEnumerable<Vector3> paths) : base(start, paths) {
        m_preSum = new float[Paths.Length];
        for (var i = 1; i < Paths.Length; i++) {
            m_preSum[i] = m_preSum[i - 1] + (Paths[i] - Paths[i - 1]).magnitude;
        }
    }

    public override Vector3 Lerp(float t) {
        var index = GAlgorithm.LowerBound(m_preSum, t);
        return index + 1 >= m_preSum.Length
            ? Paths.Last()
            : Vector3.Lerp(Paths[index], Paths[index + 1], t);
    }

    public override float PredictDistance() {
        return m_preSum.Last();
    }
}
}
