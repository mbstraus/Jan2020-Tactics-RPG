using UnityEngine;
using DG.Tweening;
using System.Collections;

public class PlayerPhaseStartHUD : MonoBehaviour
{
    public void ShowPlayerPhase(UIManager.PhaseAnimationEndCallback completeCallback)
    {
        StartCoroutine("Animate", completeCallback);
    }

    public IEnumerator Animate(UIManager.PhaseAnimationEndCallback completeCallback)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        Sequence animationSequence = DOTween.Sequence();
        animationSequence.Append(rectTransform.DOAnchorPosY(0, 1f));
        animationSequence.AppendInterval(1f);
        animationSequence.Append(rectTransform.DOAnchorPosY(40, 1f));
        animationSequence.Play();
        yield return animationSequence.WaitForCompletion();

        completeCallback();
    }
}
