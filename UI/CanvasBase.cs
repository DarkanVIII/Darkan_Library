namespace Darkan.UI
{
    using System.Collections;
    using UnityEngine;
    using UnityEngine.UIElements;

    public abstract class CanvasBase : MonoBehaviour
    {
        [SerializeField] StyleSheet _styleSheet;

        protected VisualElement Root;

        void Awake()
        {
            Root = GetComponent<UIDocument>().rootVisualElement;
            Root.styleSheets.Add(_styleSheet);

            StartCoroutine(Generate());

        }

        void OnValidate()
        {
            if (Application.isPlaying) return;

            Root.Clear();
            Root.styleSheets.Add(_styleSheet);

            StartCoroutine(Generate());
        }

        protected abstract IEnumerator Generate();
    }
}
