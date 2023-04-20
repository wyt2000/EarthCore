using System;
using System.Collections;
using System.Reflection;
using DG.Tweening;
using DG.Tweening.Core;
using UnityEngine;

namespace GUIs.Animations {
public static class GAnimation {
    // 插值动画
    public static IEnumerator Lerp(float duration, Action<float> action, Func<float, float> curve = null) {
        if (duration <= 0) {
            action(curve?.Invoke(1) ?? 1);
            yield break;
        }
        var startTime = Time.time;
        var endTime = startTime + duration;
        float current;
        do {
            current = Time.time;
            var t = Mathf.Clamp01((current - startTime) / duration);
            action(curve?.Invoke(t) ?? t);
            yield return null;
        } while (current < endTime);
    }

    //  IEnumerator WaitForCompletion(Tween t)
    private static readonly Func<Tween, IEnumerator> WaitForCompletion
        = (Func<Tween, IEnumerator>)typeof(DOTweenComponent)
            .GetMethod("WaitForCompletion", BindingFlags.Instance | BindingFlags.NonPublic)?
            .CreateDelegate(typeof(Func<Tween, IEnumerator>), DOTween.instance);

    // DOTWeen转迭代器 Todo! 重复执行会有冲突,导致各种奇奇怪怪的bug(如shake后不归位)
    public static IEnumerator Wait(this Tween tween) {
        // var finish = false;
        // tween.OnComplete(() => finish = true);
        // while (!finish) yield return null;
        // // Todo! 改成直接返回YieldInstruction
        //
        //
        //
        // yield return tween.WaitForCompletion();
        var id = tween.GetHashCode();
        Debug.Log($"begin : {id}");
        yield return WaitForCompletion?.Invoke(tween);
        Debug.Log($"end : {id}");
    }

    // 动画转迭代器
    public static IEnumerator Wait(this Animation animation) {
        animation.Play();
        while (animation.isPlaying) yield return null;
    }

    // 指定动画片段转迭代器
    public static IEnumerator Wait(this Animation animation, string name) {
        animation.Play(name);
        while (animation.IsPlaying(name)) yield return null;
    }

    // 粒子系统转迭代器
    public static IEnumerator Wait(this ParticleSystem particleSystem) {
        particleSystem.Play();
        while (particleSystem.isPlaying) yield return null;
    }

    // 等待若干秒
    public static IEnumerator Wait(float duration) {
        var target = Time.time + duration;
        while (Time.time < target) yield return null;
    }
}
}
