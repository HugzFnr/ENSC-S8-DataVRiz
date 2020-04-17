﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Globalization;

public class TxtReader
{
    public static char separator = ','; //default

    public static List<DataLine> Read(string text)
    {
        List<DataLine> pointsList = new List<DataLine>();
        bool isLabelLine = true;    

            foreach (var line in text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
            { 
                if (isLabelLine)
                {
                //skip first line
                    isLabelLine = false;
                }
                else
                {            
                    string label="";
                    string xValue="";
                    string yValue="";
                    string zValue="";
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
                            xValue += replace;
                        }
                        else if (step==2)
                        {
                            yValue += replace;
                        }
                        else if (step==3)
                        {
                            zValue += replace;
                        }
                }

                float xParsedValue = float.Parse(xValue, CultureInfo.InvariantCulture);
                float yParsedValue = float.Parse(yValue, CultureInfo.InvariantCulture);
                float zParsedValue = float.Parse(zValue, CultureInfo.InvariantCulture);              


                //Debug.Log("tent x :" + xvalue + "\n tent y : " + yvalue + "\n tent z : " + zvalue);
                pointsList.Add(new DataLine(label,
                xParsedValue,
                yParsedValue,
                zParsedValue));                                                               
          
                }
        }

        return pointsList;
    }

    public static List<string> GetVariablesLabels(string text)
    {
        string labelLine = text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)[0];

        string buffer = "";
        char replace = ' ';

        List<string> labels = new List<string>();

        foreach (char ch in labelLine)
        {
            replace = ch; 
            if (ch == separator && buffer!="" && buffer != "\"\"") //for those datafiles starting labels line with their separator
            {
                labels.Add(buffer);
                buffer = "";
            }
            else if (buffer == "\"\"") //for csv having a name for the label column
            {
                buffer = "";
            }
            else
            {
                buffer += ch;
            }
        }
        labels.Add(buffer);

        return labels;
    }

    public static float[] StandardizeData(List<DataLine> points)
    {
        float[] meansAndDeviations = new float[6]; //in order : mean of x, sd of x, mean of y, sd of y, mean of z, sd of z
                                                   //build the xvalues float list then get the mean then use it

        //lists of floats used to get mean and sd
        List<float> xValues = new List<float>();
        List<float> yValues = new List<float>();
        List<float> zValues = new List<float>();

        foreach (DataLine l in points)
        {
            xValues.Add(l.XValue);
            yValues.Add(l.YValue);
            zValues.Add(l.ZValue);
        }

        meansAndDeviations[0] = StatsHelper.CalculateMean(xValues);
        meansAndDeviations[1] = (float) StatsHelper.CalculateSD(xValues);
        meansAndDeviations[2] = StatsHelper.CalculateMean(yValues);
        meansAndDeviations[3] =(float) StatsHelper.CalculateSD(yValues);
        meansAndDeviations[4] = StatsHelper.CalculateMean(zValues);
        meansAndDeviations[5] = (float) StatsHelper.CalculateSD(zValues);

        //to compare precision with R
        //foreach (float f in meansAndDeviations)
        //{
        //    UnityEngine.Debug.Log(f);
        //}

        foreach (DataLine p in points)
        {
            p.XStandardValue = (p.XValue - meansAndDeviations[0]) / meansAndDeviations[1];
            p.YStandardValue = (p.YValue - meansAndDeviations[2]) / meansAndDeviations[3];
            p.ZStandardValue = (p.ZValue - meansAndDeviations[4]) / meansAndDeviations[5];
        }

        return meansAndDeviations;
    }

    public static bool HasSomethingToDisplay(string text)
    {
        return true;
        //should have at least 1 quantitative variable
    }

    private static bool IsQuantitativeVariable(string textValue)
    {
        float x;
        return Single.TryParse(textValue, NumberStyles.Any, CultureInfo.InvariantCulture, out x);
    }

    public static int[] CountSummary(string text)
    {
        int[] QuantiQualiIndividuals = new int[3];
        //UnityEngine.Debug.Log(text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries));

        string[] lines = text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        QuantiQualiIndividuals[2] = lines.Length - 1;

        int countQuali = 0;
        int countQuanti = 0;

        string[] secondLine = lines[1].Split(separator);
        for (int i=1; i<secondLine.Length;i++) //doesn't count the first value which is the label
        {
            if (IsQuantitativeVariable(secondLine[i])) countQuanti++;
            else countQuali++;
        }

        QuantiQualiIndividuals[0] = countQuanti;
        QuantiQualiIndividuals[1] = countQuali;

        foreach (int i in QuantiQualiIndividuals) UnityEngine.Debug.Log(i);

        return QuantiQualiIndividuals;
    }


}
