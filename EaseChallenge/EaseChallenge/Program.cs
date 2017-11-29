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
        static int lengthOfPath = 1;
        static int dropOfPath = 0;

        [STAThread]
        static void Main()
        {
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
                        String calculatedPath = String.Empty;
                        var map = CreatePointsTableFromFile(ofd.FileName);
                        int lengthOfMap = map.GetLength(0);
                        for (int i = 0; i < lengthOfMap; i++)
                        {
                            int[] row = Enumerable.Range(0, map.GetUpperBound(1) + 1).Select(x => map[i, x]).ToArray();
                            calculatedPath = CreatePath(map, Array.IndexOf(row, row.Max()), i);
                        }
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
            int selectedPoint = values[selectedY, selectedX];
            int currentPoint = selectedPoint;
            string jumpDirection = string.Empty;
            Dictionary<string, int> neighboors = new Dictionary<string, int>();
            if (selectedX > 0)
                neighboors.Add("west", values[selectedY , selectedX-1]);
            neighboors.Add("east", values[selectedY, selectedX+1]);
            if (selectedY > 0)
                neighboors.Add("north", values[selectedY-1, selectedX ]);
            neighboors.Add("south", values[selectedY+1, selectedX]);
            List<string> keysToRemove = new List<string>();
            foreach (var point in neighboors)
            {
                if (point.Value >= selectedPoint)
                {
                    keysToRemove.Add(point.Key);
                }
            }
            foreach (var key in keysToRemove)
            {
                neighboors.Remove(key);
            }

            if(neighboors.Count > 0)
                if (neighboors.Values.Max() < selectedPoint )
                    jumpDirection = neighboors.FirstOrDefault(x => x.Value.Equals(neighboors.Values.Max()) && x.Value < currentPoint).Key;

            switch (jumpDirection)
            {
                case "north":
                    lengthOfPath++;
                    neighboors.TryGetValue("north", out currentPoint);
                    return selectedPoint.ToString() + "-" + CreatePath(values, selectedX, selectedY - 1);
                case "south":
                    lengthOfPath++;
                    neighboors.TryGetValue("south", out currentPoint);
                    return selectedPoint.ToString() + "-" + CreatePath(values, selectedX, selectedY + 1);
                case "east":
                    lengthOfPath++;
                    neighboors.TryGetValue("east", out currentPoint);
                    return selectedPoint.ToString() + "-" + CreatePath(values, selectedX + 1, selectedY);
                case "west":
                    lengthOfPath++;
                    neighboors.TryGetValue("west", out currentPoint);
                    return selectedPoint.ToString() + "-" + CreatePath(values, selectedX - 1, selectedY);
                default:
                    return selectedPoint.ToString();
            }
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
                        values[i - 1,y] = Convert.ToInt32(point);
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
