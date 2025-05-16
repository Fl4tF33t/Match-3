using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class AnimationManager : Singleton<AnimationManager> {
    [SerializeField] private Transform deleteVfx;
    public Action OnGemFall;
    public Action OnDeleteMatches;
    private Tween selectionTween;
    private void Start() {
        GameManager.Instance.OnSelection += GameManager_OnSelection;
    }

    private void GameManager_OnSelection(bool isSelected, Gem gem) {
        if (!isSelected) {
            if (selectionTween != null) {
                selectionTween.Pause();
                selectionTween.Kill();
            }
            gem.transform.localScale = Vector3.one;
            gem.transform.rotation = Quaternion.identity;
            return;
        }

        selectionTween = DOTween.Sequence()
            .Append(gem.transform
                .DOScale(Vector3.one * 1.1f, 0.2f)
                .SetEase(Ease.InOutSine))
            .Join(gem.transform
                .DORotate(new Vector3(0, 0, 5f), 0.2f)
                .SetEase(Ease.InOutSine))
            .Append(gem.transform
                .DOScale(Vector3.one, 0.2f)
                .SetEase(Ease.InOutSine))
            .Join(gem.transform
                .DORotate(Vector3.zero, 0.2f)
                .SetEase(Ease.InOutSine))
            .SetLoops(-1);
    }

    public IEnumerator SwapGem(GameObject gemA, GameObject gemB) {
        Vector3 posA = gemA.transform.position;
        Vector3 posB = gemB.transform.position;

        float moveDuration = 0.4f;
        float jumpPower = 0.5f;

        Sequence dtSequence = DOTween.Sequence()
            .Join(gemA.transform
                .DOJump(posB, jumpPower, 1, moveDuration)
                .SetEase(Ease.InOutQuad))
            .Join(gemB.transform
                .DOJump(posA, jumpPower, 1, moveDuration)
                .SetEase(Ease.InOutQuad))
            .Join(gemA.transform
                .DOPunchScale(Vector3.one * 0.1f, 0.2f, 4, 0.8f))
            .Join(gemB.transform
                .DOPunchScale(Vector3.one * 0.1f, 0.2f, 4, 0.8f));

        yield return dtSequence.WaitForCompletion();
    }

    public IEnumerator DeleteMatches(List<Gem> gemList) {
        foreach (Gem gem in gemList) {
            Sequence dtSequence = DOTween.Sequence()
                .Append(gem.transform.DOScale(new Vector3(1.2f, 0.8f, 1f), 0.07f)) // squash
                .Append(gem.transform.DOScale(new Vector3(0.9f, 1.1f, 1f), 0.07f)) // stretch
                .Append(gem.transform.DOScale(Vector3.zero, 0.15f).SetEase(Ease.InBack)) // shrink
                .OnComplete(() => {
                    Destroy(gem.gameObject);
                    GameObject vfx = Instantiate(deleteVfx.gameObject, gem.transform.position, Quaternion.identity);
                });

            yield return dtSequence.WaitForCompletion();
        }

        OnDeleteMatches?.Invoke();
    }

    public IEnumerator MakeGemsFall(List<Gem> gemList, List<Vector3> gemPositions) {
        Sequence dtSequence = DOTween.Sequence();

        for (int i = 0; i < gemList.Count; i++) {
            Gem gem = gemList[i];
            Vector3 targetPos = gemPositions[i];

            dtSequence
                .Join(gem.transform
                    .DOMove(targetPos, 0.3f)
                    .SetEase(Ease.OutQuad))
                .Join(gem.transform
                    .DOScale(new Vector3(1.15f, 0.85f, 1f), 0.1f)
                    .SetLoops(2, LoopType.Yoyo));
        }

        OnGemFall?.Invoke();
        yield return dtSequence.WaitForCompletion();
    }
}