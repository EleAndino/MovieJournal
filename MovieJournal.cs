using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieJournal
{
     public class MovieJournal
    {
        List<Entry> entries;
        int entriesPerPage;

        public static void Main(string[] args)
        {
            MovieJournal mj = new MovieJournal();
        }

        //*************************************************************************
        
        public MovieJournal()
        {
            entries = new List<Entry>();
            entriesPerPage = 10;

            ShowSplashScreen();
            ShowMainMenu();

        }

        //*************************************************************************

        public void ShowSplashScreen()
        {
            string screen = "Welcome to Movie Journal!\r\n\n" +
                "Author: Elena E. Andino Perez\r\n" +
                "Version: 1.0 Final\r\n" +
                "Last Revised: 2025-04-12 08:19 PM\r\n\n" +
                "Description:\r\n" +
                "This program allows you to keep records of your movie reviews.\r\n";

            Console.WriteLine(screen);
            PressEnterToContinue();

        }

        //*************************************************************************

        public void PressEnterToContinue()
        {
            Console.Write("Press ENTER to continue.");
            while (Console.ReadKey(true).Key != ConsoleKey.Enter) { }
            Console.Clear();
        }

        //*************************************************************************

        public void ShowMainMenu()
        {
            string menu = $@"
[1] Load entries from a file.
[2] Show all journal entries.
[3] Show a particular entry in more detail.
[4] Create new entry.
[5] Edit entry.
[6] Remove entry.
[7] Save entries to a file.
[8] Exit.";

            int option = 8;
            do
            {
                Console.Clear();
                Console.WriteLine(menu);
                option = GetOption("Select an option: ", 1, 8);
                Console.Clear();

                if (option == 1)
                {
                    LoadEntriesFromFile();
                }
                else if (option == 2)
                {
                    ShowEntries();
                }
                else if (option == 3)
                {
                    ShowEntry();
                }
                else if (option == 4)
                {
                    CreateNewEntry();
                }
                else if (option == 5)
                {
                    EditEntry();
                }
                else if (option == 6)
                {
                    RemoveEntry();
                }
                else if (option == 7)
                {
                    SaveEntriesToFile();
                }
                else
                {
                    if (!Exit()) { option = -1; }
                }

                    PressEnterToContinue();
            }
            while (option != 8);
        }

        //*************************************************************************

        public void LoadEntriesFromFile()
        {
            Console.WriteLine("Loading from: ");
            Console.WriteLine("Write the filename or nothing to cancel.");
            Console.Write("Filename: ");
            string filename = Console.ReadLine();

            LoadEntriesFromFile(filename);
        }

        public void LoadEntriesFromFile(string filename) 
        { 
            if (filename == null || filename.Length == 0)
            {
                Console.WriteLine("The operation was canceled.");
            }
            else if (!File.Exists(filename))
            {
                Console.WriteLine($"ERROR: file \"{filename}\" was not found!");
            }
            else
            {
                StreamReader sr = null;

                try
                {
                    sr = new StreamReader(filename);

                    while(!sr.EndOfStream)
                    {
                        Entry e = new Entry();

                        e.title = sr.ReadLine();
                        e.year = int.Parse(sr.ReadLine());
                        e.rating = float.Parse(sr.ReadLine());
                        e.comments = sr.ReadLine();

                        entries.Add(e);
                    }

                    Console.WriteLine("Movie journal loaded successfully!");

                }
                catch (Exception)
                {
                    Console.WriteLine("ERROR: An error ocurred while reading the file!");
                }
                finally
                {
                    if (sr != null) { sr.Dispose(); }
                }
            }
            
        }

        //*************************************************************************

        public void ShowEntries()
        {
           
                int page = 1;

            do
            {
                Console.Clear();
                int pageCount = Math.Max(1, (int)Math.Ceiling(entries.Count / (double)entriesPerPage));
                int offset = (page - 1) * entriesPerPage;
                int limit = Math.Min(offset + entriesPerPage, entries.Count);

                for (int i = offset; i < limit; i++)
                {
                    Entry e = entries[i];
                    string cmt = (e.comments.Length > 16) ? e.comments.Substring(0, 13) + "..." : e.comments;
                    string tit = (e.title.Length > 42) ? e.title.Substring(0, 39) + "..." : e.title;
                    Console.WriteLine($"[{i.ToString().PadLeft(3, '0')}] {tit,-42} {e.year,4} {e.rating,4:F1} {cmt,-16}");
                }
                for (int i = 0; i < entriesPerPage - (limit - offset); i++)
                {
                    Console.WriteLine();
                }

                Console.WriteLine($"Page {page} of {pageCount}");
                page = GetOption($"Enter [1-{pageCount}] Go to page or [0] to cancel: ", 0, pageCount);

            } while (page != 0);
        }

        //*************************************************************************

        public void ShowEntry()
        {
            int s = 0;
            int e = entries.Count - 1;
            int index = GetOption($"Choose index of entry [{s}-{e}]: ", s, e);

            ShowEntry(index);
        }
        public void ShowEntry(int index)
        {
            Entry e = entries[index];
            string text = $"[{index.ToString().PadLeft(3, '0')}]\nTitle: {e.title}\nYear: {e.year}\n" +
                $"Rating: {e.rating, 4:F1}\nComments: {e.comments}\n";

            Console.WriteLine(text);
        }

        //*************************************************************************

        public void CreateNewEntry()
        {
            Console.WriteLine("Enter your new entry: \n");

            Console.Write("Title: ");
            string title = Console.ReadLine();

            int year = GetOption("Year: ", 0, 9999);

            float rating = GetOption("Rating: ", 0.0F, 10.0F);

            Console.Write("Comments: ");
            string comments = Console.ReadLine();

            string answer = GetOption("Do you want to add this entry? [Y/N] ", "Y", "N");

            if (answer == "Y")
            {
                Entry e = new Entry();

                e.title = title;
                e.year = year;
                e.rating = rating;
                e.comments = comments;

                entries.Add(e);

                Console.WriteLine("Entry created successfully!");
            }
            else
            {
                Console.WriteLine("Operation was cancelled.");
            }

        }

        //*************************************************************************

        public void EditEntry()
        {
            int s = 0;
            int e = entries.Count - 1;
            int index = GetOption($"Choose index of entry [{s}-{e}]: ", s, e);

            EditEntry(index);
        }

        public void EditEntry(int index)
        {
            Entry e = entries[index];
            string text = $"[{index.ToString().PadLeft(3, '0')}]\nTitle: {e.title}\nYear: {e.year}\n" +
                $"Rating: {e.rating,4:F1}\nComments: {e.comments}\n";

            Console.WriteLine(text);
            Console.WriteLine("[0] Title [1] Year [2] Rating [3] Comments [4] Cancel: ");
            int option = GetOption("Which property would you like to edit? ", 0, 4);
            
            if (option == 0)
            {

                Console.Write("Enter new title or nothing to cancel: ");
                string title = Console.ReadLine();

                if (title == null || title.Length == 0)
                {
                    Console.WriteLine("Edit was canceled.");
                }
                else
                {
                    e.title = title;
                    Console.WriteLine("Edit was successful!");
                }
                
            }
            else if (option == 1)
            {
                
                int year = GetOption("Enter new year or 0 to cancel: ", 0, 9999);

                if (year == 0)
                {
                    Console.WriteLine("Edit was canceled.");
                }
                else
                {
                    e.year = year;
                    Console.WriteLine("Edit was successful!");
                }

            }
            else if (option == 2)
            {
                float rating = GetOption("Enter new rating or -1 to cancel: ", -1.0F, 10.0F);

                if (rating == -1)
                {
                    Console.WriteLine("Edit was canceled.");
                }
                else
                {
                    e.rating = rating;
                    Console.WriteLine("Edit was successful!");
                }
            }
            else if (option == 3)
            {
                Console.Write("Enter new comment or nothing to cancel: ");
                string comments = Console.ReadLine();

                if (comments == null || comments.Length == 0)
                {
                    Console.WriteLine("Edit was canceled.");
                }
                else
                {
                    e.comments = comments;
                    Console.WriteLine("Edit was successful!");
                }
            }
            else // if (option == 4)
            {
                Console.WriteLine("Edit was cancelled.");
            }

            ShowEntry(index);

        }

        //*************************************************************************

        public void RemoveEntry()
        {
            int s = 0;
            int e = entries.Count - 1;
            int index = GetOption($"Choose index of entry [{s}-{e}]: ", s, e);

            RemoveEntry(index);
        }

        public void RemoveEntry(int index)
        {
            Entry e = entries[index];

            string option = GetOption("Are you sure that you want to delete this entry? [Y/N] ", "Y","N");

            if (option == "Y")
            {
                entries.RemoveAt(index);
                Console.WriteLine("Successfully removed entry.");
            }
            else
            {
                Console.WriteLine("The operation was cancelled.");
            }

        }

        //*************************************************************************

        public void SaveEntriesToFile()
        {
            Console.WriteLine("Saving to: ");
            Console.WriteLine("Write the filename or nothing to cancel.");
            Console.Write("Filename: ");
            string filename = Console.ReadLine();

            SaveEntriesToFile(filename);
        }

        public void SaveEntriesToFile(string filename)
        {
            if (filename == null || filename.Length == 0)
            {
                Console.WriteLine("The operation was canceled.");
            }
            else if (!File.Exists(filename))
            {
                Console.WriteLine($"ERROR: file \"{filename}\" was not found!");
            }
            else
            {
                StreamWriter sw = null;

                try
                {
                    sw = new StreamWriter(filename);

                    foreach(Entry e in entries)
                    {
                        sw.WriteLine(e.title);
                        sw.WriteLine(e.year);
                        sw.WriteLine(e.rating);
                        sw.WriteLine(e.comments);
                    }

                    Console.WriteLine("Movie journal saved successfully!");

                }
                catch (Exception)
                {
                    Console.WriteLine("ERROR: An error ocurred while saving the file!");
                }
                finally
                {
                    if (sw != null) { sw.Dispose(); }
                }
            }

        }

        //**************************************************************************

        public bool Exit()
        {
            string answer = GetOption("Are you sure that you want to exit? [Y/N] ", "Y", "N");
            if (answer == "Y")
            {
                Console.WriteLine("Thanks for using movie journal, see you next time!");
                return true;
            }
            else
            {
                return false;
            }
        }

        //**************************************************************************

        public class Entry
        {

            public string title;
            public int year;
            public float rating;
            public string comments;

            public Entry()
            {
                title = "";
                year = 0;
                rating = 0.0F;
                comments = "";
            }

        }

        //**************************************************************************

        public int GetOption(string prompt, int min, int max)
        {
            Console.Write(prompt);
            string input = Console.ReadLine().Trim();
            int option;
            while (!int.TryParse(input, out option) || option < min || option > max)
            {
                Console.WriteLine("ERROR: Invalid option.");
                Console.Write(prompt);
                input = Console.ReadLine().Trim();
            }
            return option;
        }

        public float GetOption(string prompt, float min, float max)
        {
            Console.Write(prompt);
            string input = Console.ReadLine().Trim();
            float option;
            while (!float.TryParse(input, out option) || option < min || option > max)
            {
                Console.WriteLine("ERROR: Invalid option.");
                Console.Write(prompt);
                input = Console.ReadLine().Trim();
            }
            return option;
        }

        public string GetOption(string prompt, params string[] validOptions)
        {
            Console.Write(prompt);
            string option = Console.ReadLine().Trim().ToUpper();
            while (!validOptions.Contains(option))
            {
                Console.WriteLine("ERROR: Invalid option.");
                Console.Write(prompt);
                option = Console.ReadLine().Trim().ToUpper();
            }
            return option;
        }

    }
}
