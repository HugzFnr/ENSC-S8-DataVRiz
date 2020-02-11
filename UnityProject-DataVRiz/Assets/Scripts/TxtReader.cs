using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;

public class TxtReader
{
    public static List<DataLine> Read(string filePath)
    {
        List<DataLine> pointsList = new List<DataLine>();
        char separator = ','; //default

        using (StreamReader sr = new StreamReader(filePath))
        {
            //first line will be used to name axis, but skipped for now
            sr.ReadLine();

            string line = sr.ReadLine();
            while (line!=null)
            {
                string label="";
                string xvalue="";
                string yvalue="";
                string zvalue="";
                int step = 0; //0 is waiting for label; 1 is label assigned, 2 is xvalue assigned, 4 is complete

                foreach (char ch in line)
                {
                    if (ch == separator) step++;
                    else if (step == 0)
                    {
                        label += ch;
                    }
                    else if (step==1)
                    {
                        xvalue += ch;
                    }
                    else if (step==2)
                    {
                        yvalue += ch;
                    }
                    else if (step==3)
                    {
                        zvalue += ch;
                    }
                }


            pointsList.Add(new DataLine(label, float.Parse(xvalue), float.Parse(yvalue), float.Parse(zvalue))); //risky, should use try parses
            line = sr.ReadLine();
            }
        }

        return pointsList;
    }

}
