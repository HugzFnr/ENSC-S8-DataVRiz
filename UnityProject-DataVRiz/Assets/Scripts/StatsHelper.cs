using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;

public static class StatsHelper
{
    public static float CalculateMean(List<float> values)
    {
        float sum = 0;
        foreach (float f in values)
        {
            if (!Single.IsNaN(f)) sum += f;
        }
        return (sum/values.Count);
    }

    //Calculates Standard deviation of the sample, which is a biased estimator of SD
    public static double CalculateSD(List<float> values)
    {
        float mean = CalculateMean(values);
        List<float> residuals = new List<float>();
        
        foreach (float f in values)
        {
            if (!Single.IsNaN(f))
            {
                double d = Convert.ToDouble(f - mean);
                float pow = (float)Math.Pow(d, 2.0);
                residuals.Add(pow);
            }
        }
        return Math.Sqrt(CalculateMean(residuals));
    }

}
