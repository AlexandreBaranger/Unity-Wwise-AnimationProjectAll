using UnityEngine;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class RTPCCurveGenerator : MonoBehaviour
{
    public AnimationClip selectedAnimation;
    public string curveName;
    public bool autoGeneratePoints;
    [HideInInspector] public float curveValueInspector;
    private Dictionary<float, List<string>> uniqueCsvLines = new Dictionary<float, List<string>>();

    public void UpdateRTPCs(float time)
    {
        if (selectedAnimation == null) return;
        var bindings = UnityEditor.AnimationUtility.GetCurveBindings(selectedAnimation);
        foreach (var binding in bindings)
        {
            var curve = UnityEditor.AnimationUtility.GetEditorCurve(selectedAnimation, binding);
            if (curve != null)
            {
                float value = curve.Evaluate(time);

                if (binding.propertyName == curveName)
                {
                    curveValueInspector = value;
                    string formattedValue = FormatCurveValue(value);
                    string csvLine = $"{time.ToString("F5", CultureInfo.InvariantCulture)}_{formattedValue}";

                    if (!uniqueCsvLines.ContainsKey(time))
                    {
                        uniqueCsvLines[time] = new List<string>();
                    }
                    uniqueCsvLines[time].Add(csvLine);
                }
            }
        }
    }

    public void GenerateFinalCSV(string fileName)
    {
        if (autoGeneratePoints && selectedAnimation != null)
        {
            GenerateAutomaticPoints();
        }

        string directoryPath = Path.Combine(Application.streamingAssetsPath, "Audio");
        string filePath = Path.Combine(directoryPath, fileName);
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
        var sortedTimings = uniqueCsvLines.Keys.OrderBy(x => x).ToList();
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            foreach (var timing in sortedTimings)
            {
                foreach (var line in uniqueCsvLines[timing])
                {
                    writer.WriteLine(line);
                }
            }
        }
        Debug.Log("Curve values saved to " + filePath);
    }

    private void GenerateAutomaticPoints()
    {
        float animationLength = selectedAnimation.length;
        float interval = animationLength / 9; // 10 points including the start and end

        uniqueCsvLines.Clear(); // Clear previous data

        for (int i = 0; i < 10; i++)
        {
            float time = i * interval;
            UpdateRTPCs(time);
        }
    }

    private string FormatCurveValue(float value)
    {
        string formattedValue = value.ToString(CultureInfo.InvariantCulture);
        formattedValue = formattedValue.Replace(',', '.');
        int secondCommaIndex = formattedValue.IndexOf(',', formattedValue.IndexOf(',') + 1);
        if (secondCommaIndex != -1)
        {
            formattedValue = formattedValue.Substring(0, secondCommaIndex) + "_" + formattedValue.Substring(secondCommaIndex + 1);
        }
        return formattedValue;
    }
}
