using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using TMPro;
using UnityEngine.Windows.Speech;

public class SpeechToText : MonoBehaviour
{
    [SerializeField] private TMP_Text outputText;

    private DictationRecognizer dictationRecognizer;
    private StringBuilder recognizedText = new StringBuilder();

    private bool shouldRestart = false;

    void Start()
    {
        if (SpeechSystemStatus.Stopped == SpeechSystemStatus.Failed)
        {
            //Debug.LogError("DictationRecognizer is not supported on this device.");
            //return;
        }

        dictationRecognizer = new DictationRecognizer();

        dictationRecognizer.DictationResult += (text, confidence) =>
        {
            recognizedText.Append(text + " ");
            string[] strings = text.Split(' ');
            //float result = -1;

            Dictionary<string, float> wordToNumber = new Dictionary<string, float>()
            {
                {"zero", 0}, {"one", 1}, {"two", 2}, {"three", 3}, {"four", 4},
                {"five", 5}, {"six", 6}, {"seven", 7}, {"eight", 8}, {"nine", 9},
                {"ten", 10}, {"can", 10}, {"pin", 10}, {"fan", 10}, {"eleven", 11}, {"twelve", 12}, {"thirteen", 13},
                {"fourteen", 14}, {"fifteen", 15}, {"sixteen", 16}, {"seventeen", 17},
                {"eighteen", 18}, {"nineteen", 19}, {"twenty", 20}, {"thirty", 30},
                {"forty", 40}, {"fifty", 50}, {"sixty", 60}, {"seventy", 70},
                {"eighty", 80}, {"ninety", 90}
            };

            float result = -1;

            foreach (string s in strings)
            {
                string word = s.ToLower().Trim();
                float temp;

                if (float.TryParse(word, out temp))
                {
                    result = temp;
                    Debug.Log($"Parsed numeric string: {result}");
                    break;
                }
                else if (wordToNumber.TryGetValue(word, out temp))
                {
                    result = temp;
                    Debug.Log($"Parsed word as number: {result}");
                    break;
                }
            }

            if (result == -1)
            {
                Debug.Log("No number found in the string.");
                return;
            }

            if (result == -1)
            {
                Debug.Log("No number found in the string.");
                return;
            }

            float liters = result / 16f;
            float tanks = Mathf.Ceil(liters / 40f);
            outputText.text = $" km: {result} liters: {liters:F2} tanks: {tanks}";

            SpeakText($"Kilometers {result}, Liters {liters:F2}, Tanks {tanks}");
        };

        dictationRecognizer.DictationHypothesis += (text) =>
        {
            Debug.Log("Hypothesis: " + text);
        };

        dictationRecognizer.DictationComplete += (completionCause) =>
        {
            if (completionCause != DictationCompletionCause.Complete)
            {
                Debug.LogWarning("Dictation stopped unexpectedly: " + completionCause);
                shouldRestart = true;
            }

            if (shouldRestart)
            {
                shouldRestart = false;
                StartDictation();
            }
        };

        dictationRecognizer.DictationError += (error, hresult) =>
        {
            Debug.LogError("Error: " + error);
            shouldRestart = true;
            dictationRecognizer.Stop();
        };

        StartDictation();
    }


    void StartDictation()
    {
        if (dictationRecognizer != null && dictationRecognizer.Status != SpeechSystemStatus.Running)
        {
            recognizedText.Clear();
            if (outputText != null)
                outputText.text = "";

            dictationRecognizer.Start();
            Debug.Log("Dictation started.");
        }
    }

    void OnDestroy()
    {
        if (dictationRecognizer != null)
        {
            if (dictationRecognizer.Status == SpeechSystemStatus.Running)
            {
                dictationRecognizer.Stop();
            }
            dictationRecognizer.Dispose();
        }
    }

    void SpeakText(string message)
    {
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
        {
            FileName = "powershell",
            Arguments = $"-Command \"Add-Type –AssemblyName System.Speech; " +
                        $"$speak = New-Object System.Speech.Synthesis.SpeechSynthesizer; " +
                        $"$speak.Speak('{message}')\"",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        });
    }
}