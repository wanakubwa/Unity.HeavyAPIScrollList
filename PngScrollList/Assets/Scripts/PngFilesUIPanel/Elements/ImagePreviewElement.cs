using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ImagePreviewElement : MonoBehaviour
{
    #region Fields

    private const string SINCE_CREATION_TIME_FORMAT = "{0}D {1}H {2}M";

    [SerializeField]
    private TextMeshProUGUI fileNameText;
    [SerializeField]
    private TextMeshProUGUI sinceCreationTimeText;
    [SerializeField]
    private RawImage imagePreview;

    #endregion

    #region Propeties

    public void Init(System.IO.FileInfo file, PngFilesPanelModel model)
    {
        fileNameText.text = file.Name;
        SetSinceCreationTime(file);

        // Zaladowanie textury.
        model.LoadPngFromFile(file, OnImageLoadedCallback);
    }

    private void SetSinceCreationTime(System.IO.FileInfo file)
    {
        TimeSpan timeDelta = DateTime.Now.Subtract(file.CreationTime);
        sinceCreationTimeText.text = string.Format(SINCE_CREATION_TIME_FORMAT, timeDelta.Days, timeDelta.Hours, timeDelta.Minutes);
    }

    // Callbacks.
    private void OnImageLoadedCallback(Texture2D texture)
    {
        imagePreview.texture = texture;
    }

    #endregion

    #region Methods



    #endregion
}