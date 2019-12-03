// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VisualTargetPresentationSource.cs" company="Vision RT Ltd.">
//   Copyright (c) Vision RT Ltd. All rights reserved.
// </copyright>
// <summary>
//   Defines the VisualTargetPresentationSource type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BackgroundUIThreadError
{
    using System.Windows;
    using System.Windows.Media;

    /// <summary>The visual target presentation source.</summary>
    public class VisualTargetPresentationSource : PresentationSource
    {
        /// <summary>The visual target.</summary>
        private readonly VisualTarget visualTarget;

        /// <summary>The is disposed.</summary>
        private bool isDisposed;

        /// <summary>Initializes a new instance of the <see cref="VisualTargetPresentationSource"/> class.</summary>
        /// <param name="hostVisual">The host visual.</param>
        public VisualTargetPresentationSource(HostVisual hostVisual)
        {
            this.visualTarget = new VisualTarget(hostVisual);
            this.AddSource();
        }

        /// <summary>Gets the desired size.</summary>
        public Size DesiredSize { get; private set; }

        /// <summary>Gets a value indicating whether is disposed.</summary>
        public override bool IsDisposed => this.isDisposed;

        /// <summary>Gets or sets the root visual.</summary>
        public override Visual RootVisual
        {
            get => this.visualTarget.RootVisual;
            set
            {
                var oldRoot = this.visualTarget.RootVisual;

                // Set the root visual of the VisualTarget.  This visual will
                // now be used to visually compose the scene.
                this.visualTarget.RootVisual = value;

                // Tell the PresentationSource that the root visual has
                // changed.  This kicks off a bunch of stuff like the
                // Loaded event.
                this.RootChanged(oldRoot, value);

                // Kickoff layout...
                var rootElement = value as UIElement;
                if (rootElement != null)
                {
                    rootElement.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    rootElement.Arrange(new Rect(rootElement.DesiredSize));

                    this.DesiredSize = rootElement.DesiredSize;
                }
                else
                {
                    this.DesiredSize = new Size(0, 0);
                }
            }
        }

        /// <summary>The dispose.</summary>
        internal void Dispose()
        {
            this.RemoveSource();
            this.isDisposed = true;
        }

        /// <summary>The get composition target core.</summary>
        /// <returns>The <see cref="CompositionTarget"/>.</returns>
        protected override CompositionTarget GetCompositionTargetCore()
        {
            return this.visualTarget;
        }
    }
}
