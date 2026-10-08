using System;
using System.IO;
using UnityEngine;

/// <summary>
/// Helper untuk membentuk URL video dari folder StreamingAssets yang kompatibel dengan:
/// 1. Unity Editor
/// 2. Standalone Windows/PC
/// 3. Android
/// 4. WebGL / Hosting Server
/// </summary>
public static class VideoUrlHelper
{
    /// <summary>
    /// Mengembalikan URL atau path yang tepat untuk VideoPlayer berdasarkan platform aktif.
    /// Tidak menggunakan path hardcode seperti localhost, C:/, D:/, atau Application.dataPath.
    /// </summary>
    /// <param name="fileName">Nama file video (contoh: "STAGE 1 - AWAL.mp4")</param>
    /// <returns>URL atau path yang siap di-assign ke VideoPlayer.url</returns>
    public static string GetStreamingAssetsUrl(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            return string.Empty;
        }

        // Jika sudah berupa URL web (http:// atau https://), gunakan langsung
        if (fileName.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
            fileName.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            return fileName;
        }

        // Bersihkan tanda slash / backslash di awal
        fileName = fileName.TrimStart('/', '\\');

#if UNITY_WEBGL && !UNITY_EDITOR
        // WebGL / Hosting:
        // Application.streamingAssetsPath mengembalikan "StreamingAssets" (relatif terhadap direktori index.html)
        // Browser Video Player mengakses video via HTTP GET dari web server hosting.
        string webStreamingPath = Application.streamingAssetsPath.Replace('\\', '/');
        if (!webStreamingPath.EndsWith("/"))
        {
            webStreamingPath += "/";
        }
        
        // Encode spasi dan karakter khusus untuk URL web yang valid
        string encodedFileName = Uri.EscapeDataString(fileName).Replace("+", "%20");
        return webStreamingPath + encodedFileName;
#elif UNITY_ANDROID && !UNITY_EDITOR
        // Android: streamingAssetsPath berbentuk jar:file://
        return Path.Combine(Application.streamingAssetsPath, fileName);
#else
        // Unity Editor & Windows Standalone:
        // Gunakan path file lokal langsung dari Application.streamingAssetsPath dengan awalan file:///
        string fullPath = Path.Combine(Application.streamingAssetsPath, fileName).Replace('\\', '/');
        return "file:///" + fullPath;
#endif
    }

    /// <summary>
    /// Memeriksa apakah file video ada di StreamingAssets (berlaku di Editor & Standalone PC).
    /// </summary>
    public static bool VideoFileExists(string fileName)
    {
        if (string.IsNullOrEmpty(fileName)) return false;
        fileName = fileName.TrimStart('/', '\\');

        string fullPath = Path.Combine(Application.streamingAssetsPath, fileName);
        return File.Exists(fullPath);
    }
}
