using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GrassTrampler : MonoBehaviour
{
    [SerializeField] UniversalRendererData rendererSetting = null;

    private bool TryGetFeature(out GrassTramplePassFeature feature)
    {
        feature = rendererSetting.rendererFeatures.OfType<GrassTramplePassFeature>().FirstOrDefault();
        return feature != null;
    }

    private void OnEnable()
    {
        if(TryGetFeature(out var feature))
        {
            feature.AddTrackedTransform(transform);
        }
    }

    private void OnDisable()
    {
        if (TryGetFeature(out var feature))
        {
            feature.RemoveTrackedTransform(transform);
        }
    }
}
