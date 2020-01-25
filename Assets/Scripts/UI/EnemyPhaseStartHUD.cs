using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPhaseStartHUD : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ShowEnemyPhase(UIManager.PhaseAnimationEndCallback completeCallback)
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
