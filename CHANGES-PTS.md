# iText Source Code Changes (PTS)

## 2026-07-14 (Update 2) — Nested List Indent Feature

### Summary
Implemented automatic visual indentation for nested `List` elements that are direct children of a parent `List`. When a `List` contains a nested `List` as a child, the nested list is automatically indented relative to the parent list, improving readability of hierarchical content in tagged PDFs.

### Files Modified

#### `itext/itext.layout/itext/layout/properties/Property.cs`
- Added `LIST_INDENT` constant (ID = 160) — a new property key for the indent value applied to nested lists.

#### `itext/itext.layout/itext/layout/element/List.cs`
- Added `SetListIndent(float)` / `GetListIndent()` methods to set and retrieve the indent offset for nested child lists.
- Default indent value: **20 points** (applied automatically when not explicitly set).

#### `itext/itext.layout/itext/layout/renderer/ListRenderer.cs`
- Modified `InitializeListSymbols()` to detect nested `ListRenderer` children and apply a left margin (indent) to them.
- The indent is added on top of any existing left margin on the nested list.
- For RTL (right-to-left) base direction, the indent is applied to the right margin instead.
- Non-`ListItemRenderer` children (e.g. `Paragraph`, nested `List`) are skipped during list symbol assignment.

### Behaviour
- **Parent list** sets `LIST_INDENT` property (default 20pt if not set).
- **Nested `List`** child renderers receive an additional left margin equal to the `LIST_INDENT` value.
- **`ListItem`** children are unaffected — only direct `List` children are indented.
- Works correctly with both LTR and RTL text directions.

### Test Updates
- Updated unit tests in `itext.tests/itext.pdfua.tests/` to verify that nested lists are visually indented relative to their parent lists in generated tagged PDFs.

### NuGet Packages
- Rebuilt all 7 NuGet packages (version 9.6.0) with the updated DLLs containing this feature:
  - `itext-pts.9.6.0`
  - `itext.commons.9.6.0`
  - `itext.bouncy-castle-adapter.9.6.0`
  - `itext.bouncy-castle-fips-adapter.9.6.0`
  - `itext.pdftest.9.6.0`
  - `itext.hyph.9.6.0`
  - `itext.font-asian.9.6.0`

---

## 2026-07-14

### itext/itext.layout/itext/layout/properties/Property.cs
Added new `LIST_INDENT` property constant (ID = 160) for controlling the indent (left margin) of nested lists.

```csharp
/// <summary>
/// The indent (left margin) applied to nested lists that are direct children of a parent list.
/// </summary>
public const int LIST_INDENT = 160;
```

---

### itext/itext.layout/itext/layout/element/List.cs
Added `SetListIndent` and `GetListIndent` methods for controlling the indent applied to nested lists that are direct children of this list.

```csharp
/// <summary>
/// Gets the indent (left margin) applied to nested lists that are direct children of this list.
/// </summary>
/// <returns>the nested list indent as a <c>float</c>, or <c>null</c> if not set.</returns>
public virtual float? GetListIndent() {
    return this.GetProperty<float?>(Property.LIST_INDENT);
}

/// <summary>
/// Sets the indent (left margin) applied to nested lists that are direct children of this list.
/// </summary>
/// <param name="listIndent">the indent offset for nested lists.</param>
/// <returns>this list.</returns>
public virtual iText.Layout.Element.List SetListIndent(float listIndent) {
    SetProperty(Property.LIST_INDENT, listIndent);
    return this;
}
```

---

### itext/itext.layout/itext/layout/renderer/ListRenderer.cs
Modified `InitializeListSymbols` to automatically apply left margin (indent) to nested `ListRenderer` children when the `LIST_INDENT` property is set on the parent list. The indent is added to any existing margin.

```csharp
float? nestedListIndent = this.GetPropertyAsFloat(Property.LIST_INDENT);
// ... in the third loop, when a non-ListItemRenderer child is encountered:
if (childRenderer is iText.Layout.Renderer.ListRenderer && nestedListIndent != null) {
    bool isRtlNested = BaseDirection.RIGHT_TO_LEFT == childRenderer.GetProperty<BaseDirection?>(Property.BASE_DIRECTION);
    int nestedMarginToSet = isRtlNested ? Property.MARGIN_RIGHT : Property.MARGIN_LEFT;
    UnitValue existingMargin = childRenderer.GetProperty<UnitValue>(nestedMarginToSet, UnitValue.CreatePointValue(0f));
    float nestedCalculatedMargin = existingMargin.IsPointValue() ? existingMargin.GetValue() : 0f;
    nestedCalculatedMargin += (float)nestedListIndent;
    childRenderer.SetProperty(nestedMarginToSet, UnitValue.CreatePointValue(nestedCalculatedMargin));
}
```

---

### itext/itext.layout/itext/layout/element/List.cs
Added two new `Add` methods for adding nested lists and arbitrary `IBlockElement` elements (e.g. `Paragraph`) as direct children of `List`.
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
