using UnityEngine;

[RequireComponent(typeof(PngFilesPanelModel), typeof(PngFilesPanelView))]
public class PngFilesPanelController : MonoBehaviour
{
    #region Fields

    [SerializeField]
    private PngFilesPanelModel model;
    [SerializeField]
    private PngFilesPanelView view;

    #endregion

    #region Propeties

    #endregion

    #region Methods

    public void RefreshList()
    {
        model.RefreshTargetFiles();
    }

    private void Awake()
    {
        // Inicjalizacja.
        view.Init();
        model.Init();
    }

    private void OnDestroy()
    {
        view.Clean();
        model.Clean();
    }

#if UNITY_EDITOR

    private void Reset()
    {
        model = GetComponent<PngFilesPanelModel>();
        view = GetComponent<PngFilesPanelView>();
    }

#endif

    #endregion
}
