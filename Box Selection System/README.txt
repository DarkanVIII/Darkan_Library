How to use:

1. Drag Selection Manager Prefab directly under MainCamera
2. Implement ISelectable and IMarkable on every object that should be selectable and implement the logic for when selected and deselected

ISelectable: when user finished the selection by letting go of the cursor
IMarkable: called on all objects on Update that are inside selection box, unmarked on letting go of cursor

The Selection Manager uses a BoxCastNonAlloc.
It only detects objects that have a collider with a layer of the layermask that you can set on the Selection Manager.
Only works with Orthographic Cameras.