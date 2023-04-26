using System;
using System.Collections;
using System.Collections.Generic;
using Combat.Story.Actions;
using Controllers;

namespace Combat.Story {
public abstract class StoryAction {
    private static readonly Dictionary<string, Type> ActionTypes = new() {
        { "init", typeof(ActionInit) },
        { "start", typeof(ActionStart) },
        { "say", typeof(ActionSay) },
        { "show", typeof(ActionShow) },
        { "hide", typeof(ActionHide) },
        { "wait", typeof(ActionWait) },
        { "play", typeof(ActionPlay) },
        { "next", typeof(ActionNext) },
    };

    public static StoryAction GetAction(string command) {
        if (ActionTypes.TryGetValue(command, out var type)) {
            return Activator.CreateInstance(type) as StoryAction;
        }

        return null;
    }

    public abstract StoryAction Build(IReadOnlyList<string> args);

    public abstract IEnumerator Execute(CombatController controller);

    protected abstract string ToDescription();

    public override sealed string ToString() {
        return ToDescription();
    }
}
}
