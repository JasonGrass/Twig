# Twig

[![](https://img.shields.io/nuget/v/Jgrass.Twig.Core?logo=nuget)](https://www.nuget.org/packages/Jgrass.Twig.Core/)

[![](https://img.shields.io/nuget/v/Jgrass.Twig.Wpf?logo=nuget)](https://www.nuget.org/packages/Jgrass.Twig.Wpf/)

A tool for data sharing between View/ViewModel based on WPF logical trees.

一个基于 WPF 逻辑树，在 View/ViewModel 中进行数据共享的工具。

[中文](./README.zh.md)

## The problem

In MVVM programming, ViewModels are usually independent, especially after introducing frameworks like Prism, which often manage ViewModels. This raises the question of how to facilitate communication and data sharing between different ViewModels.

Event-based approaches such as EventAggregator can be used, but they are not convenient for data sharing. Particularly, when a parent View(ViewModel) has multiple child Views(ViewModels), collecting data from all child ViewModels requires writing numerous event handlers.

Another approach is to manually manage ViewModels, explicitly retaining references to all child ViewModels in the parent ViewModel. While this method is clearer, it requires additional coding and introduces the complexity of maintaining ViewModel instance references when multiple instances of the same View/ViewModel exist.

Inspired by the Provide/Inject pattern in Vue and the Context API in React, this tool has been designed.

```txt
MainView/MainViewModel
    FooView/FooViewModel ------------->  provide MyShareData(instance1)
        BarView/BarViewModel --------->  get MyShareData(instance1)
    FooView/FooViewModel ------------->  provider MyShareData(instance2)
        BarView/BarViewModel --------->  get MyShareData(instance2)
```

## How to use

install by nuget

```sh
Install-Package Jgrass.Twig.Wpf
Install-Package Jgrass.Twig.Core
```

1 Inject View (WPF)

For the View/ViewModel that provides or gets data, ensure that this View is bound to the correct ViewModel (DataContext).

```cs
public partial class FooView : UserControl
{
    public FooView()
    {
        InitializeComponent();
        TwigTreeUtils.Inject(this);
    }
}
```

2 Provide data

Provide data in the parent View/ViewModel.

```cs
public FooViewModel()
{
    var data = new MyShareData() { Message = Guid.NewGuid().ToString() };
    TwigTreeUtils.Provide(this, data);
}
```

3 Get data

Get data in the current or a child View/ViewModel.

```cs
var data = TwigTreeUtils.Get<MyShareData>(this);
if (data != null)
{
    MessageBox.Show(data.Message);
}
else
{
    MessageBox.Show("Data not found.");
}
```
