using System.Diagnostics;

namespace PACMAN;

internal class Program {
    static Tile[][]? Tiles { get; set; }
    static TimeSpan WantedFrameTime = TimeSpan.FromSeconds(0.15);
    static int chargeTime = 15; // how many frames the charged state will remain after taking effect
    static void Main(string[] args) {

        Console.WindowHeight = 40;
        Console.CursorVisible = false;

        SetupBoard();
        GameLoop();



    }
    static int Charge = 0;
    static int Score = 0;
    static void GameLoop() {

        bool running = true;


        while (running) {


            UpdatePlayer();
            UpdateGhost("Blinky");

            WriteStatus();
            TimeDelay();
        }

    }
    static void UpdateGhost(string name) {
        if (Tiles == null) return;
        Ghost ghost;
        switch (name.ToLower()) {
            case "blinky": ghost = Blinky; break;
            case "pinky": ghost = Pinky; break;
            case "inky": ghost = Inky; break;
            case "clyde": ghost = Clyde; break;
            default: return;
        }

        List<Tile> posiblePos = [
            Tiles[ghost.Position.I + 1][ghost.Position.J],
            Tiles[ghost.Position.I][ghost.Position.J + 1],
            Tiles[ghost.Position.I - 1][ghost.Position.J],
            Tiles[ghost.Position.I][ghost.Position.J - 1],
        ];

        switch (ghost.Direction) {
            case Direction.North: posiblePos.RemoveAt(0); break;
            case Direction.East: posiblePos.RemoveAt(3); break;
            case Direction.South: posiblePos.RemoveAt(2); break;
            case Direction.West: posiblePos.RemoveAt(1); break;
            default: break;
        }

        for (int i = posiblePos.Count - 1; i >= 0; i--) {
            if (posiblePos[i].Type is Tile.TileType.Wall or Tile.TileType.Teleport)
                posiblePos.RemoveAt(i);
        }

        Position newGhostPos;
        if (posiblePos.Count > 1)
            switch (name.ToLower()) {
                case "blinky":

                    Tile shortest = posiblePos[0];
                    for (int i = 1; i < posiblePos.Count; i++) {

                    }
                    break;
                case "pinky":

                    break;
                case "inky":

                    break;
                case "clyde":

                    break;
                default: return;
            }
        else
            newGhostPos = posiblePos[0].Position;
    }
    static void WriteStatus() {
        int width = Tiles[0].Length + 5;
        int height = 3;

        Console.SetCursorPosition(width, height++);
        Console.Write("Score: " + Score);
        Console.SetCursorPosition(width, height++);
        Console.Write("IsCharged: " + (Charge > 0).ToString() + "    ");
        Console.SetCursorPosition(width, height++);
        Console.Write("FoodLeft: " + FoodLeft + "    ");

    }
    static ConsoleKeyInfo Key { get; set; }
    static Direction Direction { get; set; }
    public static Position Player { get; set; }
    public static int FoodLeft { get; set; }
    static void UpdatePlayer(bool called = false) {
        if (Console.KeyAvailable) {
            Key = Console.ReadKey(true);
            while (Console.KeyAvailable)
                Console.ReadKey(true);
        }

        Direction previousDirection = Direction;

        switch (Key.Key) {
            case ConsoleKey.W or ConsoleKey.UpArrow: Direction = Direction.North; break;
            case ConsoleKey.A or ConsoleKey.LeftArrow: Direction = Direction.West; break;
            case ConsoleKey.S or ConsoleKey.DownArrow: Direction = Direction.South; break;
            case ConsoleKey.D or ConsoleKey.RightArrow: Direction = Direction.East; break;
            default: break;
        }

        Position newPosTile = new(Player.I, Player.J);
        switch (Direction) {
            case Direction.North: newPosTile.I += -1; break;
            case Direction.East: newPosTile.J += 1; break;
            case Direction.South: newPosTile.I += 1; break;
            case Direction.West: newPosTile.J += -1; break;
            default: break;
        }

        bool change = false;
        if (newPosTile.I >= 0 && newPosTile.I < Tiles.Length && newPosTile.J >= 0 && newPosTile.J < Tiles[0].Length)
            switch (Tiles[newPosTile.I][newPosTile.J].Type) {
                case Tile.TileType.Wall:
                    break;
                case Tile.TileType.Teleport:
                    newPosTile = TeleportHandler(newPosTile); change = true;
                    break;
                case Tile.TileType.Charge:
                    change = true; Charge = chargeTime; Score += 50; FoodLeft--;
                    break;
                case Tile.TileType.Food:
                    change = true; Score += 10; FoodLeft--;
                    break;
                case Tile.TileType.Empty:
                    change = true;
                    break;
                case Tile.TileType.Start:
                    change = true;
                    break;
                default:
                    break;
            }
        if (change) {
            UpdateTile(Player);
            Player = newPosTile;
            UpdateTile(Player, true);
        }

        if (Charge > 0) Charge--;
        if (FoodLeft == 0) SetupBoard();

    }
    public static List<Position>? TeleportPostions { get; set; }
    static Position TeleportHandler(Position position) {
        if (TeleportPostions == null) throw new ArgumentNullException(nameof(TeleportPostions));

        Random rnd = new Random();

        switch (TeleportPostions.Count) {
            case 2:
                return TeleportPostions[position.Equals(TeleportPostions[1]) ? 0 : 1];
            default:
                List<Position> tempTeleport = [.. TeleportPostions];

                for (int i = 0; i < tempTeleport.Count; i++)
                    if (tempTeleport[i].Equals(position)) {
                        tempTeleport.RemoveAt(i);
                        break;
                    }

                return tempTeleport[rnd.Next(tempTeleport.Count)];
        }
    }
    static void UpdateTile(Position position, bool player = false) {
        Tile tile = Tiles[position.I][position.J];
        if (tile.Type is Tile.TileType.Food or Tile.TileType.Charge or Tile.TileType.Start) {
            tile.Type = Tile.TileType.Empty;
            tile.Art = ' ';
        }
        Console.SetCursorPosition(position.J, position.I);

        Console.Write(player ? "Ö" : tile.Art);
    }

