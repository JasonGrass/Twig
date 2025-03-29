# Jgrass.Twig

一个基于 WPF 逻辑树，在 View/ViewModel 中进行数据共享的工具

## The problem

在 MVVM 编程中，各个 ViewModel 通常是各自独立的，尤其是引入 prism 等框架之后，ViewModel 通常交给框架管理。
但这带来一个问题是，如何在各个 ViewModel 之间进行通信和数据共享？

可以使用 EventAggregator 等基于事件的处理方式，但对于数据共享而言，EventAggregator 并不方便。
尤其是一个父 View(ViewModel) 有多个子 View(ViewModel) 时，想要收集所有子 ViewModel 中的数据，就需要写很多事件处理。

另外一种方式，就是手动管理 ViewModel，在父 ViewModel 中，显式保留所有子 ViewModel 的引用，这样其实更清晰，
但也需要额外写一些代码，更重要的是，如果同一个 View，存在多个 View/ViewModel 的实例时，就需要小心维护 ViewModel 的实例引用了。

借鉴前端 Vue 中 Provide/Inject 以及 React 的 Context API 类似的思想，设计了本工具。

```txt
MainView/MainViewModel
    FooView/FooViewModel ------------->  provide MyShareData(instance1)
        BarView/BarViewModel --------->  get MyShareData(instance1)
    FooView/FooViewModel ------------->  provider MyShareData(instance2)
        BarView/BarViewModel --------->  get MyShareData(instance2)
```

## How to use

1 Inject View (WPF)

进行 provide 或者 get 数据的 View/ViewModel，请确保此 View 绑定了正确的 ViewModel（DataContext）。

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

在父 View/ViewModel 中 provide 数据

```cs
public FooViewModel()
{
    var data = new MyShareData() { Message = Guid.NewGuid().ToString() };
    TwigTreeUtils.Provide(this, data);
}
```

3 Get data

在当前或者子 View/ViewModel 中 get 数据

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
