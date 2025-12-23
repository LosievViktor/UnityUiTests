namespace UnityEngine.UIElements.TestFramework
{
    /// <summary>
    /// Provides utility functions related to `VisualElement`.
    /// </summary>
    public static class VisualElementUtility
    {
        /// <summary>
        /// Checks if this element or any of its descendants has mouse capture.
        /// </summary>
        /// <param name="currentElement">`VisualElement` to check.</param>
        /// <returns>`True` if the element has captured the mouse; `false` otherwise.</returns>
        public static bool ContainsMouseCapture(VisualElement currentElement)
        {
            if (currentElement.HasMouseCapture())
                return true;

            if (currentElement.hierarchy.childCount > 0)
            {
                int childCount = currentElement.hierarchy.childCount;

                for (int i = 0; i < childCount; ++i)
                {
                    if (ContainsMouseCapture(currentElement.hierarchy[i]))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the `EventHandler` for the <paramref name="currentElement"/>.
        /// </summary>
        /// <param name="currentElement">The `VisualElement` to get the `EventHandler` from.</param>
        /// <returns>The `EventHandler` for the element.</returns>
        public static IEventHandler GetMouseCaptureHandler(VisualElement currentElement)
        {
            IEventHandler handler = currentElement;
            var hasMouseCapture = currentElement.HasMouseCapture();
            int childCount = currentElement.hierarchy.childCount;
            if (!hasMouseCapture && (childCount != 0))
            {
                handler = null;

                for (var i = 0; i < childCount; ++i)
                {
                    handler = GetMouseCaptureHandler(currentElement.hierarchy[i]);

                    if (handler != null)
                    {
                        break;
                    }
                }
            }
            return handler;
        }
    }
}
