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
    }
}
