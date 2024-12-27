using UnityEngine.UIElements;

namespace Darkan.Utilities
{
    public static class UIToolkitExtensions
    {
        public static T AddClasses<T>(this T element, params string[] classNames) where T : VisualElement
        {
            foreach (var className in classNames)
                element.AddToClassList(className);

            return element;
        }

        public static T AttachTo<T>(this T element, VisualElement parent) where T : VisualElement
        {
            parent.Add(element);
            return element;
        }
    }
}