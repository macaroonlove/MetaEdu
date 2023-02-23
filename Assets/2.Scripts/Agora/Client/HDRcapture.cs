using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HDRcapture : MonoBehaviour
{
    public Camera mainCamera;
    public RenderTexture RenderTextureMono;
    public RenderTexture RenderTexture2DOut;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            VRCapture();
        }
    }

    public void VRCapture()
    {
        // 눈 사이의 거리 6.4cm 
        mainCamera.stereoSeparation = 0.064f;
        // facemask 63은 기본값으로 6개의 큐브맵 면을 모두 렌더링 한다는 의미
        mainCamera.RenderToCubemap(RenderTextureMono, 63, Camera.MonoOrStereoscopicEye.Mono);
        // 큐브맵을 equirect format으로 바꾼다. 
        RenderTextureMono.ConvertToEquirect(RenderTexture2DOut, Camera.MonoOrStereoscopicEye.Mono);
        // 2D 텍스쳐 크기를 정하고,
        Texture2D OutTexture = new Texture2D(RenderTexture2DOut.width, RenderTexture2DOut.height);
        // RenderTexture.active 설정은 Graphics.SetRenderTarget을 호출하는 것과 동일함. 변경할때 사용.
        RenderTexture.active = RenderTexture2DOut;
        // 텍스쳐에 읽어온다.
        OutTexture.ReadPixels(new Rect(0, 0, RenderTexture2DOut.width, RenderTexture2DOut.height), 0, 0);
        // 변경 닫을때 null 입력한다고 생각하는 개념으로
        RenderTexture.active = null;
        // 텍스쳐의 값을 jpg 형태로 변경
        byte[] bytes = OutTexture.EncodeToJPG();
        // 저장 위치
        string path = Application.dataPath + "/Test.jpg";
        // 파일 쓰기
        System.IO.File.WriteAllBytes(path, bytes);
    }
}
