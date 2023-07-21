using BergamotaLibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SaveManager
{
    private static readonly string pastaDosSaves = Path.Combine(Application.persistentDataPath, "Saves");
    private static readonly string nomeDoArquivo = "monsterProject";

    private static string CaminhoDoArquivoDoSaveDasConfiguracoes()
    {
        return Path.Combine(pastaDosSaves, "configuracoes" + ".txt");
    }

    private static string CaminhoDoArquivoDoSave(int slot)
    {
        return Path.Combine(pastaDosSaves, nomeDoArquivo + slot.ToString() + ".txt");
    }

    public static void IniciarPasta()
    {
        if (Directory.Exists(pastaDosSaves) == false)
        {
            Directory.CreateDirectory(pastaDosSaves);
        }
    }

    public static bool SaveConfiguracoesExiste()
    {
        IniciarPasta();

        string caminhoDoArquivo = CaminhoDoArquivoDoSaveDasConfiguracoes();

        return File.Exists(caminhoDoArquivo);
    }

    public static bool SaveExiste(int slot)
    {
        IniciarPasta();

        string caminhoDoArquivo = CaminhoDoArquivoDoSave(slot);

        return File.Exists(caminhoDoArquivo);
    }

    public static bool Salvar(SaveData save, int slot)
    {
        IniciarPasta();

        string caminhoDoArquivo = CaminhoDoArquivoDoSave(slot);

        string texto = JsonUtility.ToJson(save);

        if(texto != null)
        {
            try
            {
                //File.WriteAllText(caminhoDoArquivo, texto);

                Criptografador.WriteFile(caminhoDoArquivo, texto);
            }
            catch (Exception o)
            {
                Debug.LogError("Algo deu errado na hora de criar o arquivo!\nCaminho: " + caminhoDoArquivo);
                Debug.LogError(o);
                return false;
            }

            return true;
        }

        Debug.LogError("Nao foi possivel salvar o arquivo! O arquivo de texto estava nulo.\nCaminho: " + caminhoDoArquivo);
        return false;
    }

    public static SaveData Carregar(int slot)
    {
        IniciarPasta();

        string caminhoDoArquivo = CaminhoDoArquivoDoSave(slot);

        string texto;

        try
        {
            //texto = File.ReadAllText(caminhoDoArquivo);

            texto = Criptografador.ReadFile(caminhoDoArquivo);
        }
        catch(FileNotFoundException)
        {
            Debug.LogError("O arquivo nao foi encontrado!\nCaminho: " + caminhoDoArquivo);
            return null;
        }
        catch (Exception o)
        {
            Debug.LogError("Algo deu errado na hora de ler o arquivo!\nCaminho: " + caminhoDoArquivo);
            Debug.LogError(o);
            return null;
        }

        if (texto != null)
        {
            SaveData save = JsonUtility.FromJson<SaveData>(texto);

            return save;
        }

        Debug.LogError("Nao foi possivel carregar o arquivo! O arquivo de texto estava nulo.\nCaminho: " + caminhoDoArquivo);
        return null;
    }

    public static void CarregarInformacoesDoSave(SaveData save, PlayerSO playerSO)
    {
        playerSO.CarregarInformacoes(save.playerSO);
        SceneSpawnManager.CarregarInformacoes(save.sceneInfo);
        BergamotaLibrary.Flags.CarregarFlags(save.flags);
    }

    public static bool SalvarConfiguracoes()
    {
        IniciarPasta();

        string caminhoDoArquivo = CaminhoDoArquivoDoSaveDasConfiguracoes();

        string texto = JsonUtility.ToJson(new ConfiguracoesSave());

        if (texto != null)
        {
            try
            {
                File.WriteAllText(caminhoDoArquivo, texto);
            }
            catch (Exception o)
            {
                Debug.LogError("Algo deu errado na hora de criar o arquivo!\nCaminho: " + caminhoDoArquivo);
                Debug.LogError(o);
                return false;
            }

            return true;
        }

        Debug.LogError("Nao foi possivel salvar o arquivo! O arquivo de texto estava nulo.\nCaminho: " + caminhoDoArquivo);
        return false;
    }

    public static bool CarregarConfiguracoes()
    {
        IniciarPasta();

        string caminhoDoArquivo = CaminhoDoArquivoDoSaveDasConfiguracoes();

        string texto;

        try
        {
            texto = File.ReadAllText(caminhoDoArquivo);
        }
        catch (FileNotFoundException)
        {
            Debug.LogError("O arquivo nao foi encontrado!\nCaminho: " + caminhoDoArquivo);
            return false;
        }
        catch (Exception o)
        {
            Debug.LogError("Algo deu errado na hora de ler o arquivo!\nCaminho: " + caminhoDoArquivo);
            Debug.LogError(o);
            return false;
        }

        if (texto != null)
        {
            ConfiguracoesSave configuracoesSave = JsonUtility.FromJson<ConfiguracoesSave>(texto);

            BergamotaLibrary.MusicManager.instance.SetVolume(configuracoesSave.volumeMusicas);
            BergamotaLibrary.SoundManager.instance.SetVolume(configuracoesSave.volumeEfeitosSonoros);

            return true;
        }

        Debug.LogError("Nao foi possivel carregar o arquivo! O arquivo de texto estava nulo.\nCaminho: " + caminhoDoArquivo);
        return false;
    }

    public static bool ExcluirSave(int slot)
    {
        IniciarPasta();

        string caminhoDoArquivo = CaminhoDoArquivoDoSave(slot);

        try
        {
            File.Delete(caminhoDoArquivo);
        }
        catch (Exception o)
        {
            Debug.LogError("Algo deu errado na hora de excluir o arquivo!\nCaminho: " + caminhoDoArquivo);
            Debug.LogError(o);
            return false;
        }

        return true;
    }
}
