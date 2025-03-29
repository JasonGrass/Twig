using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Jgrass.Twig.Wpf;
using Twig.Wpf.Example.Data;

namespace Twig.Wpf.Example.ViewModels;

partial class FirstSubViewModel : ObservableObject
{
    public FirstSubViewModel()
    {
        var data = new FirstLevelShareData() { Message = Guid.NewGuid().ToString() };
        TwigTreeUtils.Provide(this, data);
    }

    [RelayCommand]
    public void ShowData()
    {
        var data = TwigTreeUtils.Get<FirstLevelShareData>(this);
        if (data != null)
        {
            MessageBox.Show(data.Message);
        }
        else
        {
            MessageBox.Show("Data not found.");
        }
    }
}
