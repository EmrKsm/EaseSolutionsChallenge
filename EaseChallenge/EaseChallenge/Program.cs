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
        static String choosenPath = String.Empty;

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
                        //This list will contain possible paths that rider could choose
                        List<String> listOfPathsPossible = new List<string>();
                        lengthOfPath = 1;
                        //Calculate paths row by row
                        for (int i = 0; i < lengthOfMap; i++)
                        {
                            int[] row = Enumerable.Range(0, map.GetUpperBound(1) + 1).Select(x => map[i, x]).ToArray();
                            for (int j = 0; j < row.Length; j++)
                            {
                                calculatedPath = CreatePath(map, j, i);
                                //Check if calculated path length is bigger or equal to max length found. Otherwise, path won't be included in possible list.
                                if (calculatedPath.Split('-').Length >= lengthOfPath)
                                {
                                    listOfPathsPossible.Add(calculatedPath);
                                    lengthOfPath = calculatedPath.Split('-').Length;
                                    listOfPathsPossible.RemoveAll(p => p.Split('-').Length < lengthOfPath);
                                }
                            }                          
                        }
                        dropOfPath = 0;
                        //Calculation max drop values of list and finds max drop and it's path
                        foreach (var path in listOfPathsPossible)
                        {
                            int calculatedDrop = CalculateDropOfPath(path);
                            if (calculatedDrop > dropOfPath)
                            {
                                dropOfPath = calculatedDrop;
                                choosenPath = path;
                            }
                        }
                        Console.WriteLine(String.Format("Path: {0}\n\rLength: {1}\n\rDrop: {2}", choosenPath, lengthOfPath.ToString(),dropOfPath.ToString()));
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

        /// <summary>
        /// Calculates drop of path between starting point and ending poinnt
        /// </summary>
        /// <param name="path">Path string</param>
        /// <returns></returns>
        static int CalculateDropOfPath(string path)
        {
            int firstPoint = Convert.ToInt32(path.Split('-')[0]);
            int lastPoint = Convert.ToInt32(path.Split('-').Last());
            return firstPoint - lastPoint;
        }
        /// <summary>
        /// Creates path recursively with given map array and current points
        /// </summary>
        /// <param name="values">Map array</param>
        /// <param name="selectedX">Current X coordinate points to look for next point</param>
        /// <param name="selectedY">Current Y coordinate points to look for next point</param>
        /// <returns></returns>
        static string CreatePath(int[,] values, int selectedX, int selectedY)
        {
            int selectedPoint = values[selectedY, selectedX];
            int currentPoint = selectedPoint;
            string jumpDirection = string.Empty;
            Dictionary<string, int> neighboors = new Dictionary<string, int>();
            if (selectedX > 0)
                neighboors.Add("west", values[selectedY , selectedX-1]);
            if (selectedX < values.GetLength(1)-1)
                neighboors.Add("east", values[selectedY, selectedX+1]);
            if (selectedY > 0)
                neighboors.Add("north", values[selectedY-1, selectedX ]);
            if (selectedY < values.GetLength(0) - 1)
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
                    neighboors.TryGetValue("north", out currentPoint);
                    return selectedPoint.ToString() + "-" + CreatePath(values, selectedX, selectedY - 1);
                case "south":
                    neighboors.TryGetValue("south", out currentPoint);
                    return selectedPoint.ToString() + "-" + CreatePath(values, selectedX, selectedY + 1);
                case "east":
                    neighboors.TryGetValue("east", out currentPoint);
                    return selectedPoint.ToString() + "-" + CreatePath(values, selectedX + 1, selectedY);
                case "west":
                    neighboors.TryGetValue("west", out currentPoint);
                    return selectedPoint.ToString() + "-" + CreatePath(values, selectedX - 1, selectedY);
                default:
                    return selectedPoint.ToString();
            }
        }
        /// <summary>
        /// Creates map from given file contains points. Get length of dimension from first line, other lines will be points
        /// </summary>
        /// <param name="path">File path contains points</param>
        /// <returns></returns>
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
