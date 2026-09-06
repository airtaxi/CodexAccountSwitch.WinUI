using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;

namespace CliAccountSwitcher.WinUI.ThirdParty.SelectorBarSegmented;

/// <summary>
/// Converts a segmented item's position (first, middle, last) into the hover margin applied to
/// its pill highlight so the end items keep their rounded corners visible.
/// </summary>
public class SelectorBarSegmentedMarginConverter : DependencyObject, IValueConverter
{
    /// <summary>
    /// Identifies the <see cref="LeftItemMargin"/> property.
    /// </summary>
    public static readonly DependencyProperty LeftItemMarginProperty =
        DependencyProperty.Register(nameof(LeftItemMargin), typeof(Thickness), typeof(SelectorBarSegmentedMarginConverter), new PropertyMetadata(null));

    /// <summary>
    /// Identifies the <see cref="MiddleItemMargin"/> property.
    /// </summary>
    public static readonly DependencyProperty MiddleItemMarginProperty =
        DependencyProperty.Register(nameof(MiddleItemMargin), typeof(Thickness), typeof(SelectorBarSegmentedMarginConverter), new PropertyMetadata(null));

    /// <summary>
    /// Identifies the <see cref="RightItemMargin"/> property.
    /// </summary>
    public static readonly DependencyProperty RightItemMarginProperty =
        DependencyProperty.Register(nameof(RightItemMargin), typeof(Thickness), typeof(SelectorBarSegmentedMarginConverter), new PropertyMetadata(null));

    /// <summary>
    /// Gets or sets the margin applied to the first item.
    /// </summary>
    public Thickness LeftItemMargin
    {
        get => (Thickness)GetValue(LeftItemMarginProperty);
        set => SetValue(LeftItemMarginProperty, value);
    }

    /// <summary>
    /// Gets or sets the margin applied to any middle item.
    /// </summary>
    public Thickness MiddleItemMargin
    {
        get => (Thickness)GetValue(MiddleItemMarginProperty);
        set => SetValue(MiddleItemMarginProperty, value);
    }

    /// <summary>
    /// Gets or sets the margin applied to the last item.
    /// </summary>
    public Thickness RightItemMargin
    {
        get => (Thickness)GetValue(RightItemMarginProperty);
        set => SetValue(RightItemMarginProperty, value);
    }

    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is SelectorBarItem segmentedItem && segmentedItem.Parent is ItemsRepeater parent)
        {
            if (parent.ItemsSource is IEnumerable<object> itemsSource)
            {
                var index = parent.GetElementIndex(segmentedItem);

                if (index == 0) return LeftItemMargin;
                if (index == itemsSource.Count() - 1) return RightItemMargin;

                return MiddleItemMargin;
            }
        }

        return new Thickness(3);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) => value;
}