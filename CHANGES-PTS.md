# iText Source Code Changes (PTS)

## 2026-07-14

### itext/itext.layout/itext/layout/element/List.cs
Added two new `Add` methods for adding nested lists and arbitrary `IBlockElement` elements (e.g. `Paragraph`) as direct children of `List`.

```csharp
/// <summary>
/// Adds a new
/// <see cref="iText.Layout.Element.List"/>
/// (nested list) to the bottom of the List.
/// </summary>
/// <param name="list">a nested list</param>
/// <returns>this list.</returns>
public virtual iText.Layout.Element.List Add(iText.Layout.Element.List list) {
    childElements.Add(list);
    return this;
}

/// <summary>
/// Adds a new
/// <see cref="IBlockElement"/>
/// to the bottom of the List.
/// </summary>
/// <param name="element">a block element (e.g. Paragraph)</param>
/// <returns>this list.</returns>
public virtual iText.Layout.Element.List Add(IBlockElement element) {
    childElements.Add(element);
    return this;
}
```

---

### itext/itext.layout/itext/layout/renderer/ListRenderer.cs
Modified `InitializeListSymbols` to skip non-`ListItemRenderer` children (e.g. `Paragraph`, nested `List`) when assigning list symbols. Added two guard checks:

**1. First loop (symbol creation):**
```csharp
foreach (IRenderer renderer in childRenderers) {
    if (!(renderer is ListItemRenderer)) {
        // Non-ListItem children (e.g. Paragraph, nested List) do not get list symbols
        symbolRenderers.Add(null);
        continue;
    }
    // ... original logic for ListItemRenderer ...
}
```

**2. Third loop (margin and symbol assignment):**
```csharp
foreach (IRenderer childRenderer in childRenderers) {
    IRenderer symbolRenderer = symbolRenderers[listItemNum++];
    if (!(childRenderer is ListItemRenderer)) {
        // Non-ListItem children (e.g. Paragraph, nested List) do not get list symbols
        continue;
    }
    // ... original logic for ListItemRenderer ...
}
```

---

### itext/itext.layout/itext/layout/renderer/ListRenderer.cs
Modified `CorrectListSplitting` with a guard for the case when the first child renderer is not a `ListItemRenderer` (e.g. it is a `Paragraph` with the `CAPTION` role). In such a case, the overflow `ListItemRenderer` logic is skipped and a standard layout result is returned:

```csharp
IRenderer firstListItemRenderer = splitRenderer.GetChildRenderers()[0];
if (!(firstListItemRenderer is ListItemRenderer)) {
    // If the first child is not a ListItemRenderer, fall back to default behaviour
    return new LayoutResult(null == overflowRenderer ? LayoutResult.FULL : LayoutResult.PARTIAL, occupiedArea, 
        splitRenderer, overflowRenderer, this);
}
```

---
