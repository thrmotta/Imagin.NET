﻿using System;

namespace Imagin.Controls.Extended
{
    public sealed class GuidPropertyItem : PropertyItem
    {
        public override void SetValue(object NewValue)
        {
            if (this.Info != null) 
                this.Info.SetValue(SelectedObject, NewValue == null ? default(Guid) : (Guid)NewValue, null);
        }

        public GuidPropertyItem(object SelectedObject, string Name, object Value, string Category, bool IsReadOnly, bool IsFeatured = false) : base(SelectedObject, Name, Value, Category, IsReadOnly, IsFeatured)
        {
            this.Type = PropertyType.Guid;
        }
    }
}
