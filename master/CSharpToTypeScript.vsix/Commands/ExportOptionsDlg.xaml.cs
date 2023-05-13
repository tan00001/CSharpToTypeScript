using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interop;
using Microsoft.VisualStudio.PlatformUI;

namespace CSharpToTypeScript.Commands
{
    /// <summary>
    /// Interaction logic for ExportOptionsDlg.xaml
    /// </summary>
    public partial class ExportOptionsDlg : DialogWindow
    {
        const int GWL_STYLE = -16;
        const int WS_MAXIMIZEBOX = 0x10000;
        const int WS_MINIMIZEBOX = 0x20000;
        const int WM_NCHITTEST = 0x0084;
        const int HTBORDER = 18;
        const int HTBOTTOM = 15;
        const int HTBOTTOMLEFT = 16;
        const int HTBOTTOMRIGHT = 17;
        const int HTLEFT = 10;
        const int HTRIGHT = 11;
        const int HTTOP = 12;
        const int HTTOPLEFT = 13;
        const int HTTOPRIGHT = 14;

        [DllImport("user32.dll")]
        public static extern Int32 GetWindowLongA(IntPtr hWnd, Int32 nIndex);

        [DllImport("user32.dll")]
        public static extern Int32 SetWindowLongA(IntPtr hWnd, Int32 nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        public static extern IntPtr DefWindowProcW(
            IntPtr hWnd,
            Int32 msg,
            IntPtr wParam,
            IntPtr lParam);

        public bool SetColCount { get; private set; }

        public IReadOnlyList<ComboBox> TypeParamCtrls { get; private set; }

        private static readonly List<DependencyProperty> TypeParamProperties = new();

        public IReadOnlyList<string> TypeParams { get; private set; } = new List<string>();

        private static readonly IReadOnlyList<string> _ParamOptions = new List<string>()
        {
            "System.String",
            "System.Int32",
            "System.Decimal",
            "System.Int16",
            "System.DateTimeOffset",
            "System.Boolean",
            "System.Int64",
            "System.DateTime",
            "System.Guid",
            "System.TimeSpan",
            "System.Double",
            "System.Single",
            "System.Char",
            "System.Byte",
            "System.SByte",
            "System.UInt32",
            "System.UInt64",
            "System.UInt16"
        };

        public static readonly DependencyProperty ColCountProperty =
            DependencyProperty.Register(nameof(ColCount), typeof(int), typeof(ExportOptionsDlg),
                new FrameworkPropertyMetadata(1, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public int ColCount
        {
            get { return (int)GetValue(ColCountProperty); }
            set { SetValue(ColCountProperty, value); }
        }


        public ExportOptionsDlg(bool showColumnCount, IReadOnlyList<string> paramNames)
        {
            InitializeComponent();

            if (!showColumnCount)
            {
                LayoutGrid.RowDefinitions[0].Height = new GridLength(0);
            }
            else
            {
                LayoutGrid.RowDefinitions[0].Height = GridLength.Auto;
                SetColCount = showColumnCount;
            }

            TypeParamCtrls = AddTypeParamSelections(paramNames);
        }

        private void OnOkClick(object sender, RoutedEventArgs e)
        {
            foreach (var item in TypeParamCtrls)
            {
                item.GetBindingExpression(ComboBox.TextProperty).UpdateSource();
            }

            if (Validation.GetErrors(ColCountCtrl).Count > 0
                || TypeParamCtrls.Any(tc => Validation.GetErrors(tc).Count > 0))
            {
                return;
            }

            TypeParams = TypeParamProperties.Select(tp => ((string)GetValue(tp)).Trim()).ToList();

            DialogResult = true;

            Close();
        }

        private IReadOnlyList<ComboBox> AddTypeParamSelections(IReadOnlyList<string> paramNames)
        {
            var typeParamCtrls = new List<ComboBox>(paramNames.Count);

            if (paramNames.Count <= 0)
            {
                return typeParamCtrls;
            }

            for (var i = 0; i < paramNames.Count; ++i)
            {
                LayoutGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            }
            Grid.SetRow(Buttons, paramNames.Count + 1);

            var typeParamValidationRule = new TypeParamValidationRule();

            for (var i = 0; i < paramNames.Count; ++i)
            {
                DependencyProperty typeParamProperty = GetTypeParamProperty("TypeParam" + i);
                var paramName = paramNames[i];
                var rowIndex = i + 1;

                ComboBox comboBox = CreateComboBox(typeParamCtrls, typeParamValidationRule, typeParamProperty, rowIndex);
                LayoutGrid.Children.Add(comboBox);

                Label label = CreateLabelForComboBox(paramName, rowIndex);
                LayoutGrid.Children.Add(label);
            }

            return typeParamCtrls;
        }

        private static Label CreateLabelForComboBox(string paramName, int rowIndex)
        {
            var label = new Label()
            {
                Content = "Type Param " + rowIndex + " (" + paramName + "):",
                HorizontalContentAlignment = HorizontalAlignment.Right
            };

            Grid.SetColumn(label, 0);
            Grid.SetRow(label, rowIndex);

            return label;
        }

        private ComboBox CreateComboBox(List<ComboBox> typeParamCtrls, TypeParamValidationRule typeParamValidationRule,
            DependencyProperty typeParamProperty, int rowIndex)
        {
            var comboBox = new ComboBox()
            {
                Name = typeParamProperty.Name + "Ctrl",
                Margin = new Thickness(0, 0, 0, 10),
                Padding = new Thickness(3),
                IsEditable = true,
                Style = (Style)this.FindResource("ComboBoxStyle"),
            };

            foreach (var paramOption in _ParamOptions)
            {
                comboBox.Items.Add(paramOption);
            }
            comboBox.SelectedIndex = 0;

            Binding binding = new(typeParamProperty.Name)
            {
                Source = this,
                UpdateSourceTrigger = UpdateSourceTrigger.Explicit,
                ValidatesOnDataErrors = true,
                Mode = BindingMode.TwoWay,
            };
            binding.ValidationRules.Add(typeParamValidationRule);
            comboBox.SetBinding(ComboBox.TextProperty, binding);

            typeParamCtrls.Add(comboBox);

            Grid.SetColumn(comboBox, 1);
            Grid.SetRow(comboBox, rowIndex);

            return comboBox;
        }

        private static DependencyProperty GetTypeParamProperty(string typeParamName)
        {
            var typeParamProperty = TypeParamProperties.FirstOrDefault(p => p.Name == typeParamName);
            if (typeParamProperty == null)
            {
                typeParamProperty = DependencyProperty.Register(typeParamName, typeof(string), typeof(ExportOptionsDlg),
                    new FrameworkPropertyMetadata(_ParamOptions[0], FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
                TypeParamProperties.Add(typeParamProperty);
            }

            return typeParamProperty;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.MinWidth = this.ActualWidth;
            this.MinHeight = this.ActualHeight;
            this.MaxHeight = this.ActualHeight;

            var hwnd = new WindowInteropHelper((Window)sender).Handle;
            var value = GetWindowLongA(hwnd, GWL_STYLE);
            _ = SetWindowLongA(hwnd, GWL_STYLE, (int)(value & ~(WS_MAXIMIZEBOX | WS_MINIMIZEBOX)));
            var mainWindowSrc = HwndSource.FromHwnd(hwnd);
            mainWindowSrc.AddHook(WndProc);
        }

        private static IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            // Override the window hit test
            // and if the cursor is over a resize border,
            // return a standard border result instead.
            if (msg == WM_NCHITTEST)
            {
                handled = true;
                var htLocation = DefWindowProcW(hwnd, msg, wParam, lParam).ToInt32();
                switch (htLocation)
                {
                    case HTLEFT:
                        break;

                    case HTTOPLEFT:
                    case HTBOTTOMLEFT:
                        htLocation = HTLEFT;
                        break;

                    case HTRIGHT:
                        break;

                    case HTTOPRIGHT:
                    case HTBOTTOMRIGHT:
                        htLocation = HTRIGHT;
                        break;

                    case HTTOP:
                    case HTBOTTOM:
                        htLocation = HTBORDER;
                        break;
                }

                return new IntPtr(htLocation);
            }

            return IntPtr.Zero;
        }
    }
}
