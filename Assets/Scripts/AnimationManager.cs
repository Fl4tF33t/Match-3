using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class AnimationManager : Singleton<AnimationManager> {
    [SerializeField] private Ease ease = Ease.InQuad;
    public Action OnGemFall;
    public Action OnDeleteMatches;
    public IEnumerator SwapGem(GameObject gemA, GameObject gemB) {
        Sequence dtSequence = DOTween.Sequence();
        dtSequence
            .Append(
                gemA.transform
                    .DOMove(gemB.transform.position, 0.5f)
                    .SetEase(ease))
            .Join(
                gemB.transform
                    .DOMove(gemA.transform.position, 0.5f)
                    .SetEase(ease));

        yield return dtSequence.WaitForCompletion();
    }

    public IEnumerator DeleteMatches(List<Gem> gemList) {
        Sequence dtSequence = DOTween.Sequence();
        foreach (Gem gem in gemList) {
            dtSequence
                .Join(
                    gem.transform
                        .DOPunchScale(Vector3.one * 0.1f, 0.1f, 1, 0.5f)
                        .OnComplete(
                            () => Destroy(gem.gameObject)));
        }
        
        OnDeleteMatches?.Invoke(); 
        yield return dtSequence.WaitForCompletion();
    }

    public IEnumerator MakeGemsFall(List<Gem> gemList, List<Vector3> gemPositions) {
        Sequence dtSequence = DOTween.Sequence();
        for (int i = 0; i < gemList.Count; i++) {
            dtSequence
                .Join(
                    gemList[i].transform
                        .DOMove(gemPositions[i], 0.5f));
        }
        
        OnGemFall?.Invoke();
        yield return dtSequence.WaitForCompletion();
    }
}