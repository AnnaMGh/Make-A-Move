using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEffectStopCallback : MonoBehaviour
{
    public void OnParticleSystemStopped()
    {
        //destroy the parent after particles stops
        Destroy(this.transform.parent.gameObject);
    }
}
