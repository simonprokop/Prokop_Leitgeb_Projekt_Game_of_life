using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace CGOF
{
    public class Game
    {
        private const int boardBackgroundWidth = 105;
        private const int boardBackgroundHeight = 55;
        private const int cellSize = 10;
        private const int boarderSize = 10;
        public bool GameStarted = false;
        DispatcherTimer gameTimer;
        const int UpdateTime = 100;
        private int cellsAlive = 0;
        private Rectangle[,] boardBackground = new Rectangle[boardBackgroundHeight, boardBackgroundWidth];
        public class Cell
        {
            public int Column { get; set; }
            public int Row { get; set; }
            public bool Status { get; set; }
        }
        public void CreateBoard()
        {
            for (int i = 0; i < boardBackgroundHeight; i++)
            {
                for (int j = 0; j < boardBackgroundWidth; j++)
                {
                    if (i == 0 || i >= boardBackgroundHeight - 1 || j == 0 || j >= boardBackgroundWidth - 1)
                    {
                        Rectangle rectangle = new Rectangle
                        {
                            Width = boarderSize,
                            Height = boarderSize,
                            Stroke = Brushes.MidnightBlue, //Maroon
                            Fill = Brushes.MidnightBlue,
                        };
                        boardBackground[i, j] = rectangle; /*Předá vlastnosti okraje prostoru do prostoru boardBackground*/
                        Canvas.SetLeft(rectangle, j * boarderSize);
                        Canvas.SetTop(rectangle, i * boarderSize);
                        CGOF.MainWindow.Window.gameCanvas.Children.Add(rectangle);
                    }
                    else
                    {
                        Cell Cell = new Cell { Column = i, Row = j, Status = false };
                        Rectangle rectangle = new Rectangle
                        {
                            Width = cellSize,
                            Height = cellSize,
                            Stroke = Brushes.Black,
                            StrokeThickness = 0.3,
                            Fill = Brushes.Black,
                            Tag = Cell
                        };
                        rectangle.MouseDown += CGOF.MainWindow.Window.RightButtonDown;
                        boardBackground[i, j] = rectangle; /*Předá vlastnosti okraje prostoru do prostoru boardBackground*/
                        Canvas.SetLeft(rectangle, j * cellSize);
                        Canvas.SetTop(rectangle, i * cellSize);
                        CGOF.MainWindow.Window.gameCanvas.Children.Add(rectangle);
                    }
                }
            }
        }
        public void TimerTick(object sender, EventArgs e)
        {
            if (cellsAlive > 0)
            {
                Rules();
            }
            else
            {
                if (cellsAlive == 0 && GameStarted)
                {
                    Stop();
                }
            }
        }
        void Rules()
        {
            List<Cell> CellsChange = new List<Cell>();

            foreach (var current in boardBackground)
            {
                List<Cell> neighbours;
                if (current.Tag != null)
                {
                    var currentCell = (Cell)current.Tag; // přiřadí buňcě Temporary vlastnosti aktualní buňky 
                    neighbours = CellNeighbors(currentCell); // TemporaryCell se po průchodu funkcí CellNeighbors vrátí do proměnné neighbours
                    int cellCount = neighbours.Count(c => c.Status); // spočtení jednotlivých Statusů pro neighbours
                                                                     //1. pravidlo = méně než 2 přátelé = die
                    if (currentCell.Status == true)
                    {
                        if (cellCount < 2 || cellCount > 3) //Při splnění podmínky se buňka započítá do Listu ke změně
                        {
                            CellsChange.Add(currentCell);
                        }
                        if (cellCount == 2 || cellCount == 3)
                        {
                            continue;
                        }
                    }
                    //2. pravidlo = pokud má buňka přesně 3 sousedy, tak oživne.
                    else
                    {
                        if (cellCount == 3 && currentCell.Status != true)
                        {
                            CellsChange.Add(currentCell);
                        }
                    }
                }
            }
            if (CellsChange.Count != 0)
            {
                ChangeCellsFromList(CellsChange);
            }
        }
        List<Cell> CellNeighbors(Cell cell)
        {
            List<Cell> neighborList = new List<Cell>();
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    var neighborCell = boardBackground[cell.Column + i, cell.Row + j];
                    if (neighborCell.Tag != null)
                    {
                        var currentNeighborCell = (Cell)neighborCell.Tag;
                        if (currentNeighborCell.Row == cell.Row && currentNeighborCell.Column == cell.Column)
                        {
                            continue;
                        }
                        else
                        {
                            if (currentNeighborCell.Status == true)
                            {
                                neighborList.Add(currentNeighborCell);
                            }
                        }
                    }
                }
            }
            return neighborList;
        }
        void ChangeCellsFromList(List<Cell> CellsChange) //Metoda pro čtení listu buněk ke změně (CellChange)
        {
            foreach (var cell in CellsChange)
            {
                ChangeCellStatus(cell);
            }
        }
        public void ChangeCellStatus(Cell cell)
        {
            if (!cell.Status)
            {
                cell.Status = true;
                //Board.boardBackground[cell.Column, cell.Row].Fill = Brushes.Black;
                cellsAlive++;
                boardBackground[cell.Column, cell.Row].Fill = Brushes.Aqua;
            }
            else
            {
                cell.Status = false;
                cellsAlive--;
                boardBackground[cell.Column, cell.Row].Fill = Brushes.Black;

                if (cellsAlive == 0 && GameStarted == true)
                { Stop(); }
            }
        }
        public void Start()
        {
            if (GameStarted == false)
            {
                gameTimer = new DispatcherTimer
                {
                    Interval = new TimeSpan(0, 0, 0, 0, UpdateTime)
                };
                gameTimer.Tick += TimerTick;
                gameTimer.Start();

                GameStarted = true;

                CGOF.MainWindow.Window.Play.IsEnabled = false;
                CGOF.MainWindow.Window.Stop.IsEnabled = true;
                CGOF.MainWindow.Window.Reset.IsEnabled = false;
            }
            /*DispatcherTimer jsou v podstatě hodiny, které si nastaví čas a interval v kterém bude updatevat pole Canvas.
			 UpdateTime je čas vykreslení nového updatu v milisekundach*/
        }
        public void Stop()
        {
            if (GameStarted == true)
            {
                gameTimer.Stop();
                GameStarted = false;
                CGOF.MainWindow.Window.Play.IsEnabled = true;
                CGOF.MainWindow.Window.Stop.IsEnabled = false;
                CGOF.MainWindow.Window.Reset.IsEnabled = true;
            }
        }
        public void Reset()
        {
            CGOF.MainWindow.Window.Stop.IsEnabled = false;
            CGOF.MainWindow.Window.Play.IsEnabled = true;
            GameStarted = false;
            if (!GameStarted)
            {
                foreach (var Field in boardBackground) /*Pro každé políčko v Canvasu*/
                {
                    if (Field.Tag != null)
                    {
                        Cell field = (Cell)Field.Tag;
                        if (field.Status)
                        {
                            ChangeCellStatus(field);
                        }
                    }
                    /*Pro každé nezmáčknuté políčko se Tag změní na Cells a Status se ve funkci Change změní na Cells */
                }
            }
        }
        public void Osc1(Cell cell)
        {
            List<Cell> osc1List = new List<Cell>();
            osc1List.Add(cell);
            for (int i = -1; i <= 1; i++)
            {
                var neighborCell = boardBackground[cell.Column + i, cell.Row];
                var neighborCellTag = (Cell)neighborCell.Tag;
                if (neighborCell.Tag != null && neighborCellTag.Status == false)
                {
                    osc1List.Add(neighborCellTag);
                }
            }
            ChangeCellsFromList(osc1List);
        }
        public void Osc2(Cell cell)
        {
            List<Cell> osc2List = new List<Cell>();
            osc2List.Add(cell);
            for (int i = 0; i <= 1; i++)
            {
                for (int j = 0; j <= 3; j++)
                {
                    var neighborCell = boardBackground[cell.Column + i, cell.Row + j];
                    var neighborCellTag = (Cell)neighborCell.Tag;
                    if (neighborCell.Tag != null && neighborCellTag.Status == false)
                    {
                        if ((i != 0 && j != 0) || (i != 1 && j != 3))
                        {
                            osc2List.Add(neighborCellTag);
                        }
                    }
                }
            }
            ChangeCellsFromList(osc2List);
        }
        public void Sta1(Cell cell)
        {
            List<Cell> sta1List = new List<Cell>();
            sta1List.Add(cell);
            for (int i = 0; i <= 1; i++)
            {
                for (int j = 0; j <= 1; j++)
                {
                    var neighborCell = boardBackground[cell.Column + i, cell.Row + j];
                    var neighborCellTag = (Cell)neighborCell.Tag;
                    if (neighborCell.Tag != null && neighborCellTag.Status == false)
                    {
                        sta1List.Add(neighborCellTag);
                    }
                }
            }
            ChangeCellsFromList(sta1List);
        }
        public void Sta2(Cell cell)
        {
            List<Cell> sta2List = new List<Cell>();
            for (int i = 0; i <= 2; i++)
            {

                for (int j = 0; j <= 2; j++)
                {
                    var neighborCell = boardBackground[cell.Column + i, cell.Row + j];
                    var neighborCellTag = (Cell)neighborCell.Tag;
                    if (neighborCell.Tag != null && neighborCellTag.Status == false)
                    {
                        if ((i == 0 && j == 0) || (i == 0 && j == 2) || (i == 1 && j == 1) || (i == 2 && j == 0) || (i == 2 && j == 2)) { }
                        else
                        {
                            sta2List.Add(neighborCellTag);
                        }     
                    }
                }
            }
            ChangeCellsFromList(sta2List);
        }
        public void Randomize()
        {
            List<Cell> randomizedCells = new List<Cell>();
            randomizedCells.Clear();
            Random rand = new Random();
            if (GameStarted)
            {
                Stop();
            }
            Reset();
            foreach (var cell in boardBackground)
            {
                var CurrentRandomCell = (Cell)cell.Tag;
                if (cell.Tag != null)
                {
                    bool randomResult = rand.Next(5) == 0;
                    if (randomResult == true)
                    {
                        randomizedCells.Add(CurrentRandomCell);
                    }
                    else { }
                }
            }
            if (randomizedCells.Count != 0)
            {
                ChangeCellsFromList(randomizedCells);
            }
        }
    }
}