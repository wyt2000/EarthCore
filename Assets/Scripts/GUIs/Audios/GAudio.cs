using UnityEngine;

namespace GUIs.Audios {
public static class GAudio {
    private static void Play(this AudioClip clip) {
        if (clip == null) return;
        AudioStore.Instance.player.PlayOneShot(clip);
    }

#region 静态函数

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

    public static void PlayDamage() {
        AudioStore.Instance.damage.Play();
    }

    public static void PlayHeal() {
        AudioStore.Instance.heal.Play();
    }

    public static void PlaySealBreak() {
        AudioStore.Instance.sealBreak.Play();
    }

    public static void PlaySealRecover() {
        AudioStore.Instance.sealRecover.Play();
    }

#endregion
}
}
