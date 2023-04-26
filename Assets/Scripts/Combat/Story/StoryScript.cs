namespace Combat.Story {
public class StoryScript {
    private readonly StoryAction[] m_actions;

    private int m_currentIndex;

    public StoryAction Current => m_currentIndex >= m_actions.Length ? null : m_actions[m_currentIndex];

    public StoryScript(string name) {
        m_actions = StoryCreator.Load(name);
    }

    public void Finish() {
        ++m_currentIndex;
    }
}
}
