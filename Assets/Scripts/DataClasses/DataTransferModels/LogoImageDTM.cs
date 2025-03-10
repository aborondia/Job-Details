using System;
using Newtonsoft.Json;

public class LogoImageDTM
{
    public string imageBytes { get; set; }
    [JsonIgnore]
    public byte[] imageBytesContent;

    public LogoImageDTM(string imageBytes)
    {
        if (String.IsNullOrEmpty(imageBytes))
        {
            return;
        }

        this.imageBytes = imageBytes;
        this.imageBytesContent = System.Convert.FromBase64String(this.imageBytes);
    }
}
