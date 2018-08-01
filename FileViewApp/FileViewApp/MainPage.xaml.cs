using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace FileViewApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        Library library = new Library();

        private void TreeView_Expanding(TreeView sender, TreeViewExpandingEventArgs args)
        {
            library.Expanding(args.Node);
        }

        private void TreeView_Collapsed(TreeView sender, TreeViewCollapsedEventArgs args)
        {
            library.Collapsed(args.Node);
        }

        private void TreeView_ItemInvoked(TreeView sender, TreeViewItemInvokedEventArgs args)
        {
            FileViewItem item = library.Invoked((TreeViewNode)args.InvokedItem);
            FileName.Text = item.Name;
            FilePath.Text = item.Path;
        }

        private void Folder_Click(object sender, RoutedEventArgs e)
        {
            library.Folder(Display);
        }
    }
}
