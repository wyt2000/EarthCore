namespace Combat.States {
public static class DynamicOperator {
    public static T ForceAdd<T>(T a, T b) {
        return (dynamic)a + (dynamic)b;
    }

    public static T ForceSub<T>(T a, T b) {
        return (dynamic)a - (dynamic)b;
    }
}
}
