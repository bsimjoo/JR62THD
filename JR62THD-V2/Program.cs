using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.IO;
using System.Windows.Forms;

namespace JR62THD_V2 {
    class Program {
        const string pattern = @"[\w\d]+-([\w\d]{2})\s+(NRM|T(\d+))\s*(-?\d{1,2}.?\d*)\s*(-?\d{1,2}.?\d*)\s*(-?\d{1,2}.?\d*)\s*((-?\d+.?\d*)\s*)+";
        [STAThread]
        static void Main(string[] args) {
            Console.BufferWidth = Console.WindowWidth = Console.LargestWindowWidth > 100 ? 100 : Console.LargestWindowWidth;
            Console.BufferHeight = Console.WindowHeight = Console.LargestWindowHeight > 50 ? 50 : Console.LargestWindowHeight;
            ResetColor();
#if DEBUG
#else
            if (Console.WindowWidth > 42 && Console.WindowHeight > 28) {
                StartupLogoPlayer.play();
                Thread.Sleep(500);
            }
#endif
            Console.BufferHeight = 1000;
            Console.Clear();
            writeTitle("STEP1: Specify source and destination files");
            AdvancedPrint(
            wrap("You can select multiple files in open files dialog or press [+] key to add more source files." +
                " you can also remove a file after choosing it by [↑] and [↓] and then pressing [Del]." +
                " after you added all source files press [Enter] to finish adding source files and select destination file to save."),
            ForegroundColor: ConsoleColor.DarkGray
                );

            List<string> sourceFiles = new List<string>();
            Console.WriteLine("Source files:");
            Console.ForegroundColor = ConsoleColor.Gray;
            
            bool showDialog = true;
            while (true) {
                OpenFileDialog ofd = new OpenFileDialog() {
                    Filter = "jr6 file|*.jr6|All files|*",
                    Multiselect = true,
                    CheckFileExists = true
                };
                if (showDialog)
                    ofd.ShowDialog();
                showDialog = true;
                sourceFiles.AddRange(ofd.FileNames);
                Console.SetCursorPosition(0, 7);
                int count = 1;
                foreach (string file in sourceFiles) {
                    Console.WriteLine(string.Format("\r  [{0,3}] {1}", count, Path.GetFileName(file)).PadRight(99));
                    count++;
                }
                AdvancedPrint("\r    Press [+], [Enter], [↑] or [↓]".PadRight(99), false, ForegroundColor: ConsoleColor.DarkGray);
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter && sourceFiles.Count > 0) break;
                else if (key.KeyChar == '=' || key.KeyChar == '+') continue;
                else if (key.Key == ConsoleKey.UpArrow || key.Key == ConsoleKey.DownArrow && sourceFiles.Count>0) {
                    ConsoleMenu menu = new ConsoleMenu();
                    int index = 1;
                    foreach(var file in sourceFiles) {
                        ConsoleMenuItem item = new ConsoleMenuItem();
                        item.Text = string.Format("[{0,3}] {1}",index,Path.GetFileName(file));
                        item.SelectedText= string.Format("[{0,3}] {1,-45} {2}", index, Path.GetFileName(file),"[Del] Remove | [Esc] Cancel");
                        item.Length = 84;
                        item.Left = 2;
                        item.Top = 6 + index;
                        index++;
                        menu.Items.Add(item);
                    }
                    menu.AvallableKeys = new ConsoleKey[] { ConsoleKey.Delete,ConsoleKey.Escape };
                    menu.ShowMenu();
                    if (menu.StartMenu().Key == ConsoleKey.Delete) {
                        var answer=MessageBox.Show($"Are you sure you want to remove \"{Path.GetFileName(sourceFiles[menu.selectedIndex])}\" from source files list",
                            "Removeing a file from list",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Exclamation);
                        if (answer == DialogResult.Yes) {
                            sourceFiles.RemoveAt(menu.selectedIndex);
                            Console.WriteLine("\r".PadRight(100));
                        }
                    }
                    showDialog = false;
                }
            }
            ResetColor();

