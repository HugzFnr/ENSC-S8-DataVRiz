using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using System.Globalization;

public class TxtReader
{
    public static List<DataLine> Read(string text)
    {
        List<DataLine> pointsList = new List<DataLine>();
        char separator = ','; //default
  
        //Debug.Log(filePath);
        //Debug.Log("rep actif :" + Directory.GetCurrentDirectory());

        //first line will be used to name axis, but skipped for now
        bool isLabelLine = true;    

            foreach (var line in text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
            { 
                if (isLabelLine)
                {
                //assign labels
                isLabelLine = false;
                }
                else
                {            
                string label="";
                string xvalue="";
                string yvalue="";
                string zvalue="";
                char replace=' ';
                int step = 0; //0 is waiting for label; 1 is label assigned, 2 is xvalue assigned, 4 is complete

                foreach (char ch in line)
                {
                    replace = ch; //test pr le input string format wrong
                    if (ch == separator) step++;
                    else if (step == 0)
                    {
                        label += replace;
                    }
                    else if (step==1)
                    {
                        xvalue += replace;
                    }
                    else if (step==2)
                    {
                        yvalue += replace;
                    }
                    else if (step==3)
                    {
                        zvalue += replace;
                    }
                }
            

                //Debug.Log("tent x :" + xvalue + "\n tent y : " + yvalue + "\n tent z : " + zvalue);
                pointsList.Add(new DataLine(label,
                float.Parse(xvalue, CultureInfo.InvariantCulture)-170, //temp, before posiotining data
                float.Parse(yvalue, CultureInfo.InvariantCulture)-85,
                float.Parse(zvalue, CultureInfo.InvariantCulture))); //risky, should use try parses                                                                    
          
                }
        }

        return pointsList;
    }

}
