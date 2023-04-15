using System.Collections.Generic;
using UnityEngine;

namespace GUIs.Animations.Lerp {
public abstract class PathLerp {
    protected readonly Vector3[] Paths;

    protected PathLerp(Vector3 start, IEnumerable<Vector3> paths) {
        var list = new List<Vector3> { start };
        list.AddRange(paths);
        Paths = list.ToArray();
    }

    public abstract Vector3 Lerp(float t);
}
}
