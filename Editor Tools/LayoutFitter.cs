using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class LayoutFitter : SerializedMonoBehaviour
{
    #region Grid
    enum BuildType { Rows, Columns };

    [TabGroup("Grid")]
    [OnValueChanged(nameof(MakeGrid), InvokeOnUndoRedo = true)]
    [EnumToggleButtons]
    [SerializeField]
    BuildType _buildBy = BuildType.Rows;

    [TabGroup("Grid")]
    [OnValueChanged(nameof(MakeGrid), InvokeOnUndoRedo = true)]
    [MinValue(1)]
    [SerializeField]
    int _itemCount = 1;

    [TabGroup("Grid")]
    [OnValueChanged(nameof(MakeGrid), InvokeOnUndoRedo = true)]
    [LabelText("Spacing")]
    [MinValue(0)]
    [SerializeField]
    float _gridSpacing = 0;

    #endregion

    #region Single Row

    enum FitDimension { Vertically, Horizontally };
    enum AlignVertically { Top, Center, Bottom };
    enum AlignHorizontally { Left, Center, Right };

    [EnumToggleButtons]
    [TabGroup("Single Row")]
    [OnValueChanged(nameof(FitRow), InvokeOnUndoRedo = true)]
    [SerializeField]
    FitDimension _fitDimension = FitDimension.Vertically;

    [EnumToggleButtons]
    [TabGroup("Single Row")]
    [OnValueChanged(nameof(FitRow), InvokeOnUndoRedo = true)]
    [HideIf(nameof(_fitDimension), FitDimension.Horizontally)]
    [SerializeField]
    AlignHorizontally _alignHorizontally = AlignHorizontally.Center;

    [EnumToggleButtons]
    [TabGroup("Single Row")]
    [OnValueChanged(nameof(FitRow), InvokeOnUndoRedo = true)]
    [HideIf(nameof(_fitDimension), FitDimension.Vertically)]
    [SerializeField]
    AlignVertically _alignVertically = AlignVertically.Center;

    [TabGroup("Single Row")]
    [OnValueChanged(nameof(FitRow), InvokeOnUndoRedo = true)]
    [HideIf(nameof(_fitDimension), FitDimension.Horizontally)]
    [MinValue(0)]
    [SerializeField]
    float _width;

    [TabGroup("Single Row")]
    [OnValueChanged(nameof(FitRow), InvokeOnUndoRedo = true)]
    [HideIf(nameof(_fitDimension), FitDimension.Vertically)]
    [MinValue(0)]
    [SerializeField]
    float _height;

    [TabGroup("Single Row")]
    [OnValueChanged(nameof(FitRow), InvokeOnUndoRedo = true)]
    [LabelText("Spacing")]
    [MinValue(0)]
    [SerializeField]
    float _rowSpacing = 0;

    #endregion


    readonly List<RectTransform> childTransforms = new();
    //readonly List<Vector2> savedAnchorsMin = new();
    //readonly List<Vector2> savedAnchorsMax = new();
    //readonly List<Vector2> savedSizes = new();
    //readonly List<Vector2> savedPositions = new();

    void FitRow()
    {
        switch (_fitDimension)
        {
            case FitDimension.Vertically:
                FitVertically();
                break;
            case FitDimension.Horizontally:
                FitHorizontally();
                break;
        }
    }

    void FitHorizontally()
    {
        GetChildTransforms(out int childCount);

        float fraction = 1f / childCount;
        float currFraction = 0;

        float reducedWidth = (_rowSpacing * (childCount - 1)) / childCount;
        float spacingPrevious = 0;

        for (int indexTrans = 0; indexTrans <= childCount - 1; indexTrans++)
        {
            var rectTrans = childTransforms[indexTrans];

            Vector2 anchorMin = rectTrans.anchorMin;
            Vector2 anchorMax = rectTrans.anchorMax;

            switch (_alignVertically)
            {
                case AlignVertically.Top:
                    anchorMin.y = 1;
                    anchorMax.y = 1;
                    rectTrans.anchoredPosition = new Vector2(0, -_height / 2);
                    break;
                case AlignVertically.Center:
                    anchorMin.y = .5f;
                    anchorMax.y = .5f;
                    rectTrans.anchoredPosition = new Vector2(0, 0);
                    break;
                case AlignVertically.Bottom:
                    anchorMin.y = 0;
                    anchorMax.y = 0;
                    rectTrans.anchoredPosition = new Vector2(0, _height / 2);
                    break;
            }

            anchorMin.x = currFraction;
            rectTrans.anchorMin = anchorMin;

            currFraction += fraction;

            anchorMax.x = currFraction;
            rectTrans.anchorMax = anchorMax;

            rectTrans.sizeDelta = new Vector2(0, _height);

            if (_rowSpacing > 0) // Apply Spacing
            {
                Vector2 sizeDelta = rectTrans.sizeDelta;
                sizeDelta.x -= reducedWidth;
                rectTrans.sizeDelta = sizeDelta;

                Vector2 anchoredPosition = rectTrans.anchoredPosition;

                if (indexTrans == 0) // First child
                {
                    anchoredPosition.x -= reducedWidth * 0.5f;
                    spacingPrevious = -anchoredPosition.x + reducedWidth * 0.5f;
                }
                else
                {
                    anchoredPosition.x += -reducedWidth * 0.5f + _rowSpacing - spacingPrevious;
                    spacingPrevious = -anchoredPosition.x + reducedWidth * 0.5f;
                }

                rectTrans.anchoredPosition = anchoredPosition;
            }
        }
    }

    void FitVertically()
    {
        GetChildTransforms(out int childCount);

        float fraction = 1f / childCount;
        float currFraction = 0;

        float reducedHeight = (_rowSpacing * (childCount - 1)) / childCount;
        float spacingPrevious = 0;

        for (int indexTrans = childTransforms.Count - 1; indexTrans >= 0; indexTrans--)
        {
            RectTransform rectTrans = childTransforms[indexTrans];

            Vector2 anchorMin = rectTrans.anchorMin;
            Vector2 anchorMax = rectTrans.anchorMax;

            switch (_alignHorizontally)
            {
                case AlignHorizontally.Left:
                    anchorMin.x = 0;
                    anchorMax.x = 0;
                    rectTrans.anchoredPosition = new Vector2(_width / 2, 0);
                    break;
                case AlignHorizontally.Center:
                    anchorMin.x = .5f;
                    anchorMax.x = .5f;
                    rectTrans.anchoredPosition = new Vector2(0, 0);
                    break;
                case AlignHorizontally.Right:
                    anchorMin.x = 1;
                    anchorMax.x = 1;
                    rectTrans.anchoredPosition = new Vector2(-_width / 2, 0);
                    break;
            }

            anchorMin.y = currFraction;
            rectTrans.anchorMin = anchorMin;

            currFraction += fraction;

            anchorMax.y = currFraction;
            rectTrans.anchorMax = anchorMax;

            rectTrans.sizeDelta = new Vector2(_width, 0);

            if (_rowSpacing > 0) // Apply Spacing
            {
                var sizeDelta = rectTrans.sizeDelta;
                sizeDelta.y -= reducedHeight;
                rectTrans.sizeDelta = sizeDelta;

                var anchoredPosition = rectTrans.anchoredPosition;

                if (indexTrans == childCount - 1) // First child
                {
                    anchoredPosition.y -= reducedHeight * 0.5f;
                    spacingPrevious = -anchoredPosition.y + reducedHeight * 0.5f;
                }
                else
                {
                    anchoredPosition.y += -reducedHeight * 0.5f + _rowSpacing - spacingPrevious;
                    spacingPrevious = -anchoredPosition.y + reducedHeight * 0.5f;
                }

                rectTrans.anchoredPosition = anchoredPosition;
            }
        }
    }

    void MakeGridColumns()
    {
        if (_itemCount <= 0)
        {
            Debug.LogWarning("You must at least have 1 element per column");
            return;
        }

        GetChildTransforms(out int childCount);

        int fullRows = childCount / _itemCount;
        int rest = childCount % _itemCount;

        float fractionWidth = 1f / _itemCount;
        float fractionHeight = 1f / (fullRows + rest);

        float currFractionWidth = 0;
        float currFractionHeight = 1;
        int counter = 0;

        int rows = fullRows + (rest > 0 ? 1 : 0);

        Vector2 reducedSize;
        reducedSize.x = (_gridSpacing * (_itemCount - 1)) / _itemCount;
        reducedSize.y = (_gridSpacing * (rows - 1)) / rows;

        for (int indexTrans = 0; indexTrans <= childCount - 1; indexTrans++)
        {
            RectTransform rectTrans = childTransforms[indexTrans];

            rectTrans.anchoredPosition = Vector2.zero;

            counter++;

            if (counter > _itemCount)
            {
                currFractionHeight -= fractionHeight;
                currFractionWidth = 0;
                counter = 1;
            }

            Vector2 anchorMin = rectTrans.anchorMin;
            Vector2 anchorMax = rectTrans.anchorMax;

            anchorMin.x = currFractionWidth;
            anchorMin.y = currFractionHeight - fractionHeight;
            rectTrans.anchorMin = anchorMin;

            currFractionWidth += fractionWidth;

            anchorMax.x = currFractionWidth;
            anchorMax.y = currFractionHeight;
            rectTrans.anchorMax = anchorMax;

            rectTrans.sizeDelta = Vector2.zero;

            if (_gridSpacing > 0) // Apply spacing
            {
                Vector2 sizeDelta = rectTrans.sizeDelta;
                sizeDelta -= reducedSize;
                rectTrans.sizeDelta = sizeDelta;

                Vector2 anchoredPosition = rectTrans.anchoredPosition;

                if (indexTrans < _itemCount) // First child of each column
                {
                    anchoredPosition.y += reducedSize.y * 0.5f;
                }
                else
                {
                    // Get spacing of previous of this column
                    float spacingPrevious = childTransforms[indexTrans - _itemCount].anchoredPosition.y + reducedSize.y * 0.5f;

                    anchoredPosition.y -= -reducedSize.y * 0.5f + _gridSpacing - spacingPrevious;
                }

                if (indexTrans % _itemCount == 0) // First child of each row
                {
                    anchoredPosition.x -= reducedSize.x * 0.5f;
                }
                else
                {
                    // Get spacing of previous of this row
                    float spacingPrevious = -childTransforms[indexTrans - 1].anchoredPosition.x + reducedSize.x * 0.5f;

                    anchoredPosition.x += -reducedSize.x * 0.5f + _gridSpacing - spacingPrevious;
                }

                rectTrans.anchoredPosition = anchoredPosition;
            }
        }
    }

    void MakeGrid()
    {
        switch (_buildBy)
        {
            case BuildType.Rows:
                MakeGridRows();
                break;
            case BuildType.Columns:
                MakeGridColumns();
                break;
        }
    }

    void MakeGridRows()
    {
        if (_itemCount <= 0)
        {
            Debug.LogWarning("You must at least have 1 element per row");
            return;
        }

        GetChildTransforms(out int childCount);

        int lines = childCount / _itemCount;
        int rest = childCount % _itemCount;

        float fractionHeight = 1f / _itemCount;
        float fractionWidth = 1f / (lines + rest);

        float currFractionWidth = 0;
        float currFractionHeight = 1;
        int counter = 0;

        foreach (var rectTrans in childTransforms)
        {
            counter++;

            rectTrans.anchoredPosition = Vector2.zero;

            if (counter > _itemCount)
            {
                currFractionWidth += fractionWidth;
                currFractionHeight = 1;
                counter = 1;
            }

            Vector2 anchorMin = rectTrans.anchorMin;
            Vector2 anchorMax = rectTrans.anchorMax;

            anchorMax.x = currFractionWidth + fractionWidth;
            anchorMax.y = currFractionHeight;
            rectTrans.anchorMax = anchorMax;

            currFractionHeight -= fractionHeight;

            anchorMin.x = currFractionWidth;
            anchorMin.y = currFractionHeight;
            rectTrans.anchorMin = anchorMin;

            rectTrans.sizeDelta = Vector2.zero;
        }
    }

    //[ButtonGroup("Saves")]
    //[Tooltip("Saves positions and sizes. Won't recover destroyed GameObjects!")]
    //void Save()
    //{
    //    savedAnchorsMax.Clear();
    //    savedAnchorsMin.Clear();
    //    savedSizes.Clear();
    //    savedPositions.Clear();

    //    int childCount = transform.childCount;

    //    if (childCount > savedSizes.Capacity)
    //    {
    //        savedAnchorsMin.Capacity = childCount;
    //        savedAnchorsMax.Capacity = childCount;
    //        savedSizes.Capacity = childCount;
    //        savedPositions.Capacity = childCount;
    //    }

    //    for (int i = 0; i < childCount; i++)
    //    {
    //        savedAnchorsMin.Add(transform.GetChild(i).GetComponent<RectTransform>().anchorMin);
    //        savedAnchorsMax.Add(transform.GetChild(i).GetComponent<RectTransform>().anchorMax);
    //        savedSizes.Add(transform.GetChild(i).GetComponent<RectTransform>().sizeDelta);
    //        savedPositions.Add(transform.GetChild(i).GetComponent<RectTransform>().position);
    //    }
    //}

    //[ButtonGroup("Saves")]
    //void Load()
    //{
    //    if (savedAnchorsMax.Count == 0)
    //    {
    //        Debug.LogWarning("No saves found");
    //        return;
    //    }

    //    int childCount = transform.childCount;

    //    for (int i = 0; i < childCount; i++)
    //    {
    //        transform.GetChild(i).GetComponent<RectTransform>().anchorMin = savedAnchorsMin[i];
    //        transform.GetChild(i).GetComponent<RectTransform>().anchorMax = savedAnchorsMax[i];
    //        transform.GetChild(i).GetComponent<RectTransform>().sizeDelta = savedSizes[i];
    //        transform.GetChild(i).GetComponent<RectTransform>().position = savedPositions[i];

    //        if (i == savedSizes.Count - 1) break;
    //    }
    //}

    void GetChildTransforms(out int childCount)
    {
        childTransforms.Clear();

        childCount = transform.childCount;

        childTransforms.Capacity = childCount;

        for (int i = 0; i < childCount; i++)
        {
            childTransforms.Add(transform.GetChild(i).GetComponent<RectTransform>());
        }
    }
}
