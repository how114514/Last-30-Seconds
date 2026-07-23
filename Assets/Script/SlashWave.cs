using System;
using UnityEngine;

/// <summary>
/// Plays an animation based on damage value, then auto-destroys.
/// </summary>
[Serializable]
public struct DamageAnimationPair
{
    [Tooltip("Damage >= this value triggers the animation below.")]
    public int damageThreshold;
    public string animationName;
}

[RequireComponent(typeof(Animator))]
public class SlashWave : MonoBehaviour
{
    [Header("Damage → Animation")]
    [SerializeField] private DamageAnimationPair[] m_DamageAnimations;

    private Animator m_Animator;
    private bool m_Started;

    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
    }

    /// <summary>Called by Player after setting damage. Picks and plays the matching animation.</summary>
    public void SelectAnimation(int damage)
    {
        string animName = null;
        int bestThreshold = -1;

        foreach (var pair in m_DamageAnimations)
        {
            if (damage >= pair.damageThreshold && pair.damageThreshold > bestThreshold)
            {
                bestThreshold = pair.damageThreshold;
                animName = pair.animationName;
            }
        }

        if (!string.IsNullOrEmpty(animName))
            m_Animator.Play(Animator.StringToHash(animName), 0, 0f);
    }

    private void Update()
    {
        var stateInfo = m_Animator.GetCurrentAnimatorStateInfo(0);

        if (!m_Started && stateInfo.normalizedTime > 0f)
            m_Started = true;

        if (m_Started && stateInfo.normalizedTime >= 1f)
            Destroy(gameObject);
    }
}
