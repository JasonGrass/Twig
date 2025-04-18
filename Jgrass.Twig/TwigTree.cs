﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jgrass.Twig.Helper;

namespace Jgrass.Twig;

/// <summary>
/// 维护一个树形结构的数据结构，保存 View/ViewModel 之前的树形关系。
/// 也可以不用手动维护，将数据保存到 ConditionalWeakTable{view, data} 中，
/// 在 get 数据时，沿着逻辑树向上查找。这是另一种实现方式。
/// </summary>
/// <typeparam name="TView"></typeparam>
public class TwigTree<TView>
    where TView : class
{
    private readonly ITwigTreeAncestorChecker<TView> _ancestorChecker;

    public IList<TwigTreeNode<TView>> Roots { get; } = [];

    public TwigTree(ITwigTreeAncestorChecker<TView> ancestorChecker)
    {
        _ancestorChecker = ancestorChecker;
    }

    public void Add(TView view, object viewModel)
    {
        if (view == null!)
        {
            throw new ArgumentNullException(nameof(view));
        }
        if (viewModel == null!)
        {
            throw new ArgumentNullException(nameof(viewModel));
        }

        var node = new TwigTreeNode<TView>(view, viewModel);

        // 线程不安全，都应该在 UI 线程调用
        new TwigTreeBuildHelper<TView>(Roots, _ancestorChecker).Add(node);
    }

    public IList<TwigTreeNode<TView>> GetAllAncestors(object obj)
    {
        if (obj == null!)
        {
            throw new ArgumentNullException(nameof(obj));
        }

        foreach (var root in Roots)
        {
            var result = TwigTreeFindAncestorHelper<TView>.FindAncestors(root, obj);
            if (result != null)
            {
                return result;
            }
        }

        return new List<TwigTreeNode<TView>>();
    }

    public string PrintTree()
    {
        var strBuilder = new StringBuilder();
        foreach (var root in Roots)
        {
            PrintTree(root, strBuilder, 0);
        }

        return strBuilder.ToString();
    }

    private void PrintTree(TwigTreeNode<TView> node, StringBuilder strBuilder, int level)
    {
        strBuilder.Append(' ', level * 4);
        strBuilder.AppendLine(GetNodeString(node));

        foreach (var child in node.Children)
        {
            PrintTree(child, strBuilder, level + 1);
        }

        string GetNodeString(TwigTreeNode<TView> n)
        {
            var v = (n.View?.GetType().Name ?? "null") + $"({n.View?.GetHashCode()})";
            var vm = (n.ViewModel?.GetType().Name ?? "null") + $"({n.ViewModel?.GetHashCode()})";
            return $"{v} {vm}";
        }
    }
}
