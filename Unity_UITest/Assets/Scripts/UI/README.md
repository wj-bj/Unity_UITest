# UI Management System

This UI system provides a comprehensive solution for managing UI windows, transitions, and effects in Unity.

## Features

- **Layer-based UI Management**: Organize UI elements in different layers (BOTTOM, STACK, FLOAT, TOP)
- **Background Effects**: Apply different background effects like blur, color overlay, etc.
- **Foreground Effects**: Apply different transition effects like fade, scale, slide, etc.
- **UI Event System**: Event-based UI management for easy integration
- **Singleton Pattern**: Easy access to the UI manager from anywhere in your code

## Setup Instructions

### 1. Scene Setup

Create a scene structure as follows:

```
- Managers
    - UIManager (Add UIManager script)
    - Other managers...

- Canvas (Main Canvas)
    - BackgroundPanel
        - BlurEffect (Add UIBlurEffect script and RawImage component)
        - ColorEffect (Add UIColorEffect script and Image component)
        - Other background effects...
    - UI Windows (Add UIWindowBase script to each window)
    - ForegroundPanel
        - FadeEffect (Add UITransitionEffect script with Fade type)
        - FlashEffect (Add UITransitionEffect script with Flash type)
        - Other foreground effects...
```

### 2. Effect Setup

1. Create background effects:
   - Implement the `IBackgroundEffect` interface
   - Add them as children of the BackgroundPanel
   - Register them in the UIManager

2. Create foreground effects:
   - Implement the `IForegroundEffect` interface
   - Add them as children of the ForegroundPanel
   - Register them in the UIManager

### 3. UI Window Setup

For each UI window:

1. Add a Canvas component
2. Add a CanvasGroup component
3. Add a UIWindowBase component (or a script that inherits from it)
4. Configure the window settings (name, layer, background effect, foreground effect)

## Usage Examples

### Opening a UI Window

```csharp
// Show a UI window using the singleton
UIManager.Instance.Show("WindowName");
```

### Closing a UI Window

```csharp
// Close a UI window using the singleton
UIManager.Instance.Close("WindowName");
```

### Listening to UI Events

```csharp
// Subscribe to UI events
UIManager.Instance.OnUIShow += HandleUIShow;
UIManager.Instance.OnUIAnimationStart += HandleUIAnimationStart;
UIManager.Instance.OnUIAnimationEnd += HandleUIAnimationEnd;
UIManager.Instance.OnUIClose += HandleUIClose;
UIManager.Instance.OnUIScreenCaptured += HandleUIScreenCaptured;

// Event handlers
private void HandleUIShow(string windowName)
{
    Debug.Log($"UI Window {windowName} shown");
}
```

## Creating Custom Effects

### Custom Background Effect

```csharp
public class MyCustomBackgroundEffect : MonoBehaviour, IBackgroundEffect
{
    public BackgroundEffectType EffectType => BackgroundEffectType.Custom;
    
    public void ApplyEffect()
    {
        // Apply your custom background effect
    }
    
    public void RemoveEffect()
    {
        // Remove your custom background effect
    }
}
```

### Custom Foreground Effect

```csharp
public class MyCustomForegroundEffect : MonoBehaviour, IForegroundEffect
{
    public ForegroundEffectType EffectType => ForegroundEffectType.Custom;
    
    public void PlayEffect()
    {
        // Play your custom foreground effect
    }
    
    public void StopEffect()
    {
        // Stop your custom foreground effect
    }
}
```

## Notes

- The UI system requires DOTween for animations. Make sure it's imported in your project.
- For blur effects, the UIBlur shader must be assigned to the blur material.
- UI windows are automatically registered with the UIManager on startup.
- The UIManager uses the singleton pattern for easy access from anywhere in your code.
- Background and foreground effects can be combined for rich UI transitions. 