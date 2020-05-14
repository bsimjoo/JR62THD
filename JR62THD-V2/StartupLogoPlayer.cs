using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JR62THD_V2 {
    class StartupLogoPlayer {
        public static void play() {
            string AsciiMap = Properties.Resources.ascii_map;

            const int AsciiMapHeight = 28;          // <-- ASCII Map each line chars count
            const int AsciiMapWidth = 42;           // <-- ASCII Map lines count
            const int AsciiMapLinesOffset = 2;      // <-- each line ends with "\n\r" characters

            //Console.WriteLine("Press any key to continue");
            //Console.ReadKey(true);
            Console.Clear();

            //Default console color:
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;

            //an array of forground-colors for each sequence of animation
            ConsoleColor[] palet = { ConsoleColor.White, ConsoleColor.White, ConsoleColor.Black };

            Random rnd = new Random();
            Console.CursorVisible = false;
            for (int seq = 0; seq<=3; seq++) {
                //counting sequences of animation
                //when all posible positions for cursor had been wrote a sequence compeleted
                List<int[]> pos = new List<int[]>();
                for (int t = 2; t <= Console.BufferHeight - 3; t++)
                    for (int l = 2; l <= Console.BufferWidth - 3; l++)
                        pos.Add(new int[] { l, t });
                //a list of all posible cursor positions
                //I used a list to make sure that each position will use just once and also know when this sequence will finish

                //this sequence foreground color
                ConsoleColor foregroundColor = palet[seq >= palet.Length ? palet.Length - 1 : seq];

                //a dictionary of characters foreground and background color
                Dictionary<char, ConsoleColor[]> CharsColorDict = new Dictionary<char, ConsoleColor[]>();
                CharsColorDict['.'] = new ConsoleColor[]{
                    (seq >= 2 ? ConsoleColor.Black : ConsoleColor.White),       //foreground color
                    (seq<=5?ConsoleColor.Blue:(ConsoleColor)(seq%15))           //background color, this will change in each sequence
                };
                CharsColorDict['#'] = new ConsoleColor[]{
                    (seq >= 2 ? ConsoleColor.Gray : ConsoleColor.White),        //foreground color
                    ConsoleColor.White                                          //background color
                };

                while (pos.Count > 0) {
                    //choosing a random position:
                    int r = rnd.Next(pos.Count - 1);
                    int[] p = pos[r];
                    pos.RemoveAt(r);

                    Console.SetCursorPosition(p[0], p[1]);
                    //calculating postion of cursor in ASCII map:
                    int l = p[0] - ((Console.BufferWidth - AsciiMapWidth) / 2);        //left
                    int t = p[1] - ((Console.BufferHeight - AsciiMapHeight) / 2);       //top

                    Console.ForegroundColor = foregroundColor;
                    Console.BackgroundColor = ConsoleColor.Black;
                    if (seq >= 1 && l >= 0 && t >= 0 && l < 42 && t < 28) {
                        // Index = (top * (lines_width + lines_offset) ) + left
                        int index = t * (AsciiMapWidth + AsciiMapLinesOffset) + l;
                        char c = AsciiMap[index];
                        if (CharsColorDict.ContainsKey(c)) {
                            Console.ForegroundColor = CharsColorDict[c][0];
                            Console.BackgroundColor = CharsColorDict[c][1];
                        }
                    }

                    Console.Write((char)rnd.Next((int)'!', (int)'z'));      //a random character
                    //System.Threading.Thread.Sleep(1);                     //wait after each sequence
                }
            }
        }
    }
}
