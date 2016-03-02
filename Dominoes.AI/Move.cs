using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominoes.AI
{
    public enum Side :int
    {
        /*
         ---0---
        | *   * |
        | *   * |
       |1|-----|3|
        | *   * |
        | *   * |
         ---2---
    */

        Top = 0,
        Left = 1,
        Bottom = 2,
        Right = 3,
        Center = 4

    }

    [DebuggerDisplay("{CurrentTile.TopEnd} || {CurrentTile.BottomEnd}")]
    public class Node
    {
        public Node TopNode { get; set; }

        public Node BottomNode { get; set; }

        public Node LeftNode { get; set; }

        public Node RigthtNode { get; set; }

        public Node ParentNode { get; set; }

        public List<KeyValuePair<Side, Node>> AvailableNeighbourNodes
        { get
            {
                var nodes = new List<KeyValuePair<Side, Node>>();
              
                nodes.Add(new KeyValuePair<Side, Node>(Side.Top, TopNode));
                nodes.Add(new KeyValuePair<Side, Node>(Side.Bottom, BottomNode));
                nodes.Add(new KeyValuePair<Side, Node>(Side.Left, LeftNode));
                nodes.Add(new KeyValuePair<Side, Node>(Side.Right, RigthtNode));
                nodes.RemoveAll(x => x.Value == null);
                return nodes;
            }
        }

        public List<KeyValuePair<Side, Node>> NeighbourNodes
        {
            get
            {
                var nodes = new List<KeyValuePair<Side, Node>>();

                nodes.Add(new KeyValuePair<Side, Node>(Side.Top, TopNode));
                nodes.Add(new KeyValuePair<Side, Node>(Side.Bottom, BottomNode));
                nodes.Add(new KeyValuePair<Side, Node>(Side.Left, LeftNode));
                nodes.Add(new KeyValuePair<Side, Node>(Side.Right, RigthtNode));
                return nodes;
            }
        }

        public void AddNewNode(Node newNode, Side tileSide)
        {
            switch (tileSide)
            {
                case Side.Top:
                    TopNode = newNode;
                    break;
                case Side.Bottom:
                    BottomNode = newNode;
                    break;
                case Side.Left:
                    LeftNode = newNode;
                    break;
                case Side.Right:
                    RigthtNode = newNode;
                    break;
            }

        }

        public Tile CurrentTile { get; set; }
    }

    public class Moves // Linked list
    {

        private Node root = null;

        public Node First { get { return root; } }

        public List<Node> Leaves
        {
            get
            {

                var leaves = new List<Node>();
                var notLeaves = new List<Node>();
                if (root == null)
                {
                    return null;
                }

                notLeaves.Add(root);

                var neighbourNodes = root.AvailableNeighbourNodes;
                if (neighbourNodes.Count == 0)
                {
                    leaves.Add(root);
                }
                else
                {
                    leaves.Add(root);
                    leaves.AddRange(LeaveSearchingRecursion(neighbourNodes, root));
                }
                return leaves;
            }
        }

        private List<Node> LeaveSearchingRecursion(List<KeyValuePair<Side, Node>> NeighbourNodes, Node parentNode)
        {
            var leaves = new List<Node>();

            foreach (var node in NeighbourNodes)
            {
                var childNeighbourNodes = node.Value.AvailableNeighbourNodes;
                childNeighbourNodes.Remove(childNeighbourNodes.Find(x => x.Value == parentNode));

                if (childNeighbourNodes.Count == 0 )
                {
                    leaves.Add(node.Value);
                } 
                else if (node.Value.CurrentTile.IsDouble() && childNeighbourNodes.Count < 3)
                {
                    leaves.Add(node.Value);
                    leaves.AddRange(LeaveSearchingRecursion(childNeighbourNodes, node.Value));
                }
                else 
                {
                    leaves.AddRange(LeaveSearchingRecursion(childNeighbourNodes, node.Value));
                }
            }
            return leaves;
        }

        public Node NewMove(Tile tile, Node parentNode, Side parentTileSide)
        {
            var childNode = new Node { CurrentTile = tile };
            Side childSide = GetMatchSide(parentNode, childNode, parentTileSide);

            parentNode.AddNewNode(childNode, parentTileSide);
            childNode.AddNewNode(parentNode, childSide);
            childNode.ParentNode = parentNode;

            return childNode;
        }

        public Side GetMatchSide(Node parentNode, Node childNode, Side parentTileSide)
        {
            var tile = childNode.CurrentTile;
            Side childSide = Side.Center;
            if (!parentNode.CurrentTile.IsDouble() && (parentTileSide == Side.Left || parentTileSide == Side.Right))
            {
                throw new Exception("Does not match");
            }
            if (childNode.CurrentTile.IsDouble())
            {
                childSide = Side.Left;
            }
            else
            {
                if (parentNode.CurrentTile.IsDouble())
                {
                    var value = parentNode.CurrentTile.TopEnd;
                    if (value == tile.TopEnd)
                    {
                        childSide = Side.Top;
                    }
                    if (value == tile.BottomEnd)
                    {
                        childSide = Side.Bottom;
                    }
                }
                else if (parentTileSide == Side.Top)
                {
                    var value = parentNode.CurrentTile.TopEnd;
                    if (value == tile.TopEnd)
                    {
                        childSide = Side.Top;
                    }
                    if (value == tile.BottomEnd)
                    {
                        childSide = Side.Bottom;
                    }
                }
                else if (parentTileSide == Side.Bottom)
                {
                    var value = parentNode.CurrentTile.BottomEnd;
                    if (value == tile.TopEnd)
                    {
                        childSide = Side.Top;
                    }
                    if (value == tile.BottomEnd)
                    {
                        childSide = Side.Bottom;
                    }
                }
                else
                {
                    throw new Exception("Does not match");
                }
            }
            return childSide;
        }

        public Node FirstMove(Tile tile)
        {
            var newNode = new Node { CurrentTile = tile };
            root = newNode;
            return newNode;
        }


    }
}
