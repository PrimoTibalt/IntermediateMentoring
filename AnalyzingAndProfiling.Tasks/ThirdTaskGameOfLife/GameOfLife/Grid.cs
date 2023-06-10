using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GameOfLife
{
    class Grid
    {
        private int SizeX;
        private int SizeY;
        private Cell[,] cells;
        private Cell[,] nextGenerationCells;
        private static Random rnd;
        private Canvas drawCanvas;
        private Ellipse[,] cellsVisuals;

        public Grid(Canvas c)
        {
            drawCanvas = c;
            rnd = new Random();
            SizeX = (int)(c.Width / 5);
            SizeY = (int)(c.Height / 5);
            cells = new Cell[SizeX, SizeY];
            nextGenerationCells = new Cell[SizeX, SizeY];
            cellsVisuals = new Ellipse[SizeX, SizeY];

            for (int i = 0; i < SizeX; i++)
                for (int j = 0; j < SizeY; j++)
                {
                    cells[i, j] = new Cell(i, j, 0, false);
                    nextGenerationCells[i, j] = new Cell(i, j, 0, false);
                }

            SetRandomPattern();
            InitCellsVisuals();
            UpdateGraphics();
        }

        public void Clear()
        {
            for (int i = 0; i < SizeX; i++)
                for (int j = 0; j < SizeY; j++)
                {
                    // This will prevent cells from ending up in GC.
                    var cell = cells[i, j];
                    cell.Age = 0;
                    cell.IsAlive = false;
                    var nextGenCell = nextGenerationCells[i, j];
                    nextGenCell.Age = 0;
                    nextGenCell.IsAlive = false;
                    cellsVisuals[i, j].Fill = Brushes.Gray;
                }
        }


        void MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                // Would only be needed on pressed state.
                var cellVisual = sender as Ellipse;

                int i = (int)cellVisual.Margin.Left / 5;
                int j = (int)cellVisual.Margin.Top / 5;
                if (!cells[i, j].IsAlive)
                {
                    cells[i, j].IsAlive = true;
                    cells[i, j].Age = 0;
                    cellVisual.Fill = Brushes.White;
                }
            }
        }

        public void UpdateGraphics()
        {
            for (int i = 0; i < SizeX; i++)
                for (int j = 0; j < SizeY; j++)
                    cellsVisuals[i, j].Fill = cells[i, j].IsAlive
                                                  ? (cells[i, j].Age < 2 ? Brushes.White : Brushes.DarkGray)
                                                  : Brushes.Gray;
        }

        public void InitCellsVisuals()
        {
            for (int i = 0; i < SizeX; i++)
                for (int j = 0; j < SizeY; j++)
                {
                    cellsVisuals[i, j] = new Ellipse();
                    cellsVisuals[i, j].Width = cellsVisuals[i, j].Height = 5;
                    double left = cells[i, j].PositionX;
                    double top = cells[i, j].PositionY;
                    cellsVisuals[i, j].Margin = new Thickness(left, top, 0, 0);
                    cellsVisuals[i, j].Fill = Brushes.Gray;
                    drawCanvas.Children.Add(cellsVisuals[i, j]);
                    cellsVisuals[i, j].MouseMove += MouseMove;
                }

            UpdateGraphics();

        }


        public static bool GetRandomBoolean()
        {
            return rnd.NextDouble() > 0.8;
        }

        public void SetRandomPattern()
        {
            for (int i = 0; i < SizeX; i++)
                for (int j = 0; j < SizeY; j++)
                    cells[i, j].IsAlive = GetRandomBoolean();
        }

        public void UpdateToNextGeneration()
        {
            for (int i = 0; i < SizeX; i++)
                for (int j = 0; j < SizeY; j++)
                {
                    cells[i, j].IsAlive = nextGenerationCells[i, j].IsAlive;
                    cells[i, j].Age = nextGenerationCells[i, j].Age;
                }

            UpdateGraphics();
        }


        public void Update()
        {
            for (int i = 0; i < SizeX; i++)
                for (int j = 0; j < SizeY; j++)
                    CalculateNextGeneration(i, j, ref nextGenerationCells[i, j]);
            
            UpdateToNextGeneration();
        }

        // Creating new instances in such magnitude could affect performance. Added reference.
        public void CalculateNextGeneration(int row, int column, ref Cell nextGenCell)    // UNOPTIMIZED
        {
            bool alive = cells[row, column].IsAlive;
            int count = CountNeighbors(row, column);

            if (alive && count < 2)
            {
                nextGenCell.Age = 0;
                nextGenCell.IsAlive = false;
                return;
            }

            if (alive && (count == 2 || count == 3))
            {
                cells[row, column].Age++;
                nextGenCell.Age = cells[row, column].Age;
                nextGenCell.IsAlive = true;
                return;
            }

            if (alive && count > 3)
            {
                nextGenCell.Age = 0;
                nextGenCell.IsAlive = false;
                return;
            }

            if (!alive && count == 3)
            {
                nextGenCell.Age = 0;
                nextGenCell.IsAlive = true;
                return;
            }

            nextGenCell.Age = 0;
            nextGenCell.IsAlive = false;
        }

        public void CalculateNextGeneration(int row, int column, ref bool isAlive, ref int age)     // OPTIMIZED
        {
            isAlive = cells[row, column].IsAlive;
            age = cells[row, column].Age;

            int count = CountNeighbors(row, column);

            if (isAlive && count < 2)
            {
                isAlive = false;
                age = 0;
            }

            if (isAlive && (count == 2 || count == 3))
            {
                cells[row, column].Age++;
                isAlive = true;
                age = cells[row, column].Age;
            }

            if (isAlive && count > 3)
            {
                isAlive = false;
                age = 0;
            }

            if (!isAlive && count == 3)
            {
                isAlive = true;
                age = 0;
            }
        }

        public int CountNeighbors(int i, int j)
        {
            // For clarity mostly.
            int count = 0;
            bool isNotNearRightBorder = i != SizeX - 1;
            bool isNotNearCeiling = j != SizeY - 1;

            if (isNotNearRightBorder && cells[i + 1, j].IsAlive) count++;
            if (isNotNearRightBorder && isNotNearCeiling && cells[i + 1, j + 1].IsAlive) count++;
            if (isNotNearCeiling && cells[i, j + 1].IsAlive) count++;
            if (i != 0 && isNotNearCeiling && cells[i - 1, j + 1].IsAlive) count++;
            if (i != 0 && cells[i - 1, j].IsAlive) count++;
            if (i != 0 && j != 0 && cells[i - 1, j - 1].IsAlive) count++;
            if (j != 0 && cells[i, j - 1].IsAlive) count++;
            if (isNotNearRightBorder && j != 0 && cells[i + 1, j - 1].IsAlive) count++;

            return count;
        }
    }
}