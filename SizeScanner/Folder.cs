// Decompiled with JetBrains decompiler
// Type: Kalantyr.SizeScanner.Folder
// Assembly: Kalantyr.SizeScanner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=388c716fd36611fb
// MVID: 579142F0-7A9B-4B68-9FBA-93B76C5631F8
// Assembly location: Q:\Backup\Documents\Old\trunk\TreeSizeScanner\Kalantyr.SizeScanner\bin\Debug\Kalantyr.SizeScanner.exe

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Data;

namespace SizeScanner
{
  public class Folder : DependencyObject
  {
    public static readonly DependencyProperty SizeProperty = DependencyProperty.Register(nameof (Size), typeof (long), typeof (Folder), new PropertyMetadata((object) 0L));

    public DirectoryInfo DirectoryInfo { get; private set; }

    public ObservableCollection<FileInfo> Files { get; private set; }

    public ObservableCollection<Folder> Folders { get; private set; }

    public CollectionView FoldersView { get; private set; }

    public long Size
    {
      get
      {
        return (long) this.GetValue(Folder.SizeProperty);
      }
      set
      {
        long size = this.Size;
        if (value == size)
          return;
        this.SetValue(Folder.SizeProperty, (object) value);
        if (this.SizeChanged == null)
          return;
          OnPropertyChanged(new DependencyPropertyChangedEventArgs(SizeProperty, size, value));
      }
    }

    public Folder Parent { get; private set; }

    public Folder(DirectoryInfo directoryInfo, Folder parent)
    {
      if (directoryInfo == null)
        throw new ArgumentNullException(nameof (directoryInfo));
      this.DirectoryInfo = directoryInfo;
      this.Parent = parent;
      this.Files = new ObservableCollection<FileInfo>();
      this.Files.CollectionChanged += new NotifyCollectionChangedEventHandler(this.Files_CollectionChanged);
      this.Folders = new ObservableCollection<Folder>();
      this.Folders.CollectionChanged += new NotifyCollectionChangedEventHandler(this.Folders_CollectionChanged);
      this.FoldersView = (CollectionView) new ListCollectionView((IList) this.Folders);
    }

    public void OnScanComplette()
    {
      this.FoldersView.SortDescriptions.Add(new SortDescription(Folder.SizeProperty.Name, ListSortDirection.Descending));
    }

    private void Folders_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      switch (e.Action)
      {
        case NotifyCollectionChangedAction.Add:
          IEnumerator enumerator1 = e.NewItems.GetEnumerator();
          try
          {
            while (enumerator1.MoveNext())
            {
              Folder current = (Folder) enumerator1.Current;
              this.Size += current.Size;
              current.SizeChanged += new EventHandler<SizeChangedEventArgs>(this.SubFolder_SizeChanged);
            }
            break;
          }
          finally
          {
            (enumerator1 as IDisposable)?.Dispose();
          }
        case NotifyCollectionChangedAction.Remove:
          IEnumerator enumerator2 = e.OldItems.GetEnumerator();
          try
          {
            while (enumerator2.MoveNext())
            {
              Folder current = (Folder) enumerator2.Current;
              current.SizeChanged -= new EventHandler<SizeChangedEventArgs>(this.SubFolder_SizeChanged);
              this.Size -= current.Size;
            }
            break;
          }
          finally
          {
            (enumerator2 as IDisposable)?.Dispose();
          }
        default:
          throw new NotImplementedException();
      }
    }

    public void SubFolder_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      this.Size += (long)(e.NewSize.Width - e.PreviousSize.Width);
    }

    private void Files_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      switch (e.Action)
      {
        case NotifyCollectionChangedAction.Add:
          IEnumerator enumerator1 = e.NewItems.GetEnumerator();
          try
          {
            while (enumerator1.MoveNext())
              this.Size += ((FileInfo) enumerator1.Current).Length;
            break;
          }
          finally
          {
            (enumerator1 as IDisposable)?.Dispose();
          }
        case NotifyCollectionChangedAction.Remove:
          IEnumerator enumerator2 = e.OldItems.GetEnumerator();
          try
          {
            while (enumerator2.MoveNext())
              this.Size -= ((FileInfo) enumerator2.Current).Length;
            break;
          }
          finally
          {
            (enumerator2 as IDisposable)?.Dispose();
          }
        default:
          throw new NotImplementedException();
      }
    }

    public event EventHandler<SizeChangedEventArgs> SizeChanged;
  }
}
