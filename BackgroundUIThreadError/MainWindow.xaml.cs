using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;

namespace BackgroundUIThreadError
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly List<Timer> decoratorTimers;
        private Random random;

        public MainWindow()
        {
            this.InitializeComponent();

            this.random = new Random(DateTime.Now.Millisecond * DateTime.Now.Hour);
            this.decoratorTimers = new List<Timer>();
            this.AddDecoratorTimers();
        }

        private void AddDecoratorTimers()
        {
            this.decoratorTimers.Add(new Timer(o =>
            {
                Application.Current.Dispatcher?.Invoke(
                    () =>
                    {
                        this.Decorator1.IsBusyIndicatorShowing = !this.Decorator1.IsBusyIndicatorShowing;
                    });
            },
            null,
            (int)(this.random.NextDouble() * 100) + 100,
            (int)(this.random.NextDouble() * 100) + 100));

            this.decoratorTimers.Add(new Timer(o =>
            {
                Application.Current.Dispatcher?.Invoke(
                    () =>
                    {
                        this.Decorator2.IsBusyIndicatorShowing = !this.Decorator2.IsBusyIndicatorShowing;
                    });
            },
                null,
                (int)(this.random.NextDouble() * 100) + 100,
                (int)(this.random.NextDouble() * 100) + 100));

            this.decoratorTimers.Add(new Timer(o =>
            {
                Application.Current.Dispatcher?.Invoke(
                    () =>
                    {
                        this.Decorator3.IsBusyIndicatorShowing = !this.Decorator3.IsBusyIndicatorShowing;
                    });
            },
                null,
                (int)(this.random.NextDouble() * 100) + 100,
                (int)(this.random.NextDouble() * 100) + 100));

            this.decoratorTimers.Add(new Timer(o =>
            {
                Application.Current.Dispatcher?.Invoke(
                    () =>
                    {
                        this.Decorator4.IsBusyIndicatorShowing = !this.Decorator4.IsBusyIndicatorShowing;
                    });
            },
                null,
                (int)(this.random.NextDouble() * 100) + 100,
                (int)(this.random.NextDouble() * 100) + 100));

            this.decoratorTimers.Add(new Timer(o =>
            {
                Application.Current.Dispatcher?.Invoke(
                    () =>
                    {
                        this.Decorator5.IsBusyIndicatorShowing = !this.Decorator5.IsBusyIndicatorShowing;
                    });
            },
                null,
                (int)(this.random.NextDouble() * 100) + 100,
                (int)(this.random.NextDouble() * 100) + 100));

            this.decoratorTimers.Add(new Timer(o =>
            {
                Application.Current.Dispatcher?.Invoke(
                    () =>
                    {
                        this.Decorator6.IsBusyIndicatorShowing = !this.Decorator6.IsBusyIndicatorShowing;
                    });
            },
                null,
                (int)(this.random.NextDouble() * 100) + 100,
                (int)(this.random.NextDouble() * 100) + 100));
           
        }
    }
}
