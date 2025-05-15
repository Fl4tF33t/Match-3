using UnityEngine;
public abstract class Factory<T> : ScriptableObject {
    public abstract T Create();
}

