using UnityEngine;

namespace GUIs.Audios {
public class AudioStore : MonoBehaviour {
#region prefab配置

    // 战斗bgm
    public AudioClip battleBGM;

    // 按钮点击
    public AudioClip buttonClick;

    // 不可选&出牌
    public AudioClip invalidCard;

    // 选牌 
    public AudioClip selectCard;

    // 出牌
    public AudioClip playCard;

    // 弃牌
    public AudioClip discardCard;

    // 抽牌
    public AudioClip drawCard;

    // 洗牌
    public AudioClip shuffleCard;

    // 造成物理伤害
    public AudioClip damagePhysical;

    // 造成魔法伤害
    public AudioClip damageMagic;

    // 回血
    public AudioClip heal;

    // 法印击碎
    public AudioClip sealBreak;

    // 法印恢复
    public AudioClip sealRecover;

#endregion

    public AudioSource player;

    public static AudioStore Instance { get; private set; }

    private void Start() {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
}
