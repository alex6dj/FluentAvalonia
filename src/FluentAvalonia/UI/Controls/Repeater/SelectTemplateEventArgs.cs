﻿using Avalonia.Controls;

namespace FluentAvalonia.UI.Controls;

public class SelectTemplateEventArgs : EventArgs
{
    internal SelectTemplateEventArgs() { }

    internal SelectTemplateEventArgs(object dataContext, Control owner)
    {
        DataContext = dataContext;
        Owner = owner;
    }

    public string TemplateKey { get; set; }

    public object DataContext { get; internal set; }

    public Control Owner { get; internal set; }
}