    public static Ghost Blinky { get; set; }
    public static Ghost Pinky { get; set; }
    public static Ghost Inky { get; set; }
    public static Ghost Clyde { get; set; }
    static void SetupBoard() {
        string[] boardInfo = Assets.DrawMainBoard() ?? throw new ArgumentException("board call failed");

        if (TeleportPostions != null) TeleportPostions = null;
        TeleportPostions = new List<Position>();

        Tiles = new Tile[boardInfo.Length][];
        for (int i = 0; i < Tiles.Length; i++) {
            Tiles[i] = new Tile[boardInfo[i].Length];
            for (int j = 0; j < Tiles[i].Length; j++) {
                Tiles[i][j] = new Tile(boardInfo[i][j], new Position(i, j));
            }
        }

        Console.SetCursorPosition(0, 0);
        for (int i = 0; i < Tiles.Length; i++) {
            for (int j = 0; j < Tiles[i].Length; j++) {
                Tile tile = Tiles[i][j];
                switch (tile.Type) {
                    case Tile.TileType.Wall:
                        Console.ForegroundColor = ConsoleColor.DarkBlue;
                        break;
                    case Tile.TileType.Teleport:
                        break;
                    case Tile.TileType.Charge:
                    case Tile.TileType.Food:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        break;
                    case Tile.TileType.Empty:
                        break;
                    default:
                        break;
                }
                Console.Write(Tiles[i][j].Art);
            }
            Console.WriteLine();
        }
        Console.ForegroundColor = ConsoleColor.White;
    }



    static private DateTime timeStamp = DateTime.Now;
    static void TimeDelay() {
        TimeSpan waitFor = WantedFrameTime - (DateTime.Now - timeStamp);
        if (waitFor > TimeSpan.Zero) Thread.Sleep(waitFor);
        timeStamp = DateTime.Now;
    }
}
public class Tile {
    public enum TileType {
        Wall,
        Teleport,
        Charge,
        Food,
        Empty,
        Start,
    }
    public TileType Type { get; set; }
    public char Art { get; set; }
    public List<Ghost> ContainedGhosts { get; set; }
    public Position Position { get; }
    public Tile(char seed, Position position) {
        Art = seed;
        ContainedGhosts = new List<Ghost>();
        Position = position;
        switch (seed) {
            case '█': Type = TileType.Wall; break;
            case '*': Type = TileType.Food; Program.FoodLeft++; break;
            case '+': Type = TileType.Charge; Program.FoodLeft++; break;
            case 't':
                Type = TileType.Teleport; Art = ' ';
                if (Program.TeleportPostions != null)
                    Program.TeleportPostions.Add(position);
                else
                    Debug.WriteLine("Teleport postions unexpected null");
                break;
            case ' ': Type = TileType.Empty; break;
            case 's': Type = TileType.Start; Art = 'Ö'; Program.Player = position; break;

            case 'b': Type = TileType.Empty; Art = ' '; Program.Blinky = new Ghost("Blinky", position); ContainedGhosts.Add(Program.Blinky); break;
            case 'p': Type = TileType.Empty; Art = ' '; Program.Pinky = new Ghost("Pinky", position); ContainedGhosts.Add(Program.Pinky); break;
            case 'i': Type = TileType.Empty; Art = ' '; Program.Inky = new Ghost("Inky", position); ContainedGhosts.Add(Program.Inky); break;
            case 'c': Type = TileType.Empty; Art = ' '; Program.Clyde = new Ghost("Clyde", position); ContainedGhosts.Add(Program.Clyde); break;

            default: Type = TileType.Empty; Debug.WriteLine("unexpected empty call"); break;
        }
    }
}
public class Position {
    public int I { get; set; }
    public int J { get; set; }

    public Position(int i, int j) {
        I = i;
        J = j;
    }
    public bool Equals(Position position) {
        return position.I == I && position.J == J;
    }
}
public enum Direction {
    North,
    East,
    South,
    West,
}
