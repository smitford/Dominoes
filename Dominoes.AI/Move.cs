using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominoes.AI
{
    public enum TileSide :int
    {
        /*
         ---0---
        | *   * |
        | *   * |
       |2|-----|3|
        | *   * |
        | *   * |
         ---1---
    */

        Top = 0,
        Bottom = 1,
        Left = 2,
        Rigt = 3
    }

    public class Node
    {
        public Node TopNode { get; set; }

        public Node BottomNode { get; set; }

        public Node LeftNode { get; set; }

        public Node RigthtNode { get; set; }

        public List<KeyValuePair<TileSide, Node>> NeighbourNodes
        { get
            {
                var nodes = new List<KeyValuePair<TileSide, Node>>();
              
                nodes.Add(new KeyValuePair<TileSide, Node>(TileSide.Top, TopNode));
                nodes.Add(new KeyValuePair<TileSide, Node>(TileSide.Bottom, BottomNode));
                nodes.Add(new KeyValuePair<TileSide, Node>(TileSide.Left, LeftNode));
                nodes.Add(new KeyValuePair<TileSide, Node>(TileSide.Rigt, RigthtNode));
                nodes.RemoveAll(x => x.Value == null);
                return nodes;
            }
        }

        public void AddNewNode(Node newNode, TileSide tileSide)
        {
            switch (tileSide)
            {
                case TileSide.Top:
                    TopNode = newNode;
                    break;
                case TileSide.Bottom:
                    BottomNode = newNode;
                    break;
                case TileSide.Left:
                    LeftNode = newNode;
                    break;
                case TileSide.Rigt:
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

                var neighbourNodes = root.NeighbourNodes;
                if (neighbourNodes.Count == 0)
                {
                    leaves.Add(root);
                }
                else
                {
                    leaves.AddRange(LeaveSearchingRecursion(neighbourNodes, root));
                }


                return leaves;
            }
        }

        private List<Node> LeaveSearchingRecursion(List<KeyValuePair<TileSide, Node>> NeighbourNodes, Node parentNode)
        {
            var leaves = new List<Node>();

            foreach (var node in NeighbourNodes)
            {
                var childNeighbourNodes = node.Value.NeighbourNodes;
                childNeighbourNodes.Remove(childNeighbourNodes.Find(x => x.Value == parentNode));
                if (childNeighbourNodes.Count == 0)
                {
                    leaves.Add(node.Value);
                }
                else
                {
                    leaves.AddRange(LeaveSearchingRecursion(childNeighbourNodes, node.Value));
                }
            }
            return leaves;
        }

        public Node NewMove(Tile tile, Node parentNode, TileSide tileSide)
        {
            var newNode = new Node { CurrentTile = tile };
            if (root == null&& parentNode==null)
            {
                root = newNode;
            }
            else
            {
                parentNode.AddNewNode(newNode, tileSide);
            }
            return newNode;
        }

        public Node FirstMove(Tile tile)
        {
            var newNode = new Node { CurrentTile = tile };
            root = newNode;
            return newNode;
        }


    }
}
