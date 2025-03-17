using UnityEngine;

/// <summary>
/// Interface for all background effects used in UI transitions
/// </summary>
public interface IBackgroundEffect
{
    /// <summary>
    /// The type of background effect
    /// </summary>
    BackgroundEffectType EffectType { get; }
    
    /// <summary>
    /// Apply the background effect
    /// </summary>
    void ApplyEffect();
    
    /// <summary>
    /// Remove the background effect
    /// </summary>
    void RemoveEffect();
} 