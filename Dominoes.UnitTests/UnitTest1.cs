using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dominoes.DB;
using Dominoes.AI;
using System.Collections.Generic;

namespace Dominoes.UnitTests
{
    [TestClass]
    public class DbTests
    {
        [TestMethod]
        public void DbWorks()
        {
            var dbconn = new DbConnector();
            var result = dbconn.Add("Test Name", true);
            var history = dbconn.GetHistory();
            bool deleted = dbconn.Remove(result);
            Assert.IsTrue(history.Contains(result) && deleted);
        }
    }

    [TestClass]
    public class AiTests
    {
        [TestMethod]
        public void DominoesDataStructureTest()
        {
            var tiles = GenerateTiles();
            var moves = new Moves();
            var tile = new Tile();

            tile = tiles[new Random().Next(tiles.Count)];
            tiles.Remove(tile);
            var root = moves.FirstMove(tile);

            tile = tiles[new Random().Next(tiles.Count)];
            tiles.Remove(tile);
            var node_0 = moves.NewMove(tile, root, Side.Top);

            tile = tiles[new Random().Next(tiles.Count)];
            tiles.Remove(tile);
            var node_1 = moves.NewMove(tile, root, Side.Bottom);

            tile = tiles[new Random().Next(tiles.Count)];
            tiles.Remove(tile);
            var node_2 = moves.NewMove(tile, root, Side.Bottom);

            tile = tiles[new Random().Next(tiles.Count)];
            tiles.Remove(tile);
            var node_0_0 = moves.NewMove(tile, node_0, Side.Top);

            tile = tiles[new Random().Next(tiles.Count)];
            tiles.Remove(tile);
            var node_0_2 = moves.NewMove(tile, node_0, Side.Bottom);

            tile = tiles[new Random().Next(tiles.Count)];
            tiles.Remove(tile);
            var node_2_3 = moves.NewMove(tile, node_2, Side.Right);

            var leaves = moves.Leaves;
            Assert.IsTrue(leaves.Contains(node_1) && leaves.Contains(node_0_0) && leaves.Contains(node_0_2) && leaves.Contains(node_2_3) && leaves.Count == 4);
        }

        private List<Tile> GenerateTiles()
        {
            var tiles = new List<Tile>();
            for (int i = 0; i < 6; i++)
            {
                for (int j = i; j < 6; j++)
                {
                    var tile = new Tile(i,j);
                    tiles.Add(tile);
                }
            }
            return tiles;
        }

        [TestMethod]
        public void TilesComparerTest()
        {
            List<Tile> tiles = new List<Tile>();
            tiles.Add(new Tile(4, 4));
            tiles.Add(new Tile(4, 6));
            tiles.Add(new Tile(6, 2));
            tiles.Add(new Tile(1, 2));
            tiles.Add(new Tile(3, 2));
            tiles.Add(new Tile(1, 1));
            var min = Tile.MinimalTile(tiles);
            var st1 = min == tiles[5];
            var st2 = min < tiles[0];
            Assert.IsTrue(st1 && st2);
        }
    }
}
