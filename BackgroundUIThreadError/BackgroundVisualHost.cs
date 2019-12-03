// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BackgroundVisualHost.cs" company="Vision RT Ltd.">
//   Copyright (c) Vision RT Ltd. All rights reserved.
// </copyright>
// <summary>
//   Defines the CreateContentFunction type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BackgroundUIThreadError
{
    using System;
    using System.Diagnostics;
    using System.Runtime.ExceptionServices;
    using System.Security;
    using System.Threading;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Threading;

    /// <summary>The create content function.</summary>
    /// <returns>The <see cref="Visual"/>.</returns>
    public delegate Visual CreateContentFunction();

    /// <summary>The background visual host.</summary>
    public class BackgroundVisualHost : FrameworkElement
    {
        /// <summary>
        /// Identifies the IsContentShowing dependency property.
        /// </summary>
        public static readonly DependencyProperty IsContentShowingProperty = DependencyProperty.Register(
            "IsContentShowing",
            typeof(bool),
            typeof(BackgroundVisualHost),
            new FrameworkPropertyMetadata(false, OnIsContentShowingChanged));

        /// <summary>
        /// Identifies the CreateContent dependency property.
        /// </summary>
        public static readonly DependencyProperty CreateContentProperty = DependencyProperty.Register(
            "CreateContent",
            typeof(CreateContentFunction),
            typeof(BackgroundVisualHost),
            new FrameworkPropertyMetadata(OnCreateContentChanged));

        /// <summary>The threaded helper.</summary>
        private ThreadedVisualHelper threadedHelper;

        /// <summary>The host visual.</summary>
        private HostVisual hostVisual;

        /// <summary>Gets or sets a value indicating whether is content showing.</summary>
        public bool IsContentShowing
        {
            get => (bool)this.GetValue(IsContentShowingProperty);
            set => this.SetValue(IsContentShowingProperty, value);
        }

        /// <summary>
        /// Gets or sets the function used to create the visual to display in a background thread.
        /// </summary>
        public CreateContentFunction CreateContent
        {
            get => (CreateContentFunction)this.GetValue(CreateContentProperty);
            set => this.SetValue(CreateContentProperty, value);
        }

        /// <summary>The visual children count.</summary>
        protected override int VisualChildrenCount => this.hostVisual != null ? 1 : 0;

        /// <summary>Gets the logical children.</summary>
        protected override System.Collections.IEnumerator LogicalChildren
        {
            get
            {
                if (this.hostVisual != null)
                {
                    yield return this.hostVisual;
                }
            }
        }

        /// <summary>The get visual child.</summary>
        /// <param name="index">The index.</param>
        /// <returns>The <see cref="Visual"/>.</returns>
        protected override Visual GetVisualChild(int index)
        {
            if (this.hostVisual != null && index == 0)
            {
                return this.hostVisual;
            }

            throw new IndexOutOfRangeException("index");
        }

        /// <summary>The measure override.</summary>
        /// <param name="availableSize">The available size.</param>
        /// <returns>The <see cref="Size"/>.</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            return this.threadedHelper?.DesiredSize ?? base.MeasureOverride(availableSize);
        }

        /// <summary>The on is content showing changed.</summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The e.</param>
        private static void OnIsContentShowingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var bvh = (BackgroundVisualHost)d;

            if (bvh.CreateContent != null)
            {
                if ((bool)e.NewValue)
                {
                    bvh.CreateContentHelper();
                }
                else
                {
                    bvh.HideContentHelper();
                }
            }
        }

        /// <summary>The on create content changed.</summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The e.</param>
        private static void OnCreateContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var bvh = (BackgroundVisualHost)d;

            if (bvh.IsContentShowing)
            {
                bvh.HideContentHelper();
                if (e.NewValue != null)
                {
                    bvh.CreateContentHelper();
                }
            }
        }

        /// <summary>The create content helper.</summary>
        private void CreateContentHelper()
        {
            this.threadedHelper = new ThreadedVisualHelper(this.CreateContent, this.SafeInvalidateMeasure);
            this.hostVisual = this.threadedHelper.HostVisual;
        }

        /// <summary>The safe invalidate measure.</summary>
        private void SafeInvalidateMeasure()
        {
            this.Dispatcher.BeginInvoke(new Action(this.InvalidateMeasure), DispatcherPriority.Loaded);
        }

        /// <summary>The hide content helper.</summary>
        private void HideContentHelper()
        {
            if (this.threadedHelper != null)
            {
                this.threadedHelper.Exit();
                this.threadedHelper = null;
                this.InvalidateMeasure();
            }
        }

        /// <summary>The threaded visual helper.</summary>
        private class ThreadedVisualHelper
        {
            /// <summary>The sync.</summary>
            private readonly AutoResetEvent sync = new AutoResetEvent(false);

            /// <summary>The create content.</summary>
            private readonly CreateContentFunction createContent;

            /// <summary>The invalidate measure.</summary>
            private readonly Action invalidateMeasure;

            /// <summary>Initializes a new instance of the <see cref="ThreadedVisualHelper"/> class.</summary>
            /// <param name="createContent">The create content.</param>
            /// <param name="invalidateMeasure">The invalidate measure.</param>
            public ThreadedVisualHelper(
                CreateContentFunction createContent,
                Action invalidateMeasure)
            {
                this.HostVisual = new HostVisual();
                this.createContent = createContent;
                this.invalidateMeasure = invalidateMeasure;

                var backgroundUi = new Thread(this.CreateAndShowContent);
                backgroundUi.SetApartmentState(ApartmentState.STA);
                backgroundUi.Name = "WaitSpinner BackgroundVisualHost Thread";
                backgroundUi.IsBackground = true;
                backgroundUi.Start();

                this.sync.WaitOne();
            }

            /// <summary>Gets the host visual.</summary>
            public HostVisual HostVisual { get; }

            /// <summary>Gets the desired size.</summary>
            public Size DesiredSize { get; private set; }

            /// <summary>Gets or sets the dispatcher.</summary>
            private Dispatcher Dispatcher { get; set; }

            /// <summary>The exit.</summary>
            public void Exit()
            {
                this.Dispatcher.BeginInvokeShutdown(DispatcherPriority.Send);
            }

            /// <summary>The create and show content.</summary>
            private void CreateAndShowContent()
            {
                this.Dispatcher = Dispatcher.CurrentDispatcher;
                var source = new VisualTargetPresentationSource(this.HostVisual);
                this.sync.Set();
                source.RootVisual = this.createContent();
                this.DesiredSize = source.DesiredSize;
                this.invalidateMeasure();

                Dispatcher.Run();
                source.Dispose();
            }
        }
    }
}
