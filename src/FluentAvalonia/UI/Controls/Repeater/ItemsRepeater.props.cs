﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using FluentAvalonia.Core;

namespace FluentAvalonia.UI.Controls;

public partial class ItemsRepeater : Panel
{
    public static readonly StyledProperty<double> VerticalCacheLengthProperty =
        AvaloniaProperty.Register<ItemsRepeater, double>(nameof(VerticalCacheLength), defaultValue: 2.0);

    public static readonly StyledProperty<double> HorizontalCacheLengthProperty =
        AvaloniaProperty.Register<ItemsRepeater, double>(nameof(HorizontalCacheLength), defaultValue: 2.0);

    public static readonly StyledProperty<Layout> LayoutProperty =
        AvaloniaProperty.Register<ItemsRepeater, Layout>(nameof(Layout), defaultValue: new StackLayout());

    public static readonly StyledProperty<object> ItemsSourceProperty =
        AvaloniaProperty.Register<ItemsRepeater, object>(nameof(ItemsSource));

    public static readonly StyledProperty<IDataTemplate> ItemTemplateProperty =
        AvaloniaProperty.Register<ItemsRepeater, IDataTemplate>(nameof(ItemTemplate));

    public static readonly StyledProperty<ElementAnimator> AnimatorProperty =
        AvaloniaProperty.Register<ItemsRepeater, ElementAnimator>(nameof(Animator));

    public double VerticalCacheLength
    {
        get => GetValue(VerticalCacheLengthProperty);
        set => SetValue(VerticalCacheLengthProperty, value);
    }

    public double HorizontalCacheLength
    {
        get => GetValue(HorizontalCacheLengthProperty);
        set => SetValue(HorizontalCacheLengthProperty, value);
    }

    public Layout Layout
    {
        get => GetValue(LayoutProperty);
        set => SetValue(LayoutProperty, value);
    }

    public object ItemsSource
    {
        get => GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public IDataTemplate ItemTemplate
    {
        get => GetValue(ItemTemplateProperty);
        set => SetValue(ItemTemplateProperty, value);
    }

    public ElementAnimator Animator
    {
        get => GetValue(AnimatorProperty);
        set => SetValue(AnimatorProperty, value);
    }

    public FAItemsSourceView ItemsSourceView => _itemsSourceView;

    internal Control MadeAnchor => _viewportManager.MadeAnchor;

    internal object LayoutState
    {
        get => _layoutState;
        set => _layoutState = value;
    }

    internal Rect VisibleWindow => _viewportManager.GetLayoutVisibleWindow();

    internal Rect RealizationWindow => _viewportManager.GetLayoutRealizationWindow();

    internal Control SuggestedAnchor => _viewportManager.SuggestedAnchor;

    internal Point LayoutOrigin
    {
        get => _layoutOrigin;
        set => _layoutOrigin = value;
    }

    internal IElementFactory ItemTemplateShim => _itemTemplateWrapper;

    internal AnimationManager AnimationManager => _animationManager;

    internal ViewManager ViewManager => _viewManager;

    private bool IsProcessingCollectionChange => _processingItemsSourceChange != null;

    public event TypedEventHandler<ItemsRepeater, ItemsRepeaterElementPreparedEventArgs> ElementPrepared;
    public event TypedEventHandler<ItemsRepeater, ItemsRepeaterElementClearingEventArgs> ElementClearing;
    public event TypedEventHandler<ItemsRepeater, ItemsRepeaterElementIndexChangedEventArgs> ElementIndexChanged;

    public event TypedEventHandler<ItemsRepeater, ContainerContentChangingEventArgs> ContainerContentChanging;

    internal static readonly AttachedProperty<VirtualizationInfo> VirtualizationInfoProperty =
        AvaloniaProperty.RegisterAttached<ItemsRepeater, Control, VirtualizationInfo>("VirtualizationInfo");

    internal static VirtualizationInfo GetVirtualizationInfo(Control c)
    {
        var result = c.GetValue(VirtualizationInfoProperty);

        if (result == null)
        {
            result = CreateAndInitializeVirtualizationInfo(c);
        }

        return result;
    }

    internal static VirtualizationInfo TryGetVirtualizationInfo(Control c) =>
        GetVirtualizationInfo(c);

    internal static VirtualizationInfo CreateAndInitializeVirtualizationInfo(Control element)
    {
        var result = new VirtualizationInfo();
        element.SetValue(VirtualizationInfoProperty, result);
        return result;
    }


    internal void RaiseContainerContentChanging(int index, object item, Control container,
        VirtualizationInfo virtInfo)
    {
        if (ContainerContentChanging != null)
        {
            var args = new ContainerContentChangingEventArgs(index, item,
                container, virtInfo);

            ContainerContentChanging?.Invoke(this, args);
        }
    }

    private ContainerContentChangingEventArgs _containerContentChangingArgs;
}

public class ContainerContentChangingEventArgs : EventArgs
{
    internal ContainerContentChangingEventArgs(int index, object item,
        Control container, VirtualizationInfo virtInfo)
    {
        ItemIndex = index;
        Item = item;
        ItemContainer = container;
        _virtInfo = virtInfo;
    }

    internal ContainerContentChangingEventArgs(int index, object item,
        Control container, VirtualizationInfo virtInfo, int phase)
    {
        ItemIndex = index;
        Item = item;
        ItemContainer = container;
        _virtInfo = virtInfo;
        Phase = phase;
    }

    //public bool Handled { get; set; }

    //public bool InRecycleQueue { get; internal set; }

    public object Item { get; internal set; }

    public Control ItemContainer { get; internal set; }

    public int ItemIndex { get; internal set; }

    public int Phase { get; private set; }

    public void RegisterUpdateCallback(
        TypedEventHandler<ItemsRepeater, ContainerContentChangingEventArgs> callback)
    {
        _virtInfo.UpdatePhasingInfo(Phase + 1, Item, callback);
    }

    private VirtualizationInfo _virtInfo;
}