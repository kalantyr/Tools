// Decompiled with JetBrains decompiler
// Type: Kalantyr.SizeScanner.Model
// Assembly: Kalantyr.SizeScanner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=388c716fd36611fb
// MVID: 579142F0-7A9B-4B68-9FBA-93B76C5631F8
// Assembly location: Q:\Backup\Documents\Old\trunk\TreeSizeScanner\Kalantyr.SizeScanner\bin\Debug\Kalantyr.SizeScanner.exe

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using SizeScanner;

namespace Kalantyr.SizeScanner
{
  public class Model : DependencyObject
  {
    public readonly DependencyProperty IsScanningProperty = DependencyProperty.Register(nameof (IsScanning), typeof (bool), typeof (Model), new PropertyMetadata((object) false));
    public readonly DependencyProperty IsIdleProperty = DependencyProperty.Register(nameof (IsIdle), typeof (bool), typeof (Model), new PropertyMetadata((object) true));
    public readonly DependencyProperty SelectedFolderProperty = DependencyProperty.Register(nameof (SelectedFolder), typeof (Folder), typeof (Model), new PropertyMetadata((PropertyChangedCallback) null));
    public readonly DependencyProperty SelectedFilesProperty = DependencyProperty.Register(nameof (SelectedFiles), typeof (IEnumerable<FileInfo>), typeof (Model), new PropertyMetadata((PropertyChangedCallback) null));
    public readonly DependencyProperty SelectedFileProperty = DependencyProperty.Register(nameof (SelectedFile), typeof (FileInfo), typeof (Model), new PropertyMetadata((PropertyChangedCallback) null));
    public readonly DependencyProperty ScanningFolderProperty = DependencyProperty.Register(nameof (ScanningFolder), typeof (Folder), typeof (Model), new PropertyMetadata((PropertyChangedCallback) null));
    public readonly DependencyProperty TargetPathProperty = DependencyProperty.Register(nameof (TargetPath), typeof (string), typeof (Model), new PropertyMetadata((object) string.Empty));

    public bool IsScanning
    {
      get
      {
        return (bool) this.GetValue(this.IsScanningProperty);
      }
      set
      {
        this.SetValue(this.IsScanningProperty, (object) value);
        this.IsIdle = !value;
      }
    }

    public bool IsIdle
    {
      get
      {
        return (bool) this.GetValue(this.IsIdleProperty);
      }
      private set
      {
        this.SetValue(this.IsIdleProperty, (object) value);
      }
    }

    public ObservableCollection<Folder> ScanResult { get; private set; }

    public CollectionView ScanResultView { get; private set; }

    public Folder SelectedFolder
    {
      get
      {
        return (Folder) this.GetValue(this.SelectedFolderProperty);
      }
      set
      {
        this.SetValue(this.SelectedFolderProperty, (object) value);
      }
    }

    public IEnumerable<FileInfo> SelectedFiles
    {
      get
      {
        return (IEnumerable<FileInfo>) this.GetValue(this.SelectedFilesProperty);
      }
      set
      {
        this.SetValue(this.SelectedFilesProperty, (object) value);
        this.SelectedFile = value.Count<FileInfo>() == 1 ? value.First<FileInfo>() : (FileInfo) null;
      }
    }

    public FileInfo SelectedFile
    {
      get
      {
        return (FileInfo) this.GetValue(this.SelectedFileProperty);
      }
      protected set
      {
        this.SetValue(this.SelectedFileProperty, (object) value);
      }
    }

    public Folder ScanningFolder
    {
      get
      {
        return (Folder) this.GetValue(this.ScanningFolderProperty);
      }
      set
      {
        this.SetValue(this.ScanningFolderProperty, (object) value);
      }
    }

    public bool IsCanceling { get; set; }

    public string TargetPath
    {
      get
      {
        return (string) this.GetValue(this.TargetPathProperty);
      }
      set
      {
        this.SetValue(this.TargetPathProperty, (object) value);
      }
    }

    public static Model Instance { get; private set; }

    public Model()
    {
      Model.Instance = this;
      this.ScanResult = new ObservableCollection<Folder>();
      this.ScanResultView = (CollectionView) new ListCollectionView((IList) this.ScanResult);
      this.ScanResultView.SortDescriptions.Add(new SortDescription(Folder.SizeProperty.Name, ListSortDirection.Descending));
    }
  }
}
