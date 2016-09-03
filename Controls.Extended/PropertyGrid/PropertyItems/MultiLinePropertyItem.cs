﻿namespace Imagin.Controls.Extended
{
    public sealed class MultiLinePropertyItem : StringPropertyItem
    {
        public MultiLinePropertyItem(object SelectedObject, string Name, object Value, string Category, bool IsReadOnly, bool IsFeatured = false) : base(SelectedObject, Name, Value, Category, IsReadOnly, IsFeatured)
        {
            this.Type = PropertyType.MultiLine;
        }
    }
}
