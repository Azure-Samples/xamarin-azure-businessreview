using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Reviewer.Core
{
    public class BusinessCell : ViewCell
    {
        public BusinessCell()
        {
            View = new BusinessCellView();
        }
    }

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BusinessCellView : ContentView
    {
        public BusinessCellView()
        {
            InitializeComponent();
        }
    }
}
