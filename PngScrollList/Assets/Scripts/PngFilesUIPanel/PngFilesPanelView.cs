using System.Collections.Generic;
using UnityEngine;

public class PngFilesPanelView : MonoBehaviour
{
    #region Fields

    [SerializeField]
    private PngFilesPanelModel model;

    [Space]
    [SerializeField]
    private ImagePreviewElement imagePreviewElementPrefab;
    [SerializeField]
    private RectTransform imagePreviewElementParent;
    [SerializeField]
    private GameObject noFilesInfo;

    #endregion

    #region Propeties

    private List<ImagePreviewElement> SpawnedImagePreviewElements { get; set; } = new List<ImagePreviewElement>();

    #endregion

    #region Methods

    public void Init()
    {
        AttachEvents();
    }

    public void Clean()
    {
        DetachEvents();
    }

    private void RefreshPngFilesList()
    {
        ClearAllSpawnedImagesPreview();

        bool isAnyFileToDisplay = model.TargetFiles != null && model.TargetFiles.Length > 0;
        noFilesInfo.SetActive(!isAnyFileToDisplay);

        if (!isAnyFileToDisplay)
        {
            // Na widoku pozostaje jedynie informacja o braku plikow do wyswietlenia.
            return;
        }

        // Wyswietlenie istniejacych plikow.
        for (int i = 0; i < model.TargetFiles.Length; i++)
        {
            ImagePreviewElement newImagePreviewElement = SpawnNewImagePreviewElement();
            newImagePreviewElement.Init(model.TargetFiles[i], model);

            SpawnedImagePreviewElements.Add(newImagePreviewElement);
        }
    }

    private ImagePreviewElement SpawnNewImagePreviewElement()
    {
        ImagePreviewElement element = Instantiate(imagePreviewElementPrefab, imagePreviewElementParent);
        return element;
    }

    private void ClearAllSpawnedImagesPreview()
    {
        for (int i = SpawnedImagePreviewElements.Count - 1; i >= 0; i--)
        {
            Destroy(SpawnedImagePreviewElements[i].gameObject);
        }

        SpawnedImagePreviewElements.Clear();
    }

    private void AttachEvents()
    {
        model.OnFilesCollectionUpdate += RefreshPngFilesList;
    }

    private void DetachEvents()
    {
        model.OnFilesCollectionUpdate -= RefreshPngFilesList;
    }

    #endregion
}
