# OGInspector

`[Button]` adds a clickable control to the Inspector for a parameterless method on a
`MonoBehaviour` or `ScriptableObject`:

```csharp
[Button("Reset", ButtonSizes.Large, DirtyOnClick = true)]
private void ResetValues()
{
}
```

The button label defaults to the method name. `ButtonHeight` can override the
height selected by `ButtonSize`.