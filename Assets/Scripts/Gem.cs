using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Gem : MonoBehaviour{
    [SerializeField] private GemType gemType;

    public void SetType(GemType gemType){
        this.gemType = gemType;
        GetComponent<SpriteRenderer>().sprite = gemType.sprite;
    }
    new public GemType GetType() => gemType;
     
}