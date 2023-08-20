using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(ParticleSystem))]
public class SimulateSeamlessParticle : MonoBehaviour
{
    [SerializeField]
    ParticleSystem ps;

    void LateUpdate()
    {
        if (ps.time >= ps.main.duration)
        {
            ps.Stop();
            ps.Play();
        }
    }

    private void Reset()
    {
        ps = GetComponent<ParticleSystem>();
        ps.randomSeed = 1;
    }
}
