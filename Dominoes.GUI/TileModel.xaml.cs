using Dominoes.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Dominoes.GUI
{
    /// <summary>
    /// Interaction logic for TileModel.xaml
    /// </summary>
    public partial class TileModel : UserControl
    {
        private Node _currentNode;
        public Node CurrentNode {
            get
            {
                return _currentNode;
            }
            set
            {
                _currentNode = value;
                if (value != null)
                {
                    SetTileSideDots(value.CurrentTile);
                }
            }
        }

        private int _angle;

        /// <summary>
        /// Rotation angle
        /// </summary>
        public int Angle
        {
            get { return _angle; }
            set
            {
                _angle = value%4;
                this.RenderTransform = new RotateTransform((360 + _angle * 90) % 360, TileWidth / 2, TileHeight / 2);
            }
        } 

        /// <summary>
        /// Coordinates of center
        /// </summary>
        public Point Center
        {
            set
            {
                var x = (TileWidth / 2 * Math.Cos(Angle / 180 * Math.PI) - TileHeight / 2 * Math.Sin(Angle / 180 * Math.PI));
                var y = (TileWidth / 2 * Math.Sin(Angle / 180 * Math.PI) + TileHeight / 2 * Math.Cos(Angle / 180 * Math.PI));

                Margin = new Thickness(value.X - x, value.Y - y, 0, 0);
            }
            get
            {
                return new Point(
                    this.Margin.Left + (TileWidth / 2 * Math.Cos(Angle / 180 * Math.PI) - TileHeight / 2 * Math.Sin(Angle / 180 * Math.PI)),
                    this.Margin.Top + (TileWidth / 2 * Math.Sin(Angle / 180 * Math.PI) + TileHeight / 2 * Math.Cos(Angle / 180 * Math.PI))
                    );
            }
        }

        public double TileWidth { get; }
        public double TileHeight { get; }

        public TileModel()
        {
            InitializeComponent();
            TileWidth = this.Tile.Width;
            TileHeight = this.Tile.Height;
            this.RenderTransformOrigin = new Point(0, 0);
            this.VerticalAlignment = VerticalAlignment.Top;
            this.HorizontalAlignment = HorizontalAlignment.Left;
        }

        /// <summary>
        /// Side coorinates on the tile side
        /// </summary>
        /// <param name="tileSide">Current tile side</param>
        /// <returns>Point</returns>
        public Point SideCoords(Side tileSide) 
        {
            Point bias = OffsetVector(tileSide);
            return new Point(Center.X + bias.X, Center.Y + bias.Y);
        }

        public Point OffsetVector(Side tileSide) 
        {
            Point bias = new Point(0, 0);
            double x = 0;
            double y = 0;

            if (tileSide != Side.Center)
            {
                switch (tileSide)
                {
                    case Side.Top:
                        y -= TileHeight / 2;
                        break;
                    case Side.Bottom:
                        y += TileHeight / 2;
                        break;
                    case Side.Left:
                        x -= TileWidth / 2;
                        break;
                    case Side.Right:
                        x += TileWidth / 2;
                        break;
                }
                bias = Rotate(new Point(x, y), (360 + Angle * 90) % 360);
            }

            return bias;
        }

        /// <summary>
        /// Side coorinates on the cardinal direction
        /// </summary>
        /// <param name="tileSide">Current tile side</param>
        /// <returns>Point</returns>
        public Point Connector(Side tileSide)
        {
            var b = Angle;
            var side = (Side)(((int)tileSide + b) % 4);
            var point = OffsetVector(side);

            return new Point(Center.X + point.X, Center.Y + point.Y);
        }

        /// <summary>
        /// Rotates vector
        /// </summary>
        /// <param name="point"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        private Point Rotate(Point point, double angle)
        {
            var cos = Math.Cos(Math.PI * (angle / 180));
            var sin = Math.Sin(Math.PI * (angle / 180));
            return new Point(point.X * cos - point.Y * sin, point.X * sin + point.Y * cos); 
        }

        /// <summary>
        /// Sets tile dots
        /// </summary>
        /// <param name="tile"></param>
        private void SetTileSideDots(Tile tile)
        {
            dot1_1.Visibility = Visibility.Hidden;
            dot1_2.Visibility = Visibility.Hidden;
            dot1_3.Visibility = Visibility.Hidden;
            dot1_4.Visibility = Visibility.Hidden;
            dot1_5.Visibility = Visibility.Hidden;
            dot1_6.Visibility = Visibility.Hidden;
            dot1_7.Visibility = Visibility.Hidden;
            dot2_1.Visibility = Visibility.Hidden;
            dot2_2.Visibility = Visibility.Hidden;
            dot2_3.Visibility = Visibility.Hidden;
            dot2_4.Visibility = Visibility.Hidden;
            dot2_5.Visibility = Visibility.Hidden;
            dot2_6.Visibility = Visibility.Hidden;
            dot2_7.Visibility = Visibility.Hidden;

            List<string> dots = new List<string>();
            var topValue= tile.TopEnd;
            var bottomValue = tile.BottomEnd;
            switch (topValue)
            {
                case 0:
                    //do nothing
                    break;
                case 1:
                    dot1_7.Visibility = Visibility.Visible;
                    break;
                case 2:
                    dot1_1.Visibility = Visibility.Visible;
                    dot1_6.Visibility = Visibility.Visible;
                    break;
                case 3:
                    dot1_1.Visibility = Visibility.Visible;
                    dot1_7.Visibility = Visibility.Visible;
                    dot1_6.Visibility = Visibility.Visible;
                    break;
                case 4:
                    dot1_1.Visibility = Visibility.Visible;
                    dot1_3.Visibility = Visibility.Visible;
                    dot1_4.Visibility = Visibility.Visible;
                    dot1_6.Visibility = Visibility.Visible;
                    break;
                case 5:
                    dot1_1.Visibility = Visibility.Visible;
                    dot1_3.Visibility = Visibility.Visible;
                    dot1_4.Visibility = Visibility.Visible;
                    dot1_6.Visibility = Visibility.Visible;
                    dot1_7.Visibility = Visibility.Visible;
                    break;
                case 6:
                    dot1_1.Visibility = Visibility.Visible;
                    dot1_2.Visibility = Visibility.Visible;
                    dot1_3.Visibility = Visibility.Visible;
                    dot1_4.Visibility = Visibility.Visible;
                    dot1_5.Visibility = Visibility.Visible;
                    dot1_6.Visibility = Visibility.Visible;
                    break;
            }
            switch (bottomValue)
            {
                case 0:
                    //do nothing
                    break;
                case 1:
                    dot2_7.Visibility = Visibility.Visible;
                    break;
                case 2:
                    dot2_1.Visibility = Visibility.Visible;
                    dot2_6.Visibility = Visibility.Visible;
                    break;
                case 3:
                    dot2_1.Visibility = Visibility.Visible;
                    dot2_7.Visibility = Visibility.Visible;
                    dot2_6.Visibility = Visibility.Visible;
                    break;
                case 4:
                    dot2_1.Visibility = Visibility.Visible;
                    dot2_3.Visibility = Visibility.Visible;
                    dot2_4.Visibility = Visibility.Visible;
                    dot2_6.Visibility = Visibility.Visible;
                    break;
                case 5:
                    dot2_1.Visibility = Visibility.Visible;
                    dot2_3.Visibility = Visibility.Visible;
                    dot2_4.Visibility = Visibility.Visible;
                    dot2_6.Visibility = Visibility.Visible;
                    dot2_7.Visibility = Visibility.Visible;
                    break;
                case 6:
                    dot2_1.Visibility = Visibility.Visible;
                    dot2_2.Visibility = Visibility.Visible;
                    dot2_3.Visibility = Visibility.Visible;
                    dot2_4.Visibility = Visibility.Visible;
                    dot2_5.Visibility = Visibility.Visible;
                    dot2_6.Visibility = Visibility.Visible;
                    break;
            }
        }

    }
}
