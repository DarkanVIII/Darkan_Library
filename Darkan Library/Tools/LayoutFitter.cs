using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class LayoutFitter : MonoBehaviour
{
    public enum AlignVertically { Top, Center, Bottom };
    public enum AlignHorizontally { Left, Center, Right };


    [Title("Make Grid", "Set amount of rows or columns", TitleAlignment = TitleAlignments.Split)]

    [InlineButton("MakeGridColumns", "Apply")]
    [MinValue(1)]
    [SerializeField]
    int columns = 1;
    [InlineButton("MakeGridRows", "Apply")]
    [MinValue(1)]
    [SerializeField]
    int rows = 1;


    [Title("Fit in single row", "Stretches and aligns all children in a row horizontally or vertically",
        TitleAlignment = TitleAlignments.Split)]

    [Tooltip("Set size for the direction that is not getting stretched")]
    [SerializeField]
    float fitSize;

    [InlineButton("FitVertically", "Apply")]
    [LabelText("Fit Vertically")]
    [SerializeField]
    AlignHorizontally alignHorizontally = AlignHorizontally.Center;

    [InlineButton("FitHorizontally", "Apply")]
    [LabelText("Fit Horizontally")]
    [SerializeField]
    AlignVertically alignVertically = AlignVertically.Center;


    readonly List<RectTransform> rectTransforms = new();
    readonly List<Vector2> savedAnchorsMin = new();
    readonly List<Vector2> savedAnchorsMax = new();
    readonly List<Vector2> savedSizes = new();
    readonly List<Vector2> savedPositions = new();

    new Transform transform;

    void OnValidate()
    {
        transform = GetComponent<Transform>();
    }

    void FitHorizontally()
    {
        GetChildTransforms(out int childCount);

        float fraction = 1f / childCount;
        float currFraction = 0;

        foreach (var rectTrans in rectTransforms)
        {
            Vector2 anchorMin = rectTrans.anchorMin;
            Vector2 anchorMax = rectTrans.anchorMax;

            switch (alignVertically)
            {
                case AlignVertically.Top:
                    anchorMin.y = 1;
                    anchorMax.y = 1;
                    rectTrans.anchoredPosition = new Vector2(0, -fitSize / 2);
                    break;
                case AlignVertically.Center:
                    anchorMin.y = .5f;
                    anchorMax.y = .5f;
                    rectTrans.anchoredPosition = new Vector2(0, 0);
                    break;
                case AlignVertically.Bottom:
                    anchorMin.y = 0;
                    anchorMax.y = 0;
                    rectTrans.anchoredPosition = new Vector2(0, fitSize / 2);
                    break;
            }

            anchorMin.x = currFraction;
            rectTrans.anchorMin = anchorMin;

            currFraction += fraction;

            anchorMax.x = currFraction;
            rectTrans.anchorMax = anchorMax;

            rectTrans.sizeDelta = new Vector2(0, fitSize);
        }
    }

    void FitVertically()
    {
        GetChildTransforms(out int childCount);

        float fraction = 1f / childCount;
        float currFraction = 0;

        foreach (var rectTrans in rectTransforms)
        {
            Vector2 anchorMin = rectTrans.anchorMin;
            Vector2 anchorMax = rectTrans.anchorMax;

            switch (alignHorizontally)
            {
                case AlignHorizontally.Left:
                    anchorMin.x = 0;
                    anchorMax.x = 0;
                    rectTrans.anchoredPosition = new Vector2(fitSize / 2, 0);
                    break;
                case AlignHorizontally.Center:
                    anchorMin.x = .5f;
                    anchorMax.x = .5f;
                    rectTrans.anchoredPosition = new Vector2(0, 0);
                    break;
                case AlignHorizontally.Right:
                    anchorMin.x = 1;
                    anchorMax.x = 1;
                    rectTrans.anchoredPosition = new Vector2(-fitSize / 2, 0);
                    break;
            }

            anchorMin.y = currFraction;
            rectTrans.anchorMin = anchorMin;

            currFraction += fraction;

            anchorMax.y = currFraction;
            rectTrans.anchorMax = anchorMax;

            rectTrans.sizeDelta = new Vector2(fitSize, 0);
        }
    }

    void MakeGridColumns()
    {
        if (columns <= 0)
        {
            Debug.LogWarning("You must at least have 1 element per column");
            return;
        }

        GetChildTransforms(out int childCount);

        int lines = childCount / columns;
        int rest = childCount % columns;

        float fractionWidth = 1f / columns;
        float fractionHeight = 1f / (lines + rest);

        float currFractionWidth = 0;
        float currFractionHeight = 1;
        int counter = 0;

        foreach (var rectTrans in rectTransforms)
        {
            counter++;

            if (counter > columns)
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
        }
    }

    void MakeGridRows()
    {
        if (rows <= 0)
        {
            Debug.LogWarning("You must at least have 1 element per row");
            return;
        }

        GetChildTransforms(out int childCount);

        int lines = childCount / rows;
        int rest = childCount % rows;

        float fractionHeight = 1f / rows;
        float fractionWidth = 1f / (lines + rest);

        float currFractionWidth = 0;
        float currFractionHeight = 1;
        int counter = 0;

        foreach (var rectTrans in rectTransforms)
        {
            counter++;

            if (counter > rows)
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

    [ButtonGroup("Saves")]
    [Tooltip("Saves positions and sizes. Won't recover destroyed GameObjects!")]
    void Save()
    {
        savedAnchorsMax.Clear();
        savedAnchorsMin.Clear();
        savedSizes.Clear();
        savedPositions.Clear();

        int childCount = transform.childCount;

        if (childCount > savedSizes.Capacity)
        {
            savedAnchorsMin.Capacity = childCount;
            savedAnchorsMax.Capacity = childCount;
            savedSizes.Capacity = childCount;
            savedPositions.Capacity = childCount;
        }

        for (int i = 0; i < childCount; i++)
        {
            savedAnchorsMin.Add(transform.GetChild(i).GetComponent<RectTransform>().anchorMin);
            savedAnchorsMax.Add(transform.GetChild(i).GetComponent<RectTransform>().anchorMax);
            savedSizes.Add(transform.GetChild(i).GetComponent<RectTransform>().sizeDelta);
            savedPositions.Add(transform.GetChild(i).GetComponent<RectTransform>().position);
        }
    }

    [ButtonGroup("Saves")]
    void Load()
    {
        if (savedAnchorsMax.Count == 0)
        {
            Debug.LogWarning("No saves found");
            return;
        }

        int childCount = transform.childCount;

        for (int i = 0; i < childCount; i++)
        {
            transform.GetChild(i).GetComponent<RectTransform>().anchorMin = savedAnchorsMin[i];
            transform.GetChild(i).GetComponent<RectTransform>().anchorMax = savedAnchorsMax[i];
            transform.GetChild(i).GetComponent<RectTransform>().sizeDelta = savedSizes[i];
            transform.GetChild(i).GetComponent<RectTransform>().position = savedPositions[i];

            if (i == savedSizes.Count - 1) break;
        }
    }
    void GetChildTransforms(out int childCount)
    {
        rectTransforms.Clear();

        childCount = transform.childCount;

        rectTransforms.Capacity = childCount;

        for (int i = 0; i < childCount; i++)
        {
            rectTransforms.Add(transform.GetChild(i).GetComponent<RectTransform>());
        }
    }
}
