using UnityEngine.UIElements;

namespace Darkan.UI
{
    public static class UIExtensions
    {
        public static T AddClasses<T>(this T element, params string[] classNames) where T : VisualElement
        {
            foreach (var className in classNames)
            {
                element.AddToClassList(className);
            }

            return element;
        }

        public static T AttachTo<T>(this T element, VisualElement parent) where T : VisualElement
        {
            parent.Add(element);

            return element;
        }
    }
}
