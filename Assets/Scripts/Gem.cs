using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Gem : MonoBehaviour, IGem{
    private GemType gemType;
    public static Gem Create(GemType gemType) {
        GameObject gemObj = new GameObject("Gem");
        Gem gem = gemObj.AddComponent<Gem>();
        gem.gemType = gemType;
        if (gem.TryGetComponent(out SpriteRenderer spriteRenderer)) {
            spriteRenderer.sprite = gemType.sprite;
        }
        else { 
            SpriteRenderer addedSpriteRenderer = gemObj.AddComponent<SpriteRenderer>();  
            addedSpriteRenderer.sprite = gemType.sprite;
        }
        return gem;
    }
    public void SetType(GemType gemType){
        // this.gemType = gemType;
        // GetComponent<SpriteRenderer>().sprite = gemType.sprite;
    }
    public new GemType GetType() => gemType;

    public void DestroyGem() => Destroy(gameObject);

    
}
public interface IGem {
    public GemType GetType();
    public void DestroyGem();
}