using UnityEngine;

[CreateAssetMenu(fileName = "GemFactory", menuName = "ScriptableObjects/GemFactory", order = 0)]
public class GemFactory : Factory<Gem> {
    public GemType[] gemTypes;
    
    public override Gem Create() {
        int randomIndex = Random.Range(0, gemTypes.Length);
        return Gem.Create(gemTypes[randomIndex]);
    }
}