namespace Darkan.UI
{
    using Sirenix.OdinInspector;
    using System.Collections;
    using UnityEngine;
    using UnityEngine.UIElements;

    [ExecuteAlways]
    public abstract class CanvasBase : SerializedMonoBehaviour
    {
        [SerializeField] UIDocument _uiDocument;

        [OnValueChanged("SetStyleSheet")]
        [SerializeField]
        StyleSheet _styleSheet;
        void SetStyleSheet() => _root.styleSheets.Add(_styleSheet);

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
            if (_uiDocument == null)
                _uiDocument = GetComponent<UIDocument>();

            _root = _uiDocument.rootVisualElement;

            if (_root != null)
            {
                _root.Clear();
                _root.styleSheets.Clear();
            }

            if (_styleSheet != null)
                _root.styleSheets.Add(_styleSheet);

            StartCoroutine(BuildCanvas());

            Visible = _visible;
        }

        void OnDisable()
        {
            if (_root != null)
            {
                _root.Clear();
                _root.styleSheets.Clear();
                _root.ReleaseMouse();
                _root.ReleasePointer(0);
            }
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
