﻿using System.Collections.Generic;
using DeepStreamNet.Contracts;
using System.Collections.Specialized;
using System;
using System.Collections;

namespace DeepStreamNet
{
    class DeepStreamList : IDeepStreamList
    {
        public string ListName { get; }

        public int Count => innerList.Count;

        public bool IsReadOnly => false;

        public string this[int index]
        {
            get =>  innerList[index];
            set  {
                var oldItem = innerList[index];
                innerList[index] = value;
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, oldItem, index));
            }
        }

        readonly IDeepStreamRecordWrapper ListRecord;

        readonly List<string> innerList = new List<string>();

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public DeepStreamList(string name, IDeepStreamRecordWrapper listRecord)
        {
            ListName = name;
            ListRecord = listRecord;
        }

        public int IndexOf(string item)
        {
            return innerList.IndexOf(item);
        }

        public void Insert(int index, string item)
        {
            innerList.Insert(index, item);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, null, index));
        }

        public void RemoveAt(int index)
        {
            var oldItem = innerList[index];
            innerList.RemoveAt(index);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, null, oldItem, index));
        }

        public void Add(string item)
        {
            innerList.Add(item);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }

        public void Clear()
        {
            var tmp = innerList;
            innerList.Clear();
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset, null, tmp));
        }

        public bool Contains(string item)
        {
            return innerList.Contains(item);
        }

        public void CopyTo(string[] array, int arrayIndex)
        {
            innerList.CopyTo(array, arrayIndex);
        }

        public bool Remove(string item)
        {
            var result = innerList.Remove(item);
            if (result)
            {
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, null, item));
            }

            return result;
        }

        public IEnumerator<string> GetEnumerator()
        {
            return innerList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return innerList.GetEnumerator();
        }
    }
}