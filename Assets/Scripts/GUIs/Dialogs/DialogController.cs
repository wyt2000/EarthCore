using System;
using System.Collections.Generic;
using UnityEngine;

namespace GUIs.Dialogs {
public class DialogController : MonoBehaviour {
    public NameBox   nameBox;
    public DialogBox dialogBox;

    private readonly Queue<Action> m_actions = new();

    public void Next() {
        if (m_actions.Count == 0) {
            return;
        }
        m_actions.Dequeue()?.Invoke();
    }

    private void Say(string character, string content) {
        nameBox.SetText(character);
        dialogBox.SetText(content);
    }

    private void Start() {
        m_actions.Enqueue(() => Say("伊克斯", "这是？"));
        m_actions.Enqueue(() => Say("伊克斯", "弗姆？"));
        m_actions.Enqueue(() => Say("伊克斯", "你没事吧弗姆？"));
        m_actions.Enqueue(() => Say("伊克斯", "你怎么了？快醒醒！"));
        m_actions.Enqueue(() => Say("弗姆",  "..."));
        m_actions.Enqueue(() => Say("弗姆",  "！！！"));
    }
}
}
