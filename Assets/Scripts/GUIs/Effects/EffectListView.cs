using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Combat;
using Combat.Effects;
using UnityEngine;
using Utils;

namespace GUIs.Effects {
public class EffectListView : MonoBehaviour {
#region prefab配置

    public CombatantComponent combatant;

    [SerializeField]
    private EffectView effectViewPrefab;

#endregion

    private readonly List<EffectView> m_views = new();

    public IEnumerator AddEffect(Effect effect) {
        var view = Instantiate(effectViewPrefab, transform);
        view.Container = this;
        view.Data      = effect;

        m_views.Add(view);

        view.transform.position = transform.position;
        return FreshUI();
    }

    public IEnumerator RemoveEffect(Effect effect) {
        var viewIndex = m_views.FindIndex(v => v.Data == effect);
        if (viewIndex == -1) yield break;
        var view = m_views[viewIndex];
        m_views.Remove(view);
        Destroy(view.gameObject);
        yield return FreshUI();
    }

    private IEnumerator FreshUI() {
        var cnt = m_views.Count;
        for (var i = 0; i < cnt; i++) {
            var view = m_views[i];
            view.IndexVertical   = combatant.isOtherPlayer ? i / 10 : 1 - i / 10;
            view.IndexHorizontal = i % 10;
        }

        return GCoroutine.Parallel(m_views.Select(view => view.MoveToTarget(0.1f)));
    }
}
}
