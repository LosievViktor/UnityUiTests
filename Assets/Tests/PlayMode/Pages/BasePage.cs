using UnityEngine.UIElements;

namespace Tests.Pages
{
    public abstract class BasePage
    {
        protected readonly VisualElement Root;

        protected BasePage(UIDocument document)
        {
            Root = document.rootVisualElement;
        }

        protected T Q<T>(string name) where T : VisualElement
        {
            return Root.Q<T>(name);
        }
    }
}