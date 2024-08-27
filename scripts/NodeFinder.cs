using Godot;
using System;
using System.Collections.Generic;

public class NodeFinder
{
    // Function to find nodes of a specific type in the scene tree
    public static List<T> FindNodesOfType<T>(Node root) where T : Node
    {
        List<T> nodes = new List<T>();
        FindNodesOfTypeInTree(root, nodes);
        return nodes;
    }

    // Recursive function to search for nodes of a specific type
    private static void FindNodesOfTypeInTree<T>(Node node, List<T> result) where T : Node
    {
        if (node is T)
        {
            result.Add((T)node);
        }

        foreach (Node child in node.GetChildren())
        {
            FindNodesOfTypeInTree(child, result);
        }
    }
}