using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jgrass.Twig.Helper;

internal class TwigTreeBuildHelper<TView>
    where TView : class
{
    private IList<TwigTreeNode<TView>> Roots { get; }

    private ITwigTreeAncestorChecker<TView> AncestorChecker { get; }

    public TwigTreeBuildHelper(
        IList<TwigTreeNode<TView>> roots,
        ITwigTreeAncestorChecker<TView> ancestorChecker
    )
    {
        Roots = roots;
        AncestorChecker = ancestorChecker;
    }

    public void Add(TwigTreeNode<TView> node)
    {
        foreach (var tree in Roots)
        {
            if (IsInTree(node, tree))
            {
                return;
            }
        }

        var inserted = false;
        foreach (var tree in Roots.ToList())
        {
            var root = Insert(node, tree);
            if (root != null)
            {
                if (root != tree)
                {
                    Roots.Remove(tree);
                    Roots.Add(root);
                }
                inserted = true;
                break;
            }
        }

        if (inserted is false)
        {
            Roots.Add(node);
        }

        while (MergeRootTree())
        {
            while (PruneRootTree()) { }
        }

        while (PruneRootTree()) { }

        PruneWeakReference();
    }

    private bool PruneRootTree()
    {
        if (Roots.Count <= 1)
        {
            return false;
        }

        for (int i = 0; i < Roots.Count - 1; i++)
        {
            var first = Roots[i];
            var second = Roots[i + 1];
            if (IsInTree(first, second))
            {
                Roots.Remove(first);
                return true;
            }

            if (IsInTree(second, first))
            {
                Roots.Remove(second);
                return true;
            }
        }
        return false;
    }

    private bool MergeRootTree()
    {
        if (Roots.Count <= 1)
        {
            return false;
        }

        for (int i = 0; i < Roots.Count - 1; i++)
        {
            var first = Roots[i];
            var second = Roots[i + 1];

            if (IsViewParent(first.View, second.View))
            {
                Insert(first, second);
                return true;
            }
            else if (IsViewParent(second.View, first.View))
            {
                Insert(second, first);
                return true;
            }
        }

        return false;
    }

    private void PruneWeakReference()
    {
        if (Roots.Count < 1)
        {
            return;
        }

        foreach (var tree in Roots.ToList())
        {
            if (tree.IsAlive is false)
            {
                Roots.Remove(tree);
            }
            else
            {
                PruneWeakReference(tree);
            }
        }
    }

    private void PruneWeakReference(TwigTreeNode<TView> node)
    {
        foreach (var child in node.Children.ToList())
        {
            if (child.IsAlive is false)
            {
                node.Children.Remove(child);
            }
            else
            {
                PruneWeakReference(child);
            }
        }
    }

    /// <summary>
    /// 将节点插入到树中
    /// </summary>
    /// <param name="node"></param>
    /// <param name="treeRoot"></param>
    /// <returns>node: 插入成功，返回新的根节点；null: 插入不成功</returns>
    private TwigTreeNode<TView>? Insert(TwigTreeNode<TView> node, TwigTreeNode<TView> treeRoot)
    {
        if (node.IsSame(treeRoot))
        {
            return treeRoot;
        }
        foreach (var child in treeRoot.Children)
        {
            if (node.IsSame(child))
            {
                return treeRoot;
            }
        }

        // 先尝试从子节点中插入
        foreach (var child in treeRoot.Children)
        {
            var root = Insert(node, child);
            if (root != null)
            {
                if (IsViewParent(root.View, treeRoot.View))
                {
                    return treeRoot;
                }
                else
                {
                    return root;
                }
            }
        }

        // 新节点 node 是 treeRoot 的子节点
        if (IsViewParent(node.View, treeRoot.View))
        {
            return InsertNode(node, treeRoot);
        }

        // 新节点 node 是 treeRoot 的父节点，在尝试向上寻找，找到嘴上的父节点

        if (IsViewParent(treeRoot.View, node.View))
        {
            var nextParent = treeRoot;
            var prevParent = treeRoot;
            while (true)
            {
                if (nextParent.Parent == null)
                {
                    if (IsViewParent(nextParent.View, node.View))
                    {
                        prevParent = nextParent;
                    }
                    break;
                }

                if (IsViewParent(nextParent.View, node.View))
                {
                    prevParent = nextParent;
                    nextParent = nextParent.Parent;
                }
            }

            return InsertNode(prevParent, node);
        }

        return null;
    }

    /// <summary>
    /// 将节点插入到指定父节点的下
    /// </summary>
    /// <param name="node"></param>
    /// <param name="parentNode"></param>
    /// <returns>新的最上层父节点</returns>
    private TwigTreeNode<TView> InsertNode(TwigTreeNode<TView> node, TwigTreeNode<TView> parentNode)
    {
        var oldParent = node.Parent;
        if (oldParent == null)
        {
            node.Parent = parentNode;
            parentNode.Children.Add(node);
            return parentNode;
        }

        // 以下是处理从中间插入的情况
        // A(oldParent) ---> C(node)  => A(oldParent) ---> B(parentNode) ---> C(node)

        oldParent.Children.Remove(node);
        var brothers = oldParent.Children.ToList();

        node.Parent = parentNode;
        oldParent.Children.Add(parentNode);
        parentNode.Parent = oldParent;
        parentNode.Children.Add(node);

        // 需要重新判断 children 与 node 的关系
        foreach (var brother in brothers)
        {
            if (IsViewParent(brother.View, parentNode.View))
            {
                oldParent.Children.Remove(brother);
                InsertNode(brother, parentNode);
            }
        }

        return oldParent;
    }

    private bool IsInTree(TwigTreeNode<TView> node, TwigTreeNode<TView> treeRoot)
    {
        if (node.IsSame(treeRoot))
        {
            return true;
        }
        foreach (var child in treeRoot.Children)
        {
            if (IsInTree(node, child))
            {
                return true;
            }
        }
        return false;
    }

    private bool IsViewParent(TView? child, TView? ancestor)
    {
        return AncestorChecker.IsAncestor(child, ancestor);
    }
}
