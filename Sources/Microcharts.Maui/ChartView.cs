using System;
using System.ComponentModel;

namespace Microcharts.Maui
{
    using Microsoft.Maui.Controls;
    using Microsoft.Maui.Graphics;
    using SkiaSharp;
    using SkiaSharp.Views.Maui;
    using SkiaSharp.Views.Maui.Controls;

    public class ChartView : SKCanvasView
    {
        #region Constructors

        public ChartView()
        {
            this.BackgroundColor = Colors.Transparent;
            this.PaintSurface += OnPaintCanvas;
            this.EnableTouchEvents = true;
        }

        #endregion

        #region Static fields

        public static readonly BindableProperty ChartProperty = BindableProperty.Create(nameof(Chart), typeof(Chart), typeof(ChartView), null, propertyChanged: OnChartChanged);

        public static readonly BindableProperty SelectedEntryProperty = BindableProperty.Create(nameof(SelectedEntry), typeof(ChartEntry), typeof(ChartView), null, propertyChanged: OnSelectedEntryChanged, defaultBindingMode: BindingMode.TwoWay);

        #endregion

        #region Fields

        private InvalidatedWeakEventHandler<ChartView> handler;

        private Chart chart;

        #endregion

        #region Properties

        public Chart Chart
        {
            get { return (Chart)GetValue(ChartProperty); }
            set { SetValue(ChartProperty, value); }
        }

        public ChartEntry SelectedEntry
        {
            get { return (ChartEntry)GetValue(SelectedEntryProperty); }
            set { SetValue(SelectedEntryProperty, value); }
        }

        protected SKPoint TouchedPoint { get; set; }

        #endregion

        #region Methods

        private static void OnSelectedEntryChanged(BindableObject d, object oldValue, object value)
        {
            var view = d as ChartView;
            if(view?.Chart != null)
            {
                view.Chart.SelectedEntry = value as ChartEntry;
            }
        }

        private static void OnChartChanged(BindableObject d, object oldValue, object value)
        {
            var view = d as ChartView;

            if (view.chart != null)
            {
                view.handler.Dispose();
                view.handler = null;
                view.chart.PropertyChanged -= view.ChartOnPropertyChanged;
            }

            view.chart = value as Chart;

            if(view.chart != null)
                view.Chart.SelectedEntry = view.SelectedEntry;

            view.InvalidateSurface();

            if (view.chart != null)
            {
                view.handler = view.chart.ObserveInvalidate(view, (v) => v.InvalidateSurface());
                view.chart.PropertyChanged += view.ChartOnPropertyChanged;
            }
        }

        private void ChartOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Microcharts.Chart.SelectedEntry))
            {
                SelectedEntry = chart?.SelectedEntry;
            }
        }

        private void OnPaintCanvas(object sender, SKPaintSurfaceEventArgs e)
        {
            if (this.chart != null)
            {
                this.chart.Draw(e.Surface.Canvas, e.Info.Width, e.Info.Height);
            }
            else
            {
                e.Surface.Canvas.Clear(SKColors.Transparent);
            }
        }

        #endregion

        protected override void OnTouch(SKTouchEventArgs e)
        {
            base.OnTouch(e);
            if (this.chart != null && e.ActionType == SKTouchAction.Pressed)
            {
                chart.SelectClosest(e.Location);
            }
        }
    }
}
