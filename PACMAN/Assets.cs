namespace PACMAN;
internal class Assets {
    public static string[]? DrawMainBoard(int type = 1) {
        switch (type) {
            case 1:
                string[] board1 = [
                    "█████████████████████████████",
                    "█************███************█",
                    "█*████*█████*███*█████*████*█",
                    "█+████*█████*███*█████*████+█",
                    "█*████*█████*███*█████*████*█",
                    "█***************************█",
                    "█*████*██*█████████*██*████*█",
                    "█*████*██*█████████*██*████*█",
                    "█******██****███****██******█",
                    "██████*█████ ███ █████*██████",
                    "     █*██           ██*█     ",
                    "██████*██ ████b████ ██*██████",
                    "t     *██ █ i p c █ ██*     t",
                    "██████*██ █████████ ██*██████",
                    "     █*██           ██*█     ",
                    "██████*█████ ███ █████*██████",
                    "█************███************█",
                    "█*████*█████*███*█████*████*█",
                    "█*████*█████*███*█████*████*█",
                    "█+**██********s********██**+█",
                    "███*██*██*█████████*██*██*███",
                    "███*██*██*█████████*██*██*███",
                    "█******██****███****██******█",
                    "█*██████████*███*██████████*█",
                    "█*██████████*███*██████████*█",
                    "█***************************█",
                    "█████████████████████████████",
                ];
                return board1;
            default:
                return null;

        }
    }


}
public class Ghost {
    public string Name { get; set; }
    public Position Position { get; set; }
    public Direction Direction { get; set; }
    ConsoleColor Color { get; }
    public Ghost(string name, Position position) {
        Name = name;
        Position = position;
        switch (name.ToLower()) {
            case "blinky": Color = ConsoleColor.Red; break;
            default: Color = ConsoleColor.White; break;
        }
    }
}
