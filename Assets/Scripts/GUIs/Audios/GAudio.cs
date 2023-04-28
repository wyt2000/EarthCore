using System.Collections;
using GUIs.Animations;
using UnityEngine;
using Utils;

namespace GUIs.Audios {
public static class GAudio {
    private static IEnumerator WaitPlay(this AudioClip clip) {
        if (clip == null) yield break;
        AudioStore.Instance.player.PlayOneShot(clip);
        yield return GAnimation.Wait(clip.length);
    }

    private static void Play(this AudioClip clip) {
        if (clip == null) return;
        AudioStore.Instance.player.PlayOneShot(clip);
    }

    private static readonly CoroutineLocker DamageLocker = new(ResolvePolicy.Delay);

#region 静态函数

    public static void StartBattleBGM() {
        AudioStore.Instance.player.clip = AudioStore.Instance.battleBGM;
        AudioStore.Instance.player.Play();
    }

    public static void StopBattleBGM() {
        AudioStore.Instance.player.Stop();
    }

    public static void PlayButtonClick() {
        AudioStore.Instance.buttonClick.Play();
    }

    public static void PlayInvalidCard() {
        AudioStore.Instance.invalidCard.Play();
    }

    public static void PlaySelectCard() {
        AudioStore.Instance.selectCard.Play();
    }

    public static void PlayPlayCard() {
        AudioStore.Instance.playCard.Play();
    }

    public static void PlayDiscardCard() {
        AudioStore.Instance.discardCard.Play();
    }

    public static void PlayDrawCard() {
        AudioStore.Instance.drawCard.Play();
    }

    public static void PlayShuffleCard() {
        AudioStore.Instance.shuffleCard.Play();
    }

    public static IEnumerator PlayPhysicalDamage() {
        return AudioStore.Instance.damagePhysical.WaitPlay().Lock(DamageLocker);
    }

    public static IEnumerator PlayMagicDamage() {
        return AudioStore.Instance.damageMagic.WaitPlay().Lock(DamageLocker);
    }

    public static IEnumerator PlayHeal() {
        return AudioStore.Instance.heal.WaitPlay().Lock(DamageLocker);
    }

    public static IEnumerator PlaySealBreak() {
        return AudioStore.Instance.sealBreak.WaitPlay().Lock(DamageLocker);
    }

    public static IEnumerator PlaySealRecover() {
        return AudioStore.Instance.sealRecover.WaitPlay().Lock(DamageLocker);
    }

#endregion
}
}
