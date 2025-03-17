using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Aksl.Modules.IndustryNavigationBar.Views
{
    public partial class MenuItemHeader : UserControl
    {
        public MenuItemHeader()
        {
            InitializeComponent();
        }

        #region Properties
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly DependencyProperty TitleProperty
             = DependencyProperty.Register(nameof(Title), typeof(string), typeof(MenuItemHeader), new PropertyMetadata(OnTitlePropertyChanged));

        private static void OnTitlePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is string value)
            {
                if (d is FrameworkElement element)
                {
                    var headerTextBlock = (TextBlock)element.FindName("headerTextBlock");
                    if (headerTextBlock != null)
                    {
                        headerTextBlock.Text = value;
                    }
                }
            };
        }
        #endregion
    }
}
