using UnityEngine;

/// <summary>
/// Interface for all foreground effects used in UI transitions
/// </summary>
public interface IForegroundEffect
{
    ForegroundEffectType EffectType { get; }
    /// <summary>
    /// Play the foreground effect
    /// </summary>
    void PlayEffect();
    
    /// <summary>
    /// Stop the foreground effect
    /// </summary>
    void StopEffect();
} 