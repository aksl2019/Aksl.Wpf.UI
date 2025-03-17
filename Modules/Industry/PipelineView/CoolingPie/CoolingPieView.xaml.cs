using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Aksl.Modules.Pipeline.Views
{
    public partial class CoolingPieView : UserControl
    {
        public CoolingPieView()
        {
            InitializeComponent();
        }

        #region FanStatus Property

        public static readonly DependencyProperty FanStatusProperty =
                                                  DependencyProperty.Register("FanStatus", typeof(FanStatus), typeof(CoolingPieView), new PropertyMetadata(defaultValue: FanStatus.Pause, propertyChangedCallback: OnFanStatusChanged));
        public FanStatus FanStatus
        {
            get { return (FanStatus)GetValue(FanStatusProperty); }
            set { SetValue(FanStatusProperty, value); }
        }

        private static void OnFanStatusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CoolingPieView coolingPieView)
            {
                if (e.NewValue is int newValue && e.OldValue is int oldValue)
                {
                    if (newValue != oldValue)
                    {
                    }
                }

                if (coolingPieView.DataContext is ViewModels.CoolingPieViewModel coolingPieViewModel)
                {

                }
            }
        }
        #endregion

        #region CenterX Property

        public static readonly DependencyProperty CenterXProperty =
                                                  DependencyProperty.Register("CenterX", typeof(int), typeof(CoolingPieView), new PropertyMetadata(defaultValue: 0, propertyChangedCallback: OnCenterChanged));
        public int CenterX
        {
            get { return (int)GetValue(CenterXProperty); }
            set { SetValue(CenterXProperty, value); }
        }

        private static void OnCenterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CoolingPieView coolingPieView)
            {
                if ((int)e.NewValue != (int)e.OldValue)
                {
                }

                if (coolingPieView.DataContext is ViewModels.CoolingPieViewModel coolingPieViewModel)
                {

                }
            }
        }
        #endregion

        #region CenterY Property

        public static readonly DependencyProperty CenterYProperty =
                                                  DependencyProperty.Register("CenterY", typeof(int), typeof(CoolingPieView), new PropertyMetadata(defaultValue: 0, propertyChangedCallback: OnCenterChanged));
        public int CenterY
        {
            get { return (int)GetValue(CenterYProperty); }
            set { SetValue(CenterYProperty, value); }
        }
        #endregion

    }
}
