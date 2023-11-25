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
        }

        void Start()
        {
            StartCoroutine(Generate());
        }

        void OnValidate()
        {
            if (Application.isPlaying) return;

            Root.Clear();

            StartCoroutine(Generate());
        }

        protected abstract IEnumerator Generate();
    }
}
