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
        string[] lines = text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

        string[] secondLine = lines[1].Split(new string[] { separator.ToString(), "\"" }, StringSplitOptions.RemoveEmptyEntries);
        //so that both "", or , starts aren't counted in the line
        for (int k=0;k<secondLine.Length;k++)
        {
            if (IsQuantitativeVariable(secondLine[k]) & k!=0) dimensionsList.Add(new QuantiDimension()); //first column is always individual's label
            else dimensionsList.Add(new QualiDimension());
            if (k!=0) dimensionsList[k].Label = GetVariablesLabels(text)[k-1];
        }
        //dimensionsList.Add(new QualiDimension());
        dimensionsList[0].Label = "IndividualLabel";
        QualiDimension q0 = (QualiDimension)dimensionsList[0];
        q0.IsLabelColumn = true;

        for (int i=1;i<lines.Length;i++)
        {
            string[] currentLine = lines[i].Split(separator);
            for (int q=0;q<currentLine.Length;q++)
            {
                if (IsQuantitativeVariable(currentLine[q]) & q!=0)
                {
                    QuantiDimension qt = (QuantiDimension)dimensionsList[q];
                    qt.Values.Add(float.Parse(currentLine[q], CultureInfo.InvariantCulture));
                }
                else
                {
                    QualiDimension ql = (QualiDimension)dimensionsList[q];
                    ql.Values.Add(currentLine[q]);
                }
            }
        }

        return dimensionsList;
    }

    public static List<string> GetVariablesLabels(string text)
    {
        string labelLine = text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)[0];

        List<string> labels = new List<string>();

        foreach (string s in labelLine.Split(new string[] { separator.ToString(), "\"" }, StringSplitOptions.RemoveEmptyEntries))
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
            if (meansAndDeviations[1] != 0) p.XStandardValue = (p.XValue - meansAndDeviations[0]) / meansAndDeviations[1];
            else p.XStandardValue = 0;

            if (meansAndDeviations[3] != 0) p.YStandardValue = (p.YValue - meansAndDeviations[2]) / meansAndDeviations[3];
            else p.YStandardValue = 0;

            if (meansAndDeviations[5] != 0) p.ZStandardValue = (p.ZValue - meansAndDeviations[4]) / meansAndDeviations[5];
            else p.ZStandardValue = 0;
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
    /// <summary>
    /// Based on second line, count the number of quanti and quali variables, and gives the number of individuals in the dataset. 
    /// </summary>
    /// <param name="text">Complete dataset to be analyzed</param>
    /// <returns>0 value is the count of quanti variables, 1 is for quali variable, 2 is for number of individuals</returns>
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

        return QuantiQualiIndividuals;
    }


}
