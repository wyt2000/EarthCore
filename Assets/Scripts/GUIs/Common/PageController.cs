using System;
using UnityEngine;

namespace GUIs.Common {
// 分页控制器
public class PageController : MonoBehaviour {
    private GameObject[] m_pages;

    private void Start() {
        m_pages = new GameObject[transform.childCount];
        for (var i = 0; i < transform.childCount; i++) {
            m_pages[i] = transform.GetChild(i).gameObject;
        }
    }

    private void Select(int index) {
        if (index < 0 || index >= m_pages.Length) {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        for (var i = 0; i < m_pages.Length; i++) {
            m_pages[i].SetActive(i == index);
        }
    }

    public void Select(Enum index) => Select(Convert.ToInt32(index));
}
}
