using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace GUIs {
// 对局日志
public class JudgeLogView : MonoBehaviour {
    [SerializeField]
    private TextMeshProUGUI text;

    private readonly LinkedList<(int, string)> m_strings = new();

    private int m_index;

    // Todo 加滚动条,监听鼠标
    public void AddLog(string str) {
        m_strings.AddLast((++m_index, str));
        LoadString();
    }

    private void LoadString() {
        text.text = string.Join("\n", m_strings.Select(item => $"{item.Item1,2}:{item.Item2}"));
    }
}
}
