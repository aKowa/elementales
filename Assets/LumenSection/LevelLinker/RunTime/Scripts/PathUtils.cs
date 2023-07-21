// Copyright (C) Lumen Section - All Rights Reserved
// Written by Nicolas Baillard <nicolas.baillard@gmail.com>
using UnityEngine;



namespace LumenSection.LevelLinker
{
public readonly struct Path
{
  // Constants
  private const char Separator = '/';

  // Internals
  private readonly string mPath;



  public Path(string path)
  {
    if (string.IsNullOrEmpty(path))
    {
      mPath = null;
      return;
    }

    // Remove trailing separator char if there is one
    if (path[path.Length - 1] == Separator)
      mPath = path.Substring(0, path.Length - 1);
    else
      mPath = path;
  }

  public Path(string[] tokens)
  {
    mPath = string.Join("/", tokens);
  }

  public Path(Path path, string name, string extension)
  {
    mPath = $"{path.ToUnix()}{Separator}{name}.{extension}";
  }

  public override bool Equals(object obj)
  {
    if (obj == null || !GetType().Equals(obj.GetType()))
    {
      return false;
    }
    else
    {
      Path p = (Path)obj;
      return mPath == p.mPath;
    }
  }

  public override int GetHashCode()
  {
    return mPath.GetHashCode();
  }

  public static implicit operator Path(string s)
  {
    return new Path(s);
  }

  public static bool operator ==(Path a, string b)
  {
    return a.mPath == b;
  }

  public static bool operator !=(Path a, string b)
  {
    return a.mPath != b;
  }

  public bool IsValid
  {
    get
    {
      return mPath != null;
    }
  }

  public string NameWithoutExtension
  {
    get
    {
      // Find the last dot
      int dotIndex = mPath.LastIndexOf('.');
      if (dotIndex < 0)
        return Name;

      // Find the last slash
      int slashIndex = mPath.LastIndexOf(Separator);
      if (slashIndex < 0)
      {
        return mPath.Substring(0, dotIndex);
      }
      else
      {
        Debug.Assert(slashIndex < dotIndex);
        return mPath.Substring(slashIndex + 1, dotIndex - (slashIndex + 1));
      }
    }
  }

  public string Name
  {
    get
    {
      // Find the last slash
      int slashIndex = mPath.LastIndexOf(Separator);
      if (slashIndex < 0)
      {
        return mPath;
      }
      else
      {
        return mPath.Substring(slashIndex + 1, mPath.Length - (slashIndex + 1));
      }
    }
  }

  public (Path Path, string Name, string Extension) Decompose()
  {
    // Find the last dot and the last slash
    int dotIndex   = mPath.LastIndexOf('.');
    int slashIndex = mPath.LastIndexOf(Separator);

    // Decompose
    return (mPath.Substring(0,                                    slashIndex >= 0 ? slashIndex : mPath.Length),
            mPath.Substring(slashIndex >= 0 ? slashIndex + 1 : 0, (dotIndex >= 0 ? dotIndex : mPath.Length) - (slashIndex >= 0 ? slashIndex + 1 : 0)),
            dotIndex >= 0 ? mPath.Substring(dotIndex + 1, mPath.Length - (dotIndex + 1)) : "");
  }

  public string ToUnix()
  {
    return mPath;
  }

  public string ToNative()
  {
    #if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
    return mPath.Replace(@"\", "/");
    #else
    return mPath;
    #endif
  }
}
}
