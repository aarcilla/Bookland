using System.Collections.Generic;

namespace Bookland.Data_Structures
{
    /// <summary>
    /// A node that is to be a part of a tree data structure. A node includes its intended data,
    /// and a list of its lower-tier children nodes.
    /// </summary>
    /// <typeparam name="T">Type that the tree will be based on.</typeparam>
    public class TreeNode<T>
    {
        public List<TreeNode<T>> Children { get; private set; }
        public T Data { get; private set; }

        public TreeNode(T data)
        {
            this.Data = data;
            this.Children = new List<TreeNode<T>>();
        }

        /// <summary>
        /// Add a child node to this node.
        /// </summary>
        /// <param name="childData">The child's data within a node.</param>
        public void AddChild(TreeNode<T> childData)
        {
            Children.Add(childData);
        }

        /// <summary>
        /// Deletes a specified node from the tree structure, as well as any descendant nodes.
        /// </summary>
        /// <param name="data">The data of the node to be deleted.</param>
        /// <returns>A Boolean specifying whether the node was successfully found & deleted from the tree.</returns>
        public bool DeleteNode(T data)
        {
            return FindNodeAndDelete(this, data);
        }

        /// <summary>
        /// Traverse through a tree to find and delete a specific tree node.
        /// </summary>
        /// <param name="currentNode">The current tree node to be checked for deletion.</param>
        /// <param name="dataToBeDeleted">The data of the node to be deleted.</param>
        /// <returns>A Boolean specifying whether the node was successfully found & deleted from the tree.</returns>
        private bool FindNodeAndDelete(TreeNode<T> currentNode, T dataToBeDeleted)
        {
            List<TreeNode<T>> childNodes = currentNode.Children;

            bool deleted = false;

            foreach (TreeNode<T> item in childNodes)
            {
                if (item.Data.Equals(dataToBeDeleted))
                {
                    deleted = currentNode.Children.Remove(item);
                    break;
                }
                else
                {
                    deleted = deleted || FindNodeAndDelete(item, dataToBeDeleted);
                }
            }

            return deleted;
        }


        /// <summary>
        /// Retrieve all items in the tree as a flattened list, in the order of a depth-first pre-order traversal.
        /// </summary>
        /// <returns>All tree items in a list.</returns>
        public List<T> ToDepthFirstPreOrderTraversalList()
        {
            return ToToDepthFirstPreOrderTraversalList(this);
        }

        /// <summary>
        /// Retrieve list of items in the order of a depth-first pre-order tree traversal.
        /// </summary>
        /// <param name="node">Current node.</param>
        /// <returns>List of items in depth-first pre-order traversal order.</returns>
        private List<T> ToToDepthFirstPreOrderTraversalList(TreeNode<T> node)
        {
            List<T> itemsOrdered = new List<T>();
            itemsOrdered.Add(node.Data);

            List<TreeNode<T>> childNodes = node.Children;

            foreach (TreeNode<T> child in childNodes)
            {
                List<T> childItems = ToToDepthFirstPreOrderTraversalList(child);

                foreach (T item in childItems)
                {
                    itemsOrdered.Add(item);
                }
            }

            return itemsOrdered;
        }

        /// <summary>
        /// Retrieve the number of nodes in this tree.
        /// </summary>
        /// <returns>The number of nodes in this tree.</returns>
        public int Count()
        {
            return GetChildrenCount(this);
        }

        /// <summary>
        /// Retrieve the number of nodes in a given tree.
        /// </summary>
        /// <param name="node">The root node of a tree.</param>
        /// <returns>The number of nodes in the specified tree.</returns>
        private int GetChildrenCount(TreeNode<T> node)
        {
            int count = 1;

            List<TreeNode<T>> childrenNodes = node.Children;
            if (childrenNodes != null)
            {
                foreach (TreeNode<T> item in childrenNodes)
                {
                    count += GetChildrenCount(item);
                }
            }

            return count;
        }
    }
}