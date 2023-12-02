namespace Darkan.UI
{
    using Sirenix.OdinInspector;
    using System.Collections;
    using UnityEngine;
    using UnityEngine.UIElements;

    [ExecuteAlways]
    public abstract class CanvasBase : SerializedMonoBehaviour
    {
        [OnValueChanged("SetStyleSheet")]
        [SerializeField]
        StyleSheet _styleSheet;
        void SetStyleSheet() => _root.styleSheets.Add(_styleSheet);


        UIDocument _uiDocument;
        VisualElement _root;

        [OnValueChanged("SetVisible")]
        [SerializeField]
        bool _visible = true;
        void SetVisible() => Visible = _visible;

        public bool Visible
        {
            get => _root.visible;
            set
            {
                _root.visible = value;
                _visible = value;
            }
        }

        public UIDocument UIDocument => _uiDocument;
        public VisualElement Root => _root;

        void OnEnable()
        {
            _uiDocument = GetComponent<UIDocument>();
            _root = _uiDocument.rootVisualElement;

            if (_styleSheet != null)
                _root.styleSheets.Add(_styleSheet);

            StartCoroutine(BuildCanvas());

            Visible = _visible;
        }

        void OnDisable()
        {
            if (Application.isPlaying)
                _uiDocument.rootVisualElement.Clear();
        }

        protected abstract IEnumerator BuildCanvas();

        protected VisualElement Create(params string[] classNames)
        {
            return Create<VisualElement>(null, classNames);
        }

        protected VisualElement Create(VisualElement parent, params string[] classNames)
        {
            return Create<VisualElement>(parent, classNames);
        }

        protected VisualElement Create(string className, VisualElement parent = null)
        {
            return Create<VisualElement>(parent, className);
        }

        protected T Create<T>(VisualElement parent, params string[] classNames) where T : VisualElement, new()
        {
            T element = new();

            if (parent != null)
                parent.Add(element);

            foreach (string className in classNames)
            {
                element.AddToClassList(className);
            }

            return element;
        }

        protected T Create<T>(params string[] classNames) where T : VisualElement, new()
        {
            T element = new();

            foreach (string className in classNames)
            {
                element.AddToClassList(className);
            }

            return element;
        }
    }
}
