namespace BackgroundUIThreadError
{
    using System;
    using System.Collections;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Media;


    public class BackgroundTextBlock : Decorator
    {
        /// <summary>Identifies the IsBusyIndicatorShowing dependency property.</summary>
        public static readonly DependencyProperty IsBusyIndicatorShowingProperty = DependencyProperty.Register(
            "IsBusyIndicatorShowing",
            typeof(bool),
            typeof(BackgroundTextBlock),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>Identifies the <see cref="BusyStyle" /> property.</summary>
        public static readonly DependencyProperty BusyStyleProperty =
            DependencyProperty.Register(
            "BusyStyle",
            typeof(Style),
            typeof(BackgroundTextBlock),
            new FrameworkPropertyMetadata(OnBusyStyleChanged));

        /// <summary>The busy host.</summary>
        private readonly BackgroundVisualHost busyHost = new BackgroundVisualHost();

        /////// <summary>The internal control.</summary>
        ////private readonly Control internalControl;

        /// <summary>Initializes static members of the <see cref="BackgroundTextBlock"/> class.</summary>
        static BackgroundTextBlock()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(BackgroundTextBlock),
                new FrameworkPropertyMetadata(typeof(BackgroundTextBlock)));
        }

        /// <summary>Initializes a new instance of the <see cref="BackgroundTextBlock"/> class.</summary>
        public BackgroundTextBlock()
        {
            this.AddLogicalChild(this.busyHost);
            this.AddVisualChild(this.busyHost);

            this.SetBinding(this.busyHost, IsBusyIndicatorShowingProperty, BackgroundVisualHost.IsContentShowingProperty);
            OnBusyStyleChanged(this, new DependencyPropertyChangedEventArgs());
            ////this.internalControl = new Control();
        }

        /// <summary>Gets or sets the Style to apply to the Control that is displayed as the busy indication.</summary>
        public Style BusyStyleAdd
        {
            get => (Style)this.GetValue(BusyStyleProperty);
            set => this.SetValue(BusyStyleProperty, value);
        }

        /// <summary>Gets or sets a value indicating whether if the BusyIndicator is being shown.</summary>
        public bool IsBusyIndicatorShowing
        {
            get => (bool)this.GetValue(IsBusyIndicatorShowingProperty);
            set => this.SetValue(IsBusyIndicatorShowingProperty, value);
        }

        /// <summary>Gets the logical children.</summary>
        protected override IEnumerator LogicalChildren
        {
            get
            {
                if (this.Child != null)
                {
                    yield return this.Child;
                }

                yield return this.busyHost;
            }
        }

        /// <summary>The visual children count.</summary>
        protected override int VisualChildrenCount => this.Child != null ? 2 : 1;

        /// <summary>The get visual child.</summary>
        /// <param name="index">The index.</param>
        /// <returns>The <see cref="Visual"/>.</returns>
        protected override Visual GetVisualChild(int index)
        {
            if (this.Child != null)
            {
                switch (index)
                {
                    case 0:
                        return this.Child;

                    case 1:
                        return this.busyHost;
                }
            }
            else if (index == 0)
            {
                return this.busyHost;
            }

            throw new IndexOutOfRangeException("index");
        }

        /// <summary>The measure override.</summary>
        /// <param name="constraint">The constraint.</param>
        /// <returns>The <see cref="Size"/>.</returns>
        protected override Size MeasureOverride(Size constraint)
        {
            var ret = new Size(0, 0);
            if (this.Child != null)
            {
                this.Child.Measure(constraint);
                ret = this.Child.DesiredSize;
            }

            this.busyHost.Measure(constraint);

            return new Size(Math.Max(ret.Width, this.busyHost.DesiredSize.Width), Math.Max(ret.Height, this.busyHost.DesiredSize.Height));
        }

        /// <summary>The arrange override.</summary>
        /// <param name="arrangeSize">The arrange size.</param>
        /// <returns>The <see cref="Size"/>.</returns>
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            var ret = new Size(0, 0);
            if (this.Child != null)
            {
                this.Child.Arrange(new Rect(arrangeSize));
                ret = this.Child.RenderSize;
            }

            this.busyHost.Arrange(new Rect(arrangeSize));

            return new Size(Math.Max(ret.Width, this.busyHost.RenderSize.Width), Math.Max(ret.Height, this.busyHost.RenderSize.Height));
        }

        /// <summary>The on busy style changed.</summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The e.</param>
        private static void OnBusyStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var bd = (BackgroundTextBlock)d;
            var style = (Style)e.NewValue;
            //style = new Style(typeof(Border));
            //style.Setters.Add(new Setter(Border.BackgroundProperty, new SolidColorBrush(Colors.Red)));
            bd.busyHost.CreateContent = () => new TextBlock { Text = "Hello" };
        }

        /// <summary>The set binding.</summary>
        /// <param name="obj">The obj.</param>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        private void SetBinding(DependencyObject obj, DependencyProperty source, DependencyProperty target)
        {
            var b = new Binding
            {
                Source = this,
                Path = new PropertyPath(source)
            };

            BindingOperations.SetBinding(obj, target, b);
        }
    }
}
