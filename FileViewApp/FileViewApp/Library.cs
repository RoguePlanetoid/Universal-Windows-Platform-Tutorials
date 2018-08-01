using System;
using System.Collections.Generic;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Controls;

namespace FileViewApp
{
    public class FileViewItem
    {
        public string Name { get; set; }
        public string Path { get; set; }
    }

    public class Library
    {
        private async void FillNode(TreeViewNode node)
        {
            StorageFolder folder = null;
            if (node.Content is StorageFolder && node.HasUnrealizedChildren)
            {
                folder = (StorageFolder)node.Content;
            }
            else
            {
                return;
            }
            IReadOnlyList<IStorageItem> list = await folder.GetItemsAsync();
            if (list.Count == 0) return;
            foreach (IStorageItem item in list)
            {
                TreeViewNode itemNode = new TreeViewNode
                {
                    Content = item
                };
                if (item is StorageFolder)
                {
                    itemNode.HasUnrealizedChildren = true;
                }
                node.Children.Add(itemNode);
            }
            node.HasUnrealizedChildren = false;
        }

        public void Expanding(TreeViewNode node)
        {
            if (node.HasChildren) FillNode(node);
        }

        public void Collapsed(TreeViewNode node)
        {
            node.Children.Clear();
            node.HasUnrealizedChildren = true;
        }

        public FileViewItem Invoked(TreeViewNode node)
        {
            FileViewItem item = null;
            if (node.Content is IStorageItem storageItem)
            {
                item = new FileViewItem()
                {
                    Name = storageItem.Name,
                    Path = storageItem.Path
                };
                if (node.Content is StorageFolder)
                {
                    node.IsExpanded = !node.IsExpanded;
                }
            }
            return item;
        }

        public async void Folder(TreeView tree)
        {
            try
            {
                FolderPicker picker = new FolderPicker()
                {
                    SuggestedStartLocation = PickerLocationId.PicturesLibrary
                };
                picker.FileTypeFilter.Add("*");
                IStorageFolder folder = await picker.PickSingleFolderAsync();
                if (folder != null)
                {
                    tree.RootNodes.Clear();
                    TreeViewNode node = new TreeViewNode
                    {
                        Content = folder,
                        IsExpanded = true,
                        HasUnrealizedChildren = true
                    };
                    tree.RootNodes.Add(node);
                    FillNode(node);
                }
            }
            finally
            {
                // Ignore Exceptions
            }
        }
    }
}