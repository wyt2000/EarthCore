using System;
using System.Collections.Generic;
using UnityEngine;

namespace GUIs.Dialogs
{
    public class DialogController : MonoBehaviour
    {
        public NameBox nameBox;
        public DialogBox dialogBox;
        private Queue<Action> _actions = new Queue<Action>();

        public void Next()
        {
            if (_actions.Count == 0)
            {
                Debug.LogError("动作队列为空！");
            }
            _actions.Dequeue()?.Invoke();
        }
        
        private void Say(string character, string content)
        {
            nameBox.SetText(character);
            dialogBox.SetText(content);
        }

        private void Start()
        {
            _actions.Enqueue(() => Say("伊克斯", "这是？"));
            _actions.Enqueue(() => Say("伊克斯", "弗姆？"));
            _actions.Enqueue(() => Say("伊克斯", "你没事吧弗姆？"));
            _actions.Enqueue(() => Say("伊克斯", "你怎么了？快醒醒！"));
            _actions.Enqueue(() => Say("弗姆", "..."));
            _actions.Enqueue(() => Say("弗姆", "！！！"));
        }
        
    }
}