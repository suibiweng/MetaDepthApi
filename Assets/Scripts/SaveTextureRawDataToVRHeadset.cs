using UnityEngine;
using System.IO;
using OVR.Input;

public class SaveTextureRawDataToVRHeadset : MonoBehaviour
{
    public Texture sourceTexture; // Assign your source texture in the inspector
    public AudioSource audioSource;

    void Start()
    {

    }

    void SaveTextureAsBMP(Texture texture, string filePath)
    {
        // Create a new Texture2D with the same dimensions as the source texture
        Texture2D texture2D = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);

        // If the texture is a Texture2D, we can use GetRawTextureData directly
        if (texture is Texture2D)
        {
            texture2D = (Texture2D)texture;
        }
        else
        {
            // If the texture is not a Texture2D, we need to render it onto a RenderTexture and read the pixels
            RenderTexture renderTexture = new RenderTexture(texture.width, texture.height, 24);
            Graphics.Blit(texture, renderTexture);
            RenderTexture.active = renderTexture;
            texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            texture2D.Apply();
            RenderTexture.active = null;
            renderTexture.Release();
        }

        // Encode the Texture2D to BMP
        byte[] bytes = EncodeToBMP(texture2D);

        // Write the bytes to a file
        File.WriteAllBytes(filePath, bytes);

        Debug.Log("Saved BMP file to: " + filePath);
    }

    byte[] EncodeToBMP(Texture2D texture2D)
    {
        Color32[] pixels = texture2D.GetPixels32();
        byte[] bmpBytes = new byte[54 + texture2D.width * texture2D.height * 3];
        int index = 0;

        // BMP Header
        bmpBytes[index++] = 0x42; // B
        bmpBytes[index++] = 0x4D; // M
        int fileSize = 54 + pixels.Length * 3;
        bmpBytes[index++] = (byte)(fileSize);
        bmpBytes[index++] = (byte)(fileSize >> 8);
        bmpBytes[index++] = (byte)(fileSize >> 16);
        bmpBytes[index++] = (byte)(fileSize >> 24);
        index += 4; // Reserved
        bmpBytes[index++] = 54; // Offset to bitmap data
        index += 3;

        // DIB Header
        bmpBytes[index++] = 40; // DIB header size
        index += 3;
        bmpBytes[index++] = (byte)(texture2D.width);
        bmpBytes[index++] = (byte)(texture2D.width >> 8);
        bmpBytes[index++] = (byte)(texture2D.width >> 16);
        bmpBytes[index++] = (byte)(texture2D.width >> 24);
        bmpBytes[index++] = (byte)(texture2D.height);
        bmpBytes[index++] = (byte)(texture2D.height >> 8);
        bmpBytes[index++] = (byte)(texture2D.height >> 16);
        bmpBytes[index++] = (byte)(texture2D.height >> 24);
        bmpBytes[index++] = 1; // Number of color planes
        bmpBytes[index++] = 0;
        bmpBytes[index++] = 24; // Bits per pixel
        bmpBytes[index++] = 0;
        index += 24; // Ignore rest of DIB header

        // Bitmap data
        for (int y = 0; y < texture2D.height; y++)
        {
            for (int x = 0; x < texture2D.width; x++)
            {
                Color32 pixel = pixels[(texture2D.height - y - 1) * texture2D.width + x];
                bmpBytes[index++] = pixel.b;
                bmpBytes[index++] = pixel.g;
                bmpBytes[index++] = pixel.r;
            }
        }

        return bmpBytes;
    }



    void Update(){

        if(OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger)){


                    string filePath = Application.persistentDataPath+"SavedTexture.bmp";
        SaveTextureAsBMP(sourceTexture, filePath);


        audioSource.Play();


        }




    }
}
