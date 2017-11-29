using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EaseChallenge
{
    class Program
    {
        [STAThread]
        static void Main()
        {
            int[,] map = null;
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Title = "";
                ofd.RestoreDirectory = true;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        int[] currentPoint = { 0, 0 };
                        int lengthOfPath = 0;
                        int dropOfPath = 0;
                        String calculatedPath = String.Empty;
                        map = CreatePointsTableFromFile(ofd.FileName);

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(String.Format("Error occured when creating points table. Ex: {0}", ex.Message));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Format("Error occured when opening file dialog. Ex: {0}", ex.Message));
            }
            Console.Read();
        }

        static string CreatePath(int[,] values, int selectedX, int selectedY)
        {
            Dictionary<string, int> neighboors = new Dictionary<string, int>(); //Indexes; 0. north, 1. south, 2. east, 3. west
            string path = string.Empty;
            if (selectedX > 0)
                neighboors.Add("west", values[selectedX - 1, selectedY]);
            neighboors.Add("east", values[selectedX + 1, selectedY]);
            if (selectedY > 0)
                neighboors.Add("north", values[selectedX, selectedY - 1]);
            neighboors.Add("south", values[selectedX, selectedY + 1]);

            for (int i = 0; i < neighboors.Count; i++)
            {
                if (neighboors.ElementAt(i).Value >= values[selectedX, selectedY])
                {
                    neighboors.Remove(neighboors.ElementAt(i).Key);
                }
            }

            return null;
        }

        static int[,] CreatePointsTableFromFile(string path)
        {
            try
            {
                String[] lines = File.ReadAllLines(path);
                int lengthOfMap = Convert.ToInt32(lines[0].Split(' ')[0]);
                int[,] values = new int[lengthOfMap, lengthOfMap];
                for (int i = 1; i < lines.Length; i++)
                {
                    int y = 0;
                    foreach (var point in lines[i].Split(' '))
                    {
                        values[i - 1, y] = Convert.ToInt32(point);
                        y++;
                    }
                }
                return values;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
