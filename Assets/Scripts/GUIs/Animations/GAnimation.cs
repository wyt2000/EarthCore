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

    // DOTWeen转迭代器
    public static IEnumerator Wait(this Tween tween) {
        // 先暂停再延迟play,避免delay无效
        tween.Pause();
        // 此处不用yield防止延迟执行
        return ImpWait(tween);
    }

    private static IEnumerator ImpWait(Tween tween) {
        tween.Play();
        yield return WaitForCompletion?.Invoke(tween);
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
