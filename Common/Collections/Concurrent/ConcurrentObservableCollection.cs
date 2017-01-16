﻿using Imagin.Common.Collections.Generic;
using Imagin.Common.Input;
using Imagin.Common.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Imagin.Common.Collections.Concurrent
{
    /// <summary>
    /// This class provides a collection that can be bound to
    /// a WPF control, where the collection can be modified from a thread
    /// that is not the GUI thread. The notify event is thrown using the
    /// dispatcher from the event listener(s).
    /// </summary>
    /// <typeparam name="T">The type of the elements in the collection.</typeparam>
    [Serializable]
    public class ConcurrentObservableCollection<T> : ConcurrentObservableBase<T>, ICollection<T>, IList<T>, IHierarchialCollection<T>, ITrackableCollection<T>, IList, ICollection, INotifyPropertyChanged
    {
        #region Properties

        #region ITrackableList<T>

        /// <summary>
        /// Occurs when a single item is added.
        /// </summary>
        [field: NonSerializedAttribute()]
        public event EventHandler<EventArgs<T>> ItemAdded;

        /// <summary>
        /// Occurs when any number of items are added.
        /// </summary>
        [field: NonSerializedAttribute()]
        public event EventHandler<EventArgs<IEnumerable<T>>> ItemsAdded;

        /// <summary>
        /// Occurs when the collection changes.
        /// </summary>
        [field: NonSerializedAttribute()]
        public event EventHandler<EventArgs> ItemsChanged;

        /// <summary>
        /// Occurs when the collection is cleared.
        /// </summary>
        [field: NonSerializedAttribute()]
        public event EventHandler<EventArgs> ItemsCleared;

        /// <summary>
        /// Occurs when a single item is inserted.
        /// </summary>
        [field: NonSerializedAttribute()]
        public event EventHandler<EventArgs<T>> ItemInserted;

        /// <summary>
        /// Occurs when a single item is removed.
        /// </summary>
        [field: NonSerializedAttribute()]
        public event EventHandler<EventArgs<T>> ItemRemoved;

        /// <summary>
        /// Occurs when any number of items are removed.
        /// </summary>
        [field: NonSerializedAttribute()]
        public event EventHandler<EventArgs<IEnumerable<T>>> ItemsRemoved;

        bool isEmpty = true;
        public bool IsEmpty
        {
            get
            {
                return this.isEmpty;
            }
            set
            {
                this.isEmpty = value;
                this.OnPropertyChanged("IsEmpty");
            }
        }

        int total = 0;
        public int Total
        {
            get
            {
                return this.total;
            }
            set
            {
                this.total = value;
                this.OnPropertyChanged("Total");
            }
        }

        #endregion

        #endregion

        #region ConcurrentObservableCollection

        public ConcurrentObservableCollection() : base()
        {
        }

        public ConcurrentObservableCollection(T Item) : base()
        {
            this.Add(Item);
        }

        public ConcurrentObservableCollection(params T[] Items) : base(Items)
        {
        }

        public ConcurrentObservableCollection(IEnumerable<T> Items) : base(Items)
        {
        }

        public ConcurrentObservableCollection(params IEnumerable<T>[] Collections) : base()
        {
            foreach (IEnumerable<T> Items in Collections)
            {
                foreach (T t in Items)
                    this.Add(t);
            }
        }

        #endregion

        #region Methods

        #region Private

        #region ICollection

        void ICollection.CopyTo(Array array, int index)
        {
            DoBaseRead(() => ((ICollection)ReadCollection).CopyTo(array, index));
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                return DoBaseRead(() =>
                {
                    return ((ICollection)ReadCollection).IsSynchronized;
                });
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                return DoBaseRead(() =>
                {
                    return ((ICollection)ReadCollection).SyncRoot;
                });
            }
        }

        #endregion 

        #region IList 

        int IList.Add(object value)
        {
            return DoBaseWrite(() =>
            {
                return ((IList)WriteCollection).Add(value);
            });
        }

        bool IList.Contains(object value)
        {
            return DoBaseRead(() =>
            {
                return ((IList)ReadCollection).Contains(value);
            });
        }

        int IList.IndexOf(object value)
        {
            return DoBaseRead(() =>
            {
                return ((IList)ReadCollection).IndexOf(value);
            });
        }

        void IList.Insert(int index, object value)
        {
            DoBaseWrite(() =>
            {
                ((IList)WriteCollection).Insert(index, value);
            });
        }

        bool IList.IsFixedSize
        {
            get
            {
                return DoBaseRead(() =>
                {
                    return ((IList)ReadCollection).IsFixedSize;
                });
            }
        }

        bool IList.IsReadOnly
        {
            get
            {
                return DoBaseRead(() =>
                {
                    return ((IList)ReadCollection).IsReadOnly;
                });
            }
        }

        void IList.Remove(object value)
        {
            DoBaseWrite(() => ((IList)WriteCollection).Remove(value));
        }

        void IList.RemoveAt(int index)
        {
            DoBaseWrite(() => {
                ((IList)WriteCollection).RemoveAt(index);
            });
        }

        object IList.this[int index]
        {
            get
            {
                return DoBaseRead(() => {
                    return ((IList)ReadCollection)[index];
                });
            }
            set
            {
                DoBaseWrite(() => {
                    ((IList)WriteCollection)[index] = value;
                });
            }
        }

        #endregion

        #endregion

        #region Protected

        protected virtual void OnItemAdded(T Item)
        {
            if (this.ItemAdded != null)
                this.ItemAdded(this, new EventArgs<T>(Item));
        }

        protected virtual void OnItemsAdded(IEnumerable<T> Items)
        {
            if (this.ItemsAdded != null)
                this.ItemsAdded(this, new EventArgs<IEnumerable<T>>(Items));
        }

        protected virtual void OnItemsChanged()
        {
            this.Total = this.Count;
            this.IsEmpty = this.Total == 0;

            if (this.ItemsChanged != null)
                this.ItemsChanged(this, new EventArgs());
        }

        protected virtual void OnItemsCleared()
        {
            if (this.ItemsCleared != null)
                this.ItemsCleared(this, new EventArgs());
        }

        protected virtual void OnItemInserted(T Item, int Index)
        {
            if (this.ItemInserted != null)
                this.ItemInserted(this, new EventArgs<T>(Item, Index));
        }

        protected virtual void OnItemRemoved(T Item)
        {
            if (this.ItemRemoved != null)
                this.ItemRemoved(this, new EventArgs<T>(Item));
        }

        protected virtual void OnItemsRemoved(IEnumerable<T> OldItems)
        {
            if (this.ItemsRemoved != null)
                this.ItemsRemoved(this, new EventArgs<IEnumerable<T>>(OldItems));
        }

        #endregion

        #region Public

        #region ICollection<T>

        public void Add(T Item)
        {
            DoBaseWrite(() => WriteCollection.Add(Item));
            this.OnItemAdded(Item);
            this.OnItemsChanged();
        }

        public void Clear()
        {
            DoBaseClear(null);
            this.OnItemsChanged();
        }

        public bool Contains(T item)
        {
            return DoBaseRead(() =>
            {
                return ReadCollection.Contains(item);
            });
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            DoBaseRead(() =>
            {
                if (array.Count() >= ReadCollection.Count)
                    ReadCollection.CopyTo(array, arrayIndex);
                else Console.Out.WriteLine("ConcurrentObservableCollection attempting to copy into wrong sized array (array.Count < ReadCollection.Count)");
            });
        }

        public int Count
        {
            get
            {
                return DoBaseRead(() =>
                {
                    return ReadCollection.Count;
                });
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return DoBaseRead(() =>
                {
                    return ((ICollection<T>)ReadCollection).IsReadOnly;
                });
            }
        }

        public bool Remove(T Item)
        {
            bool Result = DoBaseWrite(() =>
            {
                return WriteCollection.Remove(Item);
            });
            this.OnItemRemoved(Item);
            this.OnItemsChanged();
            return Result;
        }

        #endregion

        #region IHierarchialList<T>

        /// <summary>
        /// Add item A to B.
        /// </summary>
        public bool AddTo(T A, T B)
        {
            if (B.HasProperty("Items") && ((object)B.ToDynamic().Items).Is<IList<T>>())
            {
                B.ToDynamic().Items.Add(A);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Add item A to B.
        /// </summary>
        public async Task<bool> BeginAddTo(T A, T B)
        {
            bool Result = false;
            await Task.Run(new Action(() => Result = AddTo(A, B)));
            return Result;
        }

        public async Task<bool> BeginClone(T Item)
        {
            var Parent = await this.BeginGetParent(Item);

            var IList = this as IList;

            if (Parent == null)
                return false;
            else if (Parent != this && Parent is IContainer)
                IList = ((IContainer)Parent).Items;

            if (IList != null)
            {
                var Clone = Item.As<ICloneable>().Clone();
                if (Clone != null)
                {
                    IList.Add(Clone);
                    return true;
                }
            }
            return false;
        }

        async Task<object> BeginGetParent(T ToEvaluate, object Parent)
        {
            var Result = default(object);
            if (ToEvaluate != null)
            {
                await Task.Run(new Action(async () =>
                {
                    var IList = default(IList);

                    if (Parent == this)
                    {
                        IList = this as IList;
                    }
                    else if (Parent is IContainer)
                    {
                        IList = ((IContainer)Parent).Items;
                    }
                    else return;

                    var j = 1;
                    foreach (var i in IList)
                    {
                        if (i.Equals(ToEvaluate))
                        {
                            Result = Parent;
                            break;
                        }
                        else
                        {
                            if ((Result = await this.BeginGetParent(ToEvaluate, i)) != null)
                                break;
                        }
                        j++;
                    }
                }));
            } 

            return Result;
        }

        public async Task<object> BeginGetParent(T Item)
        {
            return await this.BeginGetParent(Item, this);
        }

        /// <summary>
        /// Moves item up one level.
        /// </summary>
        public async Task<bool> BeginLevelUp(T Item)
        {
            if (Item != null)
            {
                var Parent = await this.BeginGetParent(Item);
                if (Parent != null)
                {
                    if (Parent == this)
                        return true;
                    else if (Parent is IContainer)
                    {
                        var ParentOfParent = await this.BeginGetParent((T)Parent);
                        if (ParentOfParent != null)
                        {
                            var IContainer = Parent as IContainer;
                            IContainer.Items.Remove(Item as AbstractObject);

                            var IList = this as IList;
                            if (ParentOfParent != this && ParentOfParent is IContainer)
                                IList = ((IContainer)ParentOfParent).Items;

                            if (IList != null)
                            {
                                IList.Add(Item);
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        public async Task<bool> BeginMove(T Item, Direction Direction)
        {
            var Parent = await this.BeginGetParent(Item);

            if (Parent != null)
            {
                var IList = this as IList;
                if (Parent != this && Parent is IContainer)
                    IList = ((IContainer)Parent).Items;

                if (IList != null)
                {
                    int Index = IList.IndexOf(Item);

                    var ToRemove = default(T);
                    if (Direction == Direction.Up && (Index - 1) >= 0)
                        ToRemove = (T)IList[Index - 1];
                    else if (Direction == Direction.Down && (Index + 1) < IList.Count)
                        ToRemove = (T)IList[Index + 1];

                    if (ToRemove != null)
                    {
                        IList.Remove(ToRemove);
                        IList.Insert(Index, ToRemove);
                        return true;
                    }
                }
            }

            return false;
        }

        public async Task<bool> BeginMoveDown(T Item)
        {
            return await this.BeginMove(Item, Direction.Down);
        }

        public async Task<bool> BeginMoveUp(T Item)
        {
            return await this.BeginMove(Item, Direction.Up);
        }

        /// <summary>
        /// Removes data item from parent.
        /// </summary>
        public async Task<bool> BeginRemove(T Item)
        {
            if (Item != null)
            {
                var Parent = await this.BeginGetParent(Item);
                if (Parent != null)
                {
                    var IList = this as IList;
                    if (Parent != this && Parent is IContainer)
                        IList = (Parent as IContainer).Items;

                    if (IList != null)
                        IList.Remove(Item);

                    this.OnItemRemoved(Item);
                    this.OnItemsChanged();

                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Removes data item from parent.
        /// </summary>
        public async Task<bool> BeginRemove(IEnumerable<T> Items)
        {
            var Result = true;
            foreach (var i in Items)
            {
                if (!await BeginRemove(i))
                    Result = false;
            }
            return Result;
        }

        /// <summary>
        /// Wraps item in a virtual folder via recursive search (assumes Parent.Items exists).
        /// </summary>
        /// <param name="Folder">The container item.</param>
        /// <param name="Item">The item to wrap.</param>
        /// <returns>Result of wrap attempt; false if failure.</returns>
        public async Task<bool> BeginWrap(T Folder, T Item)
        {
            if (Folder == null || Item == null)
                return false;

            var Parent = await this.BeginGetParent(Item);

            var IList = default(IList);

            if (Parent == null)
            {
                return false;
            }
            else if (Parent == this)
            {
                IList = this as IList;
            }
            else if (Parent is IContainer)
            {
                IList = ((IContainer)Parent).Items;
            }
            else
            {
                return false;
            }

            if (IList != null)
            {
                var Index = IList.IndexOf(Item);

                var IContainer = (IContainer)Folder;

                IList.Remove(Item);
                IContainer.Items.Add(Item as AbstractObject);
                IList.Insert(Index, Folder);

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Removes all occurences of each item in the specified collection.
        /// </summary>
        public bool RemoveAll(IEnumerable<T> OldItems)
        {
            if (OldItems.IsNull()) return false;

            var Set = new HashSet<T>(OldItems);

            int i = 0;
            while (i < this.Count)
            {
                if (Set.Contains(this[i]))
                    this.RemoveAt(i);
                else i++;
            }
            this.OnItemsRemoved(OldItems);
            this.OnItemsChanged();

            return true;
        }

        #endregion

        #region IList<T> 

        public T this[int index]
        {
            get
            {
                return DoBaseRead(() =>
                {
                    return ReadCollection[index];
                });
            }
            set
            {
                DoBaseWrite(() => WriteCollection[index] = value);
            }
        }

        public int IndexOf(T item)
        {
            return DoBaseRead(() =>
            {
                return ReadCollection.IndexOf(item);
            });
        }

        public void Insert(int index, T item)
        {
            DoBaseWrite(() => WriteCollection.Insert(index, item));
            this.OnItemsChanged();
        }

        public void RemoveAt(int index)
        {
            DoBaseWrite(() => WriteCollection.RemoveAt(index));
            this.OnItemsChanged();
        }

        #endregion

        #region ITrackable<T>

        public void Add(IEnumerable<T> Items)
        {
            foreach (var i in Items)
                DoBaseWrite(() => WriteCollection.Add(i));
            this.OnItemsAdded(Items);
            this.OnItemsChanged();
        }

        public void Remove(IEnumerable<T> Items)
        {
            //foreach (var i in Items)
            //base.Remove(i);
            this.OnItemsRemoved(Items);
            this.OnItemsChanged();
        }

        public void RemoveAt(params int[] Indices)
        {
            var Items = new List<T>();
            var j = 0;
            foreach (var i in Indices)
            {
                Items.Add(this[i]);
                //base.Remove(Items[j]);
                j++;
            }
            this.OnItemsRemoved(Items);
            this.OnItemsChanged();
        }

        #endregion

        #endregion

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
