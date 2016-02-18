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
            var node_0 = moves.NewMove(tile, root, TileSide.Top);

            tile = tiles[new Random().Next(tiles.Count)];
            tiles.Remove(tile);
            var node_1 = moves.NewMove(tile, root, TileSide.Bottom);

            tile = tiles[new Random().Next(tiles.Count)];
            tiles.Remove(tile);
            var node_2 = moves.NewMove(tile, root, TileSide.Left);

            tile = tiles[new Random().Next(tiles.Count)];
            tiles.Remove(tile);
            var node_0_0 = moves.NewMove(tile, node_0, TileSide.Top);

            tile = tiles[new Random().Next(tiles.Count)];
            tiles.Remove(tile);
            var node_0_2 = moves.NewMove(tile, node_0, TileSide.Left);

            tile = tiles[new Random().Next(tiles.Count)];
            tiles.Remove(tile);
            var node_2_3 = moves.NewMove(tile, node_2, TileSide.Rigt);

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
    }
}
