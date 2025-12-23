namespace UnityEngine.UIElements.TestFramework
{
    /// <summary>
    /// A <see cref="PanelSimulator"/> for use with runtime tests.
    /// </summary>
    public sealed class RuntimePanelSimulator : PanelSimulator
    {
        /// <summary>
        /// Creates a `RuntimePanelSimulator` with
        /// a null `PanelSettings`.
        /// </summary>
        public RuntimePanelSimulator() : this(null) { }

        /// <summary>
        /// Creates a `RuntimePanelSimulator` with
        /// the provided <paramref name="panelSettings"/>.
        /// </summary>
        /// <param name="panelSettings">The `PanelSettings` object to set for the created panel.</param>
        public RuntimePanelSimulator(PanelSettings panelSettings)
            #pragma warning disable CS0618 // Disable warning on Internal usage
            : base()
            #pragma warning restore CS0618
        {
            if (panelSettings != null)
            {
                #pragma warning disable CS0618 // Disable warning on Internal usage
                AssignPanel(panelSettings);
                #pragma warning restore CS0618
            }
        }

        [System.Obsolete("For Internal Use Only.")]
        internal void AssignPanel(PanelSettings panelSettingsAsset)
        {
            if (panelSettingsAsset == null)
            {
                #pragma warning disable CS0618 // Disable warning on Internal usage
                SetPanel(null);
                #pragma warning restore CS0618
            }
            else
            {
                #pragma warning disable CS0618 // Disable warning on Internal usage
                SetPanel(panelSettingsAsset.panel);
                SetRootVisualElement(panelSettingsAsset.panel.visualTree);
                #pragma warning restore CS0618
            }
        }

        /// <inheritdoc cref="PanelSimulator.FrameUpdate(double)"/>
        public override void FrameUpdate(double time)
        {
            #pragma warning disable CS0618 // Disable warning on Internal usage
            EnsureFrameUpdateCalledDuringTest();
            DoFrameUpdate(time);
            #pragma warning restore CS0618
        }
    }
}
