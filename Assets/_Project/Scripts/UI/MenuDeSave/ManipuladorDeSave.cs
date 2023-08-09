using System.Collections;
using UnityEngine;

public class ManipuladorDeSave : MonoBehaviour
{
    //Componentes
    [SerializeField] private MenuCarregarController menuCarregar;

    //Variaveis
    private int slotAtual;

    public IEnumerator ShareSaveFile(int slot)
    {
        yield return new WaitForEndOfFrame();

        string filePath = SaveManager.CaminhoDoArquivoDoSave(slot);

        NativeFilePicker.Permission permission =  NativeFilePicker.ExportFile(filePath, success =>
        {
            Debug.LogWarning($"Exported: {success}");
        });
        
        Debug.LogWarning($"Permission result: {permission}");
    }
    
    public void ImportFile(int slot)
    {
        slotAtual = slot;

        string textFileType = NativeFilePicker.ConvertExtensionToFileType( "txt" );
        
        NativeFilePicker.Permission permission =  NativeFilePicker.PickFile(path =>
        {
            if (path == null)
            {
                Debug.LogWarning("Importing failed");
            }
            else
            {
                Debug.LogWarning($"Imported path: {path}");
                ApplyImportedSave(path);
            }
        }, new string[] {textFileType});
        
        Debug.LogWarning($"Permission result: {permission}");
    }

    private void ApplyImportedSave(string path)
    {
        menuCarregar.ImportarSave(path, slotAtual);
    }
}