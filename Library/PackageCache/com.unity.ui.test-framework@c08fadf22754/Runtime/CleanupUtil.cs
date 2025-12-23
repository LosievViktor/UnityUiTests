using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements.TestFramework
{
    /// <summary>
    /// Tracks and cleans up objects that require disposal or destruction after tests.
    /// When added to a test fixture, `CleanupUtil` automatically cleans up all registered 
    /// objects after the test execution.
    /// </summary>
    /// <remarks>
    /// For an example usage, refer to [Clean up objects after tests](xref:clean-up-objects-after-tests).
    /// </remarks>
    public class CleanupUtil : UITestComponent
    {
        List<IDisposable> m_ObjectsToDispose = new();
        List<Object> m_ObjectsToDestroy = new();

        /// <summary>
        /// Adds an object for disposal after the test.
        /// </summary>
        /// <param name="disposable">The object for disposal.</param>
        public void AddDisposable(IDisposable disposable)
        {
            m_ObjectsToDispose.Add(disposable);
        }

        /// <summary>
        /// Adds an object for destruction after the test.
        /// </summary>
        /// <param name="destructible">
        /// The `UnityEngine.Object` to destroy. This can be a GameObject, ScriptableObject, or any other object.
        /// </param>
        public void AddDestructible(UnityEngine.Object destructible)
        {
            m_ObjectsToDestroy.Add(destructible);
        }

        /// <summary>
        /// Cleans up all objects that you added for disposal or destruction.
        /// </summary>
        public void Cleanup()
        {
            for (int i = 0; i < m_ObjectsToDispose.Count; ++i)
            {
                IDisposable disposable = m_ObjectsToDispose[i];
                if (disposable != null)
                    disposable.Dispose();
            }

            m_ObjectsToDispose.Clear();

            for (int i = 0; i < m_ObjectsToDestroy.Count; ++i)
            {
                Object obj = m_ObjectsToDestroy[i];
                if (obj != null && obj)
                {
#if UNITY_EDITOR
                    (obj as UnityEditor.EditorWindow)?.Close();
#endif
                    Object.DestroyImmediate(obj);
                }
            }
            m_ObjectsToDestroy.Clear();
        }

        /// <summary>
        /// Cleans up `CleanupUtil` after each test.
        /// </summary>
        protected override void AfterTest()
        {
            base.AfterTest();
            Cleanup();
        }
    }
}
