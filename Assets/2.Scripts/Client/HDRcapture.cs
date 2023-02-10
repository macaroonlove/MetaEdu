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
        // �� ������ �Ÿ� 6.4cm 
        mainCamera.stereoSeparation = 0.064f;
        // facemask 63�� �⺻������ 6���� ť��� ���� ��� ������ �Ѵٴ� �ǹ�
        mainCamera.RenderToCubemap(RenderTextureMono, 63, Camera.MonoOrStereoscopicEye.Mono);
        // ť����� equirect format���� �ٲ۴�. 
        RenderTextureMono.ConvertToEquirect(RenderTexture2DOut, Camera.MonoOrStereoscopicEye.Mono);
        // 2D �ؽ��� ũ�⸦ ���ϰ�,
        Texture2D OutTexture = new Texture2D(RenderTexture2DOut.width, RenderTexture2DOut.height);
        // RenderTexture.active ������ Graphics.SetRenderTarget�� ȣ���ϴ� �Ͱ� ������. �����Ҷ� ���.
        RenderTexture.active = RenderTexture2DOut;
        // �ؽ��Ŀ� �о�´�.
        OutTexture.ReadPixels(new Rect(0, 0, RenderTexture2DOut.width, RenderTexture2DOut.height), 0, 0);
        // ���� ������ null �Է��Ѵٰ� �����ϴ� ��������
        RenderTexture.active = null;
        // �ؽ����� ���� jpg ���·� ����
        byte[] bytes = OutTexture.EncodeToJPG();
        // ���� ��ġ
        string path = Application.dataPath + "/Test.jpg";
        // ���� ����
        System.IO.File.WriteAllBytes(path, bytes);
    }
}
