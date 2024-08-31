#nullable enable
using System.Collections;
using System.Text;
using UnityEngine;

[ExecuteAlways]
public class AudioClipSpectrum : MonoBehaviour
{
    [SerializeField] AudioSource? audioSource;
    [SerializeField] AudioClip? audioClip;

    void OnValidate()
    {
        if (audioClip == null)
            return;

        float[] datas = new float[audioClip.samples * audioClip.channels];

        int length = 0;
        if (!audioClip.GetData(datas, 0))
            return;

        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < datas.Length; i += audioClip.frequency / 250 * audioClip.channels)
        {
            float sample = 0;
            for (int j = 0; j < audioClip.channels; j++)
                sample += Mathf.Abs(datas[i + j]);

            sb.AppendLine((sample / audioClip.channels * 100).ToString());
            length++;
        }

        GUIUtility.systemCopyBuffer = sb.ToString();
        Debug.Log(sb.ToString());
        Debug.Log(length);
        Debug.Log(datas.Length);
    }

    readonly StringBuilder sb = new StringBuilder();
    readonly float[] spectrum = new float[64];
    IEnumerator Start()
    {
        if (!Application.isPlaying || audioSource == null || audioClip == null)
            yield break;

        yield return new WaitForSeconds(1);

        sb.Clear();

        audioSource.clip = audioClip;
        audioSource.Play();

        int vSyncCount = QualitySettings.vSyncCount;
        int targetFrameRate = Application.targetFrameRate;

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 250;

        while (audioSource.isPlaying)
        {
            audioSource.GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);

            for (int i = 0; i < spectrum.Length; i++)
            {
                /*if (i > 0 && i < spectrum.Length - 1)
                {
                    UnityEngine.Debug.DrawLine(new Vector3(i - 1, Mathf.Log(spectrum[i]) + 10, 0), new Vector3(i, Mathf.Log(spectrum[i + 1]) + 10, 0), Color.red);
                    UnityEngine.Debug.DrawLine(new Vector3(i - 1, Mathf.Log10(spectrum[i]) + 10, 0), new Vector3(i, Mathf.Log10(spectrum[i + 1]) + 10, 0), Color.green);
                    UnityEngine.Debug.DrawLine(new Vector3(i - 1, spectrum[i - 1] * 100, 3), new Vector3(i, spectrum[i] * 100, 3), Color.blue);
                }*/

                float value = spectrum[i] * 1000;
                if (value > 0)
                    sb.Append(value);
                else
                    sb.Append(0);

                if (i < spectrum.Length - 1)
                    sb.Append(',');
            }

            sb.Append('\n');

            yield return null;
        }

        QualitySettings.vSyncCount = vSyncCount;
        Application.targetFrameRate = targetFrameRate;

        GUIUtility.systemCopyBuffer = sb.ToString();
        Debug.Log(sb.ToString());
    }
}
