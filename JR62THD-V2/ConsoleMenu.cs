using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JR62THD_V2 {
    class ConsoleMenu {
        public List<ConsoleMenuItem> Items = new List<ConsoleMenuItem>();
        public ConsoleMenuItem SelectedItem = null;
        public void ShowMenu() {
            int left = Console.CursorLeft;
            int top = Console.CursorTop;
            var bgc = Console.BackgroundColor;
            var fgc = Console.ForegroundColor;
            foreach (var item in Items) {
                Console.SetCursorPosition(item.Left, item.Top);
                Console.BackgroundColor = item.BackgroundColor;
                Console.ForegroundColor = item.ForegroundColor;
                Console.Write(item.Text.PadRight(item.Length));
            }
            Console.SetCursorPosition(left, top);
            Console.BackgroundColor = bgc;
            Console.ForegroundColor = fgc;
        }
        public ConsoleKey[] AvallableKeys { get; set; } = new ConsoleKey[] { ConsoleKey.Enter };
        public char[] AvallableKeyChars { get; set; } = new char[] { };
        public int selectedIndex { get; set; } = 0;
        public ConsoleKeyInfo StartMenu() {
            int left = Console.CursorLeft;
            int top = Console.CursorTop;
            var bgc = Console.BackgroundColor;
            var fgc = Console.ForegroundColor;
            Console.CursorVisible = false;
            while (true) {
                if (selectedIndex < 0) selectedIndex = Items.Count - 1;
                selectedIndex %= Items.Count;
                SelectedItem = Items[selectedIndex];
                selectItem(selectedIndex);
                var k = Console.ReadKey(true);
                if (k.Key == ConsoleKey.UpArrow || k.Key == ConsoleKey.LeftArrow) {
                    deSelectItem(selectedIndex);
                    selectedIndex--;
                } else if (k.Key == ConsoleKey.DownArrow || k.Key == ConsoleKey.RightArrow) {
                    deSelectItem(selectedIndex);
                    selectedIndex++;
                } else if (AvallableKeys.Contains(k.Key) || AvallableKeyChars.Contains(k.KeyChar)) {
                    Console.SetCursorPosition(left, top);
                    Console.BackgroundColor = bgc;
                    Console.ForegroundColor = fgc;
                    Console.CursorVisible = true;
                    return k;
                }
            }
        }
        void selectItem(int index) {
            var item = Items[index];
            Console.SetCursorPosition(item.Left, item.Top);
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write(item.SelectedText.PadRight(item.Length));
        }
        void deSelectItem(int index) {
            var item = Items[index];
            Console.SetCursorPosition(item.Left, item.Top);
            Console.BackgroundColor = item.BackgroundColor;
            Console.ForegroundColor = item.ForegroundColor;
            Console.Write(item.Text.PadRight(item.Length));
        }
    }
    class ConsoleMenuItem {
        public int Top { get; set; }
        public int Left { get; set; }
        public int Length { get; set; }
        public string Text { get; set; }
        public string SelectedText { get;set; }
        public ConsoleColor BackgroundColor { get; set; } = ConsoleColor.DarkGray;
        public ConsoleColor ForegroundColor { get; set; } = ConsoleColor.White;
        public object Value { get; set; }
    }
}