            Console.WriteLine("\n\rDestination file:");
            SaveFileDialog sfd = new SaveFileDialog() {
                Filter = "THD file|*.thd|All files|*"
            };
            while (sfd.ShowDialog() != DialogResult.OK) ;
            AdvancedPrint("  "+ sfd.FileName,ForegroundColor:ConsoleColor.Gray);
            string output = sfd.FileName;
            Console.WriteLine();
            bool singleOutput = true;
            if (sourceFiles.Count > 1) {
                singleOutput = MessageBox.Show(
                    "Include all source files into a single output file?",
                    "JR6 to THD converter version 2"
                    , MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) == DialogResult.Yes;
                Console.WriteLine("Single output mode: {0}", singleOutput ? "Yes" : "No");
                if (!singleOutput)
                    AdvancedPrint("files will save in splited numbred files.", ForegroundColor: ConsoleColor.DarkGray);
                Console.WriteLine();
            }
            int top = Console.CursorTop;
            writeTitle("STEP2: Checking source files format");
            AdvancedPrint("Checking source files to sure selected files format correction",ForegroundColor:ConsoleColor.DarkGray);
            Console.WriteLine();
            Console.CursorVisible = false;
            List<string> incorrectFiles = new List<string>();
            string[] _sourceFiles = sourceFiles.ToArray();
            foreach(string file in _sourceFiles) {
                Console.Write($"\r Scanning file: {Path.GetFileName(file)}".PadRight(100));
                using(StreamReader reader=new StreamReader(file)) {
                    while (!reader.EndOfStream) {
                        string line = reader.ReadLine();
                        if (!Regex.IsMatch(line, pattern)){
                            incorrectFiles.Add(Path.GetFileName(file));
                            sourceFiles.Remove(file);
                            AdvancedPrint($"{incorrectFiles.Count} bad files found",
                                false,
                                ForegroundColor: ConsoleColor.Red,
                                CursorTop: Console.CursorTop + 1,
                                CursorLeft:0);
                            Console.CursorTop--;
                            break;
                        }

                    }
                }
            }
            if (incorrectFiles.Count > 0) {
                Console.WriteLine("\nBad files removed:");
                Console.WriteLine(string.Join("\n", incorrectFiles));
            }
            if (sourceFiles.Count == 0) {
                MessageBox.Show("All source files weren't in correct format and now, there's not any file to convert.",
                    "Bad files",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                Environment.Exit(1);
            }

            Console.WriteLine();
            writeTitle("STEP3: Collecting more data and converting");
            AdvancedPrint(wrap(
                "I need more data to convert file. please move between colored parts by [↑] and [←] or [→] and [↓] " +
                "and edit value by pressing [Enter] and then at the end press [End] to go.")
                , ForegroundColor: ConsoleColor.DarkGray);
            Console.WriteLine();
            top = Console.CursorTop;
            object[][] values = {
                new string[] {"0.00", "0.00", "0.0", "0.0", "0.0", "0.00", "0.00", "0.00", "0.00", "0.00"},
                new object[10]
            };
            string[] formats = (string[])values[0];
            values[1] = values[1].Select(x => (object)false).ToArray();
            foreach (string file in sourceFiles) {
                //reading source file
                Dictionary<string, List<string[]>> FileData = new Dictionary<string, List<string[]>>();
                using (StreamReader reader = new StreamReader(file)) {
                    while (!reader.EndOfStream) {
                        var m = Regex.Match(reader.ReadLine(), pattern);
                        if (!FileData.ContainsKey(m.Groups[1].Value))
                            FileData[m.Groups[1].Value] = new List<string[]>();
                        string[] dataArray = {
                            m.Groups[2].Value=="NRM"?"0":m.Groups[3].Value,
                            m.Groups[4].Value,
                            m.Groups[5].Value,
                            m.Groups[6].Value
                        };
                        FileData[m.Groups[1].Value].Add(dataArray);
                    }
                }
                int filenum = 1;
                foreach (var sample in FileData) {
                    string Format = "KRIR1-"+sample.Key+"{0,10}{1,7}{2,8}{3,8}{4,8}{5,7}{6,7}{7,7}{8,7}\n";
                    foreach(string[] data in sample.Value) {
                        Format += string.Format("{0,4}{1,15}{2,15}{3,15}", data) + "{9,7}\n";
                    }
                    bool showMenu = false;
                    foreach(bool b in values[1])
                        if (!b) {
                            showMenu = true;
                            break;
                        }
                    Format += "1001\n";
                    if (showMenu) {
                        int index = 0;
                        while (true) {
                            string menuFields = string.Format(
                                    Format,
                                    values[0].Select((x, i) => (bool)values[1][i] ? $"|{x}|" : $"[{x}]").ToArray());
                            Console.SetCursorPosition(0, top);
                            for (int i = 0; i < 20; i++)
                                Console.WriteLine("\r".PadRight(Console.WindowWidth));
                            Console.SetCursorPosition(0, top);
                            var items = writeTextGetItems(menuFields);
                            Console.WriteLine("\n");
                            ConsoleMenu editMenu = new ConsoleMenu();
                            var Button = new ConsoleMenuItem();
                            Button.Text =         " [Done and convert ] ";
                            Button.SelectedText = " [Press enter to go] ";
                            Button.Length = Button.Text.Length;
                            Button.Top = Console.CursorTop;
                            Button.Left = Console.WindowWidth - Button.Length - 5;
                            Button.BackgroundColor = ConsoleColor.Yellow;
                            Button.ForegroundColor = ConsoleColor.Black;
                            Button.Value = true;
                            items.Add(Button);
                            editMenu.Items = items;
                            editMenu.selectedIndex = index;
                            editMenu.AvallableKeys = new ConsoleKey[] { ConsoleKey.Enter, ConsoleKey.End };
                            editMenu.ShowMenu();
                            var k=editMenu.StartMenu();
                            if (k.Key == ConsoleKey.Enter) {
                                if (editMenu.selectedIndex < items.Count - 1) {
                                    InputDialog id = new InputDialog() {
                                        Value = (string)values[0][(int)editMenu.SelectedItem.Value]
                                    };
                                    if (id.ShowDialog() == DialogResult.OK && float.TryParse(id.Value, out float f)) {
                                        if ((int)editMenu.SelectedItem.Value < 9) {
                                            values[0][(int)editMenu.SelectedItem.Value] = f.ToString(formats[(int)editMenu.SelectedItem.Value]);
                                            values[1][(int)editMenu.SelectedItem.Value] = id.ForAll;
                                        } else {
                                            values[0][9] = f.ToString(formats[9]);
                                            values[1][9] = id.ForAll;
                                        }
                                        index = editMenu.selectedIndex;
                                    }

                                } else if (editMenu.SelectedItem.Value != null) {
                                    if ((bool)editMenu.SelectedItem.Value)
                                        break;
                                }
                            } else
                                break;
                        }
                    }
                    string outputFile = "";
                    if (!singleOutput) {
                        outputFile = Path.GetFileNameWithoutExtension(output) + $" ({filenum})" + Path.GetExtension(output);
                        filenum++;
                    } else outputFile = output;
                    using (StreamWriter writer = new StreamWriter(outputFile, singleOutput)) {
                        writer.Write(string.Format(Format, values[0]));
                    }
                }
            }
            Console.WriteLine("Done");
            Thread.Sleep(1000);
        }
        static List<ConsoleMenuItem> writeTextGetItems(string text) {
            List<ConsoleMenuItem> items = new List<ConsoleMenuItem>();
            bool open = true;
            int index = 0;
            for(int i=0;i<text.Length;i++) {
                char ch = text[i];
                if (ch == '|' || ch == '[') {
                    var newitem = new ConsoleMenuItem();
                    newitem.Top = Console.CursorTop;
                    newitem.Left = Console.CursorLeft;
                    switch (ch) {
                        case '|':
                            if (open) {
                                open = false;
                                newitem.SelectedText = newitem.Text = text.Substring(i + 1, text.IndexOf('|', i + 1) - i - 1);
                                newitem.Length = text.IndexOf('|', i + 1) - i - 1;
                                newitem.BackgroundColor = ConsoleColor.DarkGreen;
                            } else {
                                open = true;
                                continue;
                            }
                            break;
                        case '[':
                            newitem.SelectedText = newitem.Text = text.Substring(i + 1, text.IndexOf(']', i + 1) - i - 1);
                            newitem.Length = text.IndexOf(']', i + 1) - i - 1;
                            break;
                    }
                    newitem.Value = index;
                    index++;
                    items.Add(newitem);
                } else if (ch != ']') Console.Write(ch);
            }
            Console.WriteLine();
            return items;
        }
        static void ResetColor() {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
        }
        static void AdvancedPrint(string Text,bool WriteLine=true,ConsoleColor BackgroundColor=ConsoleColor.Black,ConsoleColor ForegroundColor=ConsoleColor.White,int CursorTop=-1,int CursorLeft = -1) {
            Console.BackgroundColor = BackgroundColor;
            Console.ForegroundColor = ForegroundColor;
            Console.CursorTop = CursorTop == -1 ? Console.CursorTop : CursorTop;
            Console.CursorLeft = CursorLeft == -1 ? Console.CursorLeft : CursorLeft;
            if (WriteLine)
                Console.WriteLine(Text);
            else
                Console.Write(Text);
            ResetColor();
        }
        static void writeTitle(string title) {
            AdvancedPrint(title.PadRight(Console.WindowWidth),BackgroundColor:ConsoleColor.Cyan,ForegroundColor:ConsoleColor.Black);
            Console.CursorTop--;
        }
        static string wrap(string Text) {
            int MaxLength = Console.WindowWidth;
            List<string> Lines = new List<string>();
            Lines.AddRange(Text.Split('\n'));
            int count = Lines.Count;
            for (int i = 0; i < count; i++) {
                string line = Lines[i];
                if (line.Length > MaxLength) {
                    if (line.Substring(0, MaxLength).Contains(' ')) {
                        int index = line.Substring(0, MaxLength).LastIndexOf(' ');
                        Lines.Insert(i + 1, line.Substring(index).Trim(' '));
                        Lines[i] = line.Substring(0, index);        // index+1 = length BUT I WANT TO REMOVE LAST SPACE
                        count = Lines.Count;
                    } else {
                        Lines.Insert(i + 1, line.Substring(MaxLength - 2));
                        Lines[i] = line.Substring(0, MaxLength - 1) + '-';
                        count = Lines.Count;
                    }
                }
            }
            return string.Join("\n", Lines);
        }
    }
}
