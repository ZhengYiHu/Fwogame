using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] CanvasGroup rootCanvasGroup;
    [SerializeField] float lerpSpeed;

    [ShowNonSerializedField] private float currentValue = 0;
    private bool _uiVisible;
    public bool UIVisible
    {
        set
        {
            _uiVisible = value;
            TweenAlpha(value).Forget();
        }
        get
        {
            return _uiVisible;
        }
    }

    private async UniTask TweenAlpha(bool visible)
    {
        float elapsedTime = 0;
        float targetValue = visible ? 1 : 0;
        float initialValue = currentValue;

        while (elapsedTime < lerpSpeed)
        {
            currentValue = Mathf.Lerp(initialValue, targetValue, elapsedTime / lerpSpeed);
            rootCanvasGroup.alpha = currentValue;
            elapsedTime += Time.deltaTime;

            await UniTask.Yield();
        }
        // Ensure the final value is exactly the target value
        rootCanvasGroup.alpha = targetValue;
    }

}
