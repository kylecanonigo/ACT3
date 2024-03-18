using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
// Kyle A. Canonigo | BSCS 3 F1 | ACT 3 | BFS
namespace Activity3
{
    public partial class Form1 : Form
    {
        int side;
        int numX = 20;
        int numY = 16;
        Square[,] grid;

        Square highlighted = new Square();
        Square selected = new Square(0, 0);

        //int mode = 0;
        LinkedList<PathNode> tree;
        Queue<PathNode> search;
        int exploreLimit;
        int exploreCount;

        PathNode goalNode;

        bool changedSquare = false;
        int currentX, currentY;

        public Form1()
        {
            InitializeComponent();

            grid = new Square[numX, numY];
            exploreLimit = numX * numY;
            resetGrid();
            side = Convert.ToInt16(pictureBox1.Width / numX);

            tree = new LinkedList<PathNode>();
            tree.AddFirst(new PathNode(selected));
            search = new Queue<PathNode>();
            search.Enqueue(tree.First.Value);
        }

        public void resetGrid()
        {
            for (int x = 0; x < numX; x++)
            {
                for (int y = 0; y < numY; y++)
                {
                    grid[x, y] = new Square(x, y);
                }
            }
        }

        public bool isValid(int x, int y)
        {
            if (x >= 0 && x < numX && y >= 0 && y < numY)
                return true;
            else
                return false;
        }

        private bool hasArrived(PathNode target)
        {
            return target.location.X == highlighted.X && target.location.Y == highlighted.Y;
        }

        private void exploreNode(PathNode target)
        {
            // UP DOWN LEFT RIGHT
            Point UP = new Point(target.location.X, target.location.Y - 1);
            Point DOWN = new Point(target.location.X, target.location.Y + 1);
            Point LEFT = new Point(target.location.X - 1, target.location.Y);
            Point RIGHT = new Point(target.location.X + 1, target.location.Y);

            Point[] direction = { UP, DOWN, LEFT, RIGHT };

            for (int i = 0; i < 4; i++)
            {
                if (isValid(direction[i].X, direction[i].Y))
                {
                    Square check = grid[direction[i].X, direction[i].Y];
                    if (check.passable && !check.explored)
                    {
                        tree.AddLast(new PathNode(grid[direction[i].X, direction[i].Y], target));
                        search.Enqueue(tree.Last.Value);
                        grid[direction[i].X, direction[i].Y].explored = true;
                    }
                }
            }
            exploreCount++;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            highlighted.X = e.X / side;
            highlighted.Y = e.Y / side;

            tree = new LinkedList<PathNode>();
            tree.AddFirst(new PathNode(selected));
            search = new Queue<PathNode>();
            search.Enqueue(tree.First.Value);
            bool foundGoal = false;

            if (!changedSquare)
            {
                currentX = highlighted.X;
                currentY = highlighted.Y;
                changedSquare = true;

                while (foundGoal == false && search.Count > 0)
                {
                    PathNode target = (PathNode)search.Dequeue();
                    if (target != null)
                    {
                        foundGoal = hasArrived(target);
                        if (foundGoal)
                        {
                            foundGoal = true;
                            goalNode = target;
                            lblPath.Text = "PathFound";
                            tree.Clear();
                            tree.AddFirst(new PathNode(selected));
                            search.Clear();
                            search.Enqueue(tree.First.Value);
                            resetGrid();
                        }
                        else
                        {
                            exploreNode(target);
                            //listBox1.Items = search;
                        }
                    }
                }
                pictureBox1.Refresh();
            }

            if (currentX != highlighted.X || currentY != highlighted.Y)
            {
                changedSquare = false;
                listBox1.Items.Clear();
            }

            lblMouse.Text = "X: " + e.X + " Y: " + e.Y;
            lblSquare.Text = "( " + highlighted.X + " , " + highlighted.Y + " ) ";
        }


        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            for (int x = 0; x < numX; x++)
            {
                for (int y = 0; y < numY; y++)
                {
                    Rectangle s = new Rectangle(x * side, y * side, side, side);
                    if (x == selected.X && y == selected.Y)
                        e.Graphics.FillRectangle(Brushes.SlateBlue, s);
                    else if (x == highlighted.X && y == highlighted.Y)
                        e.Graphics.FillRectangle(Brushes.SkyBlue, s);
                    else
                        e.Graphics.FillRectangle(Brushes.Silver, s);
                    e.Graphics.DrawRectangle(Pens.Gray, s);
                }
            }

            while (goalNode != null)
            {
                if (goalNode.location.X != selected.X || goalNode.location.Y != selected.Y)
                {
                    Rectangle s = new Rectangle(goalNode.location.X * side,
                                            goalNode.location.Y * side,
                                            side, side);
                    e.Graphics.FillRectangle(Brushes.DarkBlue, s);
                }

                listBox1.Items.Add("("+goalNode.location.X.ToString() + "," + goalNode.location.Y.ToString()+")");

                goalNode = goalNode.origin;
            }
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            selected.X = e.X / side;
            selected.Y = e.Y / side;

            //pictureBox1.Refresh();

            lblSelected.Text = "( " + selected.X + " , " + selected.Y + " ) ";
            listBox1.Items.Clear();
        }
    }
}
