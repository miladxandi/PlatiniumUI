namespace PlatiniumUI.Fluent.DesignSystem.Platform;

/// <summary>
/// Platform helper that strips native input decorations (underlines, borders, rounded boxes)
/// from MAUI input controls so our own design system styling is the only visual treatment.
/// Call StripNativeDecorations() on any Entry, Picker, DatePicker, TimePicker, or SearchBar
/// after the handler is available.
/// </summary>
public static class FluentInputHelper
{
    /// <summary>
    /// Removes all native platform decorations (underline, border, background rounding)
    /// from the given input view. Call after the handler is attached (e.g. in HandlerChanged).
    /// </summary>
    public static void StripNativeDecorations(Microsoft.Maui.Controls.View inputView)
    {
        if (inputView == null) return;

        inputView.HandlerChanged += OnHandlerChanged;

        void OnHandlerChanged(object? sender, EventArgs e)
        {
            if (inputView.Handler?.PlatformView == null) return;
            inputView.HandlerChanged -= OnHandlerChanged;
            DoStrip(inputView.Handler.PlatformView);
        }
    }

    /// <summary>
    /// Immediately strips decorations if the handler is already available,
    /// otherwise waits for HandlerChanged.
    /// </summary>
    public static void StripNativeDecorationsImmediate(Microsoft.Maui.Controls.View inputView)
    {
        if (inputView == null) return;

        if (inputView.Handler?.PlatformView != null)
        {
            DoStrip(inputView.Handler.PlatformView);
        }
        else
        {
            StripNativeDecorations(inputView);
        }
    }

    private static void DoStrip(object platformView)
    {
#if WINDOWS
        StripWindows(platformView);
#elif ANDROID
        StripAndroid(platformView);
#elif IOS
        StripIos(platformView);
#endif
    }

#if WINDOWS
    private static void StripWindows(object platformView)
    {
        // WinUI 3: Remove border and background from TextBox / ComboBox / etc.
        if (platformView is Microsoft.UI.Xaml.Controls.TextBox textBox)
        {
            textBox.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);
            textBox.BorderBrush = null;
            textBox.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Transparent);
            // Remove the default padding that the border creates
            textBox.Padding = new Microsoft.UI.Xaml.Thickness(0);
        }
        else if (platformView is Microsoft.UI.Xaml.Controls.ComboBox comboBox)
        {
            comboBox.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);
            comboBox.BorderBrush = null;
            comboBox.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Transparent);
        }
        else if (platformView is Microsoft.UI.Xaml.Controls.CalendarDatePicker calendarPicker)
        {
            calendarPicker.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);
            calendarPicker.BorderBrush = null;
            calendarPicker.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Transparent);
        }
        else if (platformView is Microsoft.UI.Xaml.Controls.TimePicker timePicker)
        {
            // WinUI 3 doesn't have a native TimePicker with border; it uses a Flyout
        }
        else if (platformView is Microsoft.UI.Xaml.Controls.AutoSuggestBox suggestBox)
        {
            suggestBox.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);
            suggestBox.BorderBrush = null;
            suggestBox.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Transparent);
        }

        // Generic FrameworkElement fallback: try to clear border via visual tree
        if (platformView is Microsoft.UI.Xaml.FrameworkElement element)
        {
            // Walk and clear any Border child decorations
            ClearBorderDecorations(element);
        }
    }

    private static void ClearBorderDecorations(Microsoft.UI.Xaml.DependencyObject element)
    {
        int childCount = Microsoft.UI.Xaml.Media.VisualTreeHelper.GetChildrenCount(element);
        for (int i = 0; i < childCount; i++)
        {
            var child = Microsoft.UI.Xaml.Media.VisualTreeHelper.GetChild(element, i);

            if (child is Microsoft.UI.Xaml.Controls.Border border)
            {
                border.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);
                border.BorderBrush = null;
                border.Background = null;
            }

            ClearBorderDecorations(child);
        }
    }
#endif

#if ANDROID
    private static void StripAndroid(object platformView)
    {
        if (platformView is global::Android.Widget.EditText editText)
        {
            // Remove the underline drawable
            editText.Background = null;
        }
        else if (platformView is global::Android.Widget.TextView textView)
        {
            textView.Background = null;
        }
        else if (platformView is global::Android.Widget.Spinner spinner)
        {
            spinner.Background = null;
        }
        else if (platformView is global::Android.Widget.DatePicker datePicker)
        {
            // Android Material DatePicker styling
        }
        else if (platformView is global::Android.Widget.TimePicker timePicker)
        {
            // Android Material TimePicker styling
        }
    }
#endif

#if IOS
    private static void StripIos(object platformView)
    {
        if (platformView is UIKit.UITextField textField)
        {
            textField.BorderStyle = UIKit.UITextBorderStyle.None;
            textField.BackgroundColor = UIKit.UIColor.Clear;
        }
        else if (platformView is UIKit.UITextView textView)
        {
            textView.Layer.BorderWidth = 0;
            textView.Layer.BorderColor = UIKit.UIColor.Clear.CGColor;
            textView.BackgroundColor = UIKit.UIColor.Clear;
        }
    }
#endif
}
