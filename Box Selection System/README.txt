How to use:

1. Drag Selection Manager Prefab directly under MainCamera
2. Implement ISelectable on every object that should be selectable and implement the logic for when selected and deselected

The Selection Manager uses a BoxCastNonAlloc.
It only detects objects that have a collider with a layer of the layermask that you can set on the Selection Manager.
Only works with Orthographic Cameras.