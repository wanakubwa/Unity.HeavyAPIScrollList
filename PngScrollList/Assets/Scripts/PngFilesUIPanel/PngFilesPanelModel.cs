using System;
using System.IO;
using System.Threading;
using UnityEngine;

public class PngFilesPanelModel : MonoBehaviour
{
    #region Fields

    private const string TARGET_FILE_FILTER = "*.png";
    
    [SerializeField]
    private string pngFilesFolderName = "CustomPngFiles";

    #endregion

    #region Propeties

    public event Action OnFilesCollectionUpdate = delegate { };

    public FileInfo[] TargetFiles { get; private set; } = null;

    // Variables.
    private DirectoryInfo FilesRootDirectory { get; set; }
    private CancellationTokenSource ImageFilesPoolTokenSource { get; set; }

    #endregion

    #region Methods

    public void Init()
    {
        BuildFilesRootDirectoryPath();
        RefreshTargetFiles();
    }

    public void Clean()
    {
        ImageFilesPoolTokenSource?.Cancel();
    }

    public void RefreshTargetFiles()
    {
        ImageFilesPoolTokenSource?.Cancel();
        ImageFilesPoolTokenSource = new CancellationTokenSource();

        FetchFilesInTargetDirectory();
    }

    public void LoadPngFromFile(FileInfo file, Action<Texture2D> onFinishCallback)
    {
        SynchronizationContext mainThreadContext = SynchronizationContext.Current;

        // ThreadPool, poniewaz plikow moze byc bardzo duzo, dzieki czemu zastosowanie poola zapewni optymalne zarzadzanie watkami.
        // Zamiast threadpool mozna rowniez wykorzystac samo wywolanie metod async + await (np. ReadAllBytesAsync) lub z uzyciem bilioteki UniTask.
        // UniTask: https://github.com/Cysharp/UniTask

        // Pliki moga byc duze, a sama operacja zajac duzo czasu dlatego zastosowalem podejscie asynchroniczne.
        ThreadPool.QueueUserWorkItem(task =>
        {
            CancellationToken token = (CancellationToken)task;

            byte[] fileData = null;
            if (file.Exists)
            {
                fileData = File.ReadAllBytes(file.FullName);
            }

            if (token.IsCancellationRequested)
            {
                return;
            }

            // Powrot do glownego watku zeby skorzystac z API unity.
            mainThreadContext.Post(d => OnFileFinishReadHandlerMainThred(fileData, onFinishCallback), null);

        }, ImageFilesPoolTokenSource.Token);
    }

    // Handler po wczytaniu danych z pliku - wywolany z glownego watku.
    private void OnFileFinishReadHandlerMainThred(byte[] fileData, Action<Texture2D> callback)
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.LoadImage(fileData);

        callback(texture);
    }

    private void BuildFilesRootDirectoryPath()
    {
        string dataPath = Application.dataPath;
        dataPath = dataPath.Replace("/Assets", "");
        dataPath = Path.Combine(dataPath, pngFilesFolderName);

        FilesRootDirectory = new DirectoryInfo(dataPath);
    }

    private void FetchFilesInTargetDirectory()
    {
        if(FilesRootDirectory.Exists == true)
        {
            // Odfiltrowanie tylko plikow .png.
            TargetFiles = FilesRootDirectory.GetFiles(TARGET_FILE_FILTER);
        }

        OnFilesCollectionUpdate();
    }

    #endregion
}
