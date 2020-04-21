using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Globalization;

public class TxtReader
{
    public static char separator = ','; //default

    public static List<Dimension> Read(string text)
    {
        List<Dimension> dimensionsList = new List<Dimension>();
        string[] lines = GetLinesFromText(text);

        string[] secondLine = lines[1].Replace("\"", String.Empty).Split(new string[] { separator.ToString(), "\"" }, StringSplitOptions.None); //purposedly not deleting empty values
        //so that both "", or , starts aren't counted in the line
        for (int k=0;k<secondLine.Length;k++) //but then needs to ignore the last empty value
        {
            if (IsQuantitativeVariable(secondLine[k]))
            {
                dimensionsList.Add(new QuantiDimension());
            }
            else
            {
                dimensionsList.Add(new QualiDimension());
            }
        }

        for (int i=1;i<lines.Length;i++)
        {
            string[] currentLine = lines[i].Replace("\"",String.Empty).Split(new string[] { separator.ToString(), "\"" }, StringSplitOptions.None); 
            for (int q=0;q<currentLine.Length;q++)
            {
                if (dimensionsList[q] is QuantiDimension)
                {
                    QuantiDimension qt = (QuantiDimension)dimensionsList[q];
                    if (IsQuantitativeVariable(currentLine[q])) qt.Values.Add(float.Parse(currentLine[q], CultureInfo.InvariantCulture));
                    else qt.Values.Add(float.NaN);
                }
                else
                {
                    QualiDimension ql = (QualiDimension)dimensionsList[q];
                    if (currentLine[q] != "") ql.Values.Add(currentLine[q]);
                    else ql.Values.Add("Incorrect value");
                }
            }
        }

        //if first column is indeed a header column, we just replace it with its QualiDimension equivalent; else we add it at the beginning of the dimensionsList
        if (IsLabelColumn(dimensionsList[0]))
        {
            dimensionsList[0] = QualiDimension.LabelColumn(lines.Length-1);
        }
        else
        {
            dimensionsList.Insert(0, QualiDimension.LabelColumn(lines.Length - 1));
        }

        for (int k = 1; k <dimensionsList.Count; k++)
        {
            dimensionsList[k].Label = GetVariablesLabels(text)[k-1];
        }

        return dimensionsList;
    }

    public static List<string> GetVariablesLabels(string text)
    {
        string labelLine = GetLinesFromText(text)[0];

        List<string> labels = new List<string>();

        foreach (string s in GetValuesFromLine(labelLine))
        {
            labels.Add(s);
        }

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
            if (!Single.IsNaN(l.XValue)) xValues.Add(l.XValue);
            if (!Single.IsNaN(l.YValue)) yValues.Add(l.YValue);
            if (!Single.IsNaN(l.ZValue)) zValues.Add(l.ZValue);
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
            if (Single.IsNaN(p.XValue) || Single.IsNaN(p.YValue) || Single.IsNaN(p.ZValue)) //if incorrect line, won't be visible
            {
                p.XStandardValue = -10f;
                p.YStandardValue = -10f;
                p.ZStandardValue = -10f;
                UnityEngine.Debug.Log("caught one !");
            }
            else
            {
                if (meansAndDeviations[1] != 0) p.XStandardValue = (p.XValue - meansAndDeviations[0]) / meansAndDeviations[1];
                else p.XStandardValue = 0;

                if (meansAndDeviations[3] != 0) p.YStandardValue = (p.YValue - meansAndDeviations[2]) / meansAndDeviations[3];
                else p.YStandardValue = 0;

                if (meansAndDeviations[5] != 0) p.ZStandardValue = (p.ZValue - meansAndDeviations[4]) / meansAndDeviations[5];
                else p.ZStandardValue = 0;
            }
        }

        return meansAndDeviations;
    }

    private static bool IsQuantitativeVariable(string textValue)
    {
        float x;
        return Single.TryParse(textValue, NumberStyles.Any, CultureInfo.InvariantCulture, out x);
    }

    private static bool IsLabelColumn(Dimension d)
    {
        if (d is QualiDimension)
        {
            QualiDimension ql = (QualiDimension)d;
            QualiDimension qlCompare = QualiDimension.LabelColumn(ql.Values.Count);
            for(int k=0;k<ql.Values.Count;k++)
            {
                if (ql.Values[k] != qlCompare.Values[k]) return false;
            }
        }
        else
        {
            QuantiDimension qt = (QuantiDimension)d;
            QuantiDimension qtCompare = QuantiDimension.LabelColumn(qt.Values.Count);
            for (int k = 0; k < qt.Values.Count; k++)
            {
                if (qt.Values[k] != qtCompare.Values[k]) return false;
            }
        }
        return true;
    }
    /// <summary>
    /// Based on second line, count the number of quanti and quali variables, and gives the number of individuals in the dataset. 
    /// </summary>
    /// <param name="text">Complete dataset to be analyzed</param>
    /// <returns>0 value is the count of quanti variables, 1 is for quali variable, 2 is for number of individuals, 3 is -1 if it doesn't have a header column, 0 otherwhise </returns>
    public static int[] CountSummary(string text)
    {
        int[] QuantiQualiIndividuals = new int[3];
        //UnityEngine.Debug.Log(text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries));

        string[] lines = GetLinesFromText(text);
        QuantiQualiIndividuals[2] = lines.Length - 1;

        int countQuali = 0;
        int countQuanti = 0;

        string[] secondLine = GetValuesFromLine(lines[1]);
        for (int i=0; i<secondLine.Length;i++) //doesn't count the first value which is the label
        {
            if (IsQuantitativeVariable(secondLine[i])) countQuanti++;
            else countQuali++;
        }

        //very dirty and last minute way to check if first value is actually a label
        QuantiDimension d0qt = new QuantiDimension();
        QualiDimension d0ql = new QualiDimension();
        if (IsQuantitativeVariable(secondLine[0])) d0ql = null;
        else d0qt = null;

        for (int i = 1; i < lines.Length; i++)
        {
            string[] currentLine = GetValuesFromLine(lines[i]);

            if (d0qt != null) d0qt.Values.Add(float.Parse(currentLine[0], CultureInfo.InvariantCulture));
            else d0ql.Values.Add(currentLine[0]);
        }
        if (d0qt != null && IsLabelColumn(d0qt)) countQuanti-=1;
        else if (d0ql != null && IsLabelColumn(d0ql)) countQuali-=1;

        QuantiQualiIndividuals[0] = countQuanti;
        QuantiQualiIndividuals[1] = countQuali;

        return QuantiQualiIndividuals;
    }

    private static string[] GetLinesFromText(string text)
    {
        return text.Split(new string[] { Environment.NewLine, "\n" }, StringSplitOptions.RemoveEmptyEntries);
    }

    private static string[] GetValuesFromLine(string line)
    {
        return line.Split(new string[] { separator.ToString(), "\"" }, StringSplitOptions.RemoveEmptyEntries);
    }
}
