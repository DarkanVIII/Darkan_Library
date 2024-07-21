using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ScreenSpaceOutlines : ScriptableRendererFeature
{

    [System.Serializable]
    class ScreenSpaceOutlineSettings
    {
        [Header("General Outline Settings")]

        public Color OutlineColor = Color.black;
        public bool IsPulsating;

        [Range(.1f, 10f)]
        public float PulseFrequency;

        [Range(0.0f, 20.0f)]
        public float OutlineScale = 1.0f;

        [Header("Depth Settings")]

        [Range(0.0f, 100.0f)]
        public float DepthThreshold = 1.5f;

        [Range(0.0f, 500.0f)]
        public float RobertsCrossMultiplier = 100.0f;

        [Header("Normal Settings")]

        [Range(0.0f, 1.0f)]
        public float NormalThreshold = 0.4f;

        [Header("Depth Normal Relation Settings")]

        [Range(0.0f, 2.0f)]
        public float SteepAngleThreshold = 0.2f;

        [Range(0.0f, 500.0f)]
        public float SteepAngleMultiplier = 25.0f;

        [Header("General Scene View Space Normal Texture Settings")]

        public RenderTextureFormat ColorFormat;
        public int DepthBufferBits;
        public FilterMode FilterMode;
        public Color BackgroundColor = Color.clear;

        [Header("View Space Normal Texture Object Draw Settings")]

        public PerObjectData PerObjectData;
        public bool EnableDynamicBatching;
        public bool EnableInstancing;
    }

    class ScreenSpaceOutlinePass : ScriptableRenderPass
    {
        readonly Material _screenSpaceOutlineMaterial;
        readonly ScreenSpaceOutlineSettings _settings;

        FilteringSettings _filteringSettings;

        readonly List<ShaderTagId> _shaderTagIdList;
        readonly Material _normalsMaterial;

        RTHandle _normals;
        RendererList _normalsRenderersList;

        RTHandle _temporaryBuffer;

        public ScreenSpaceOutlinePass(RenderPassEvent renderPassEvent, LayerMask layerMask, ScreenSpaceOutlineSettings settings)
        {
            _settings = settings;
            this.renderPassEvent = renderPassEvent;

            _screenSpaceOutlineMaterial = new Material(Shader.Find("Hidden/OutlineFullscreen"));
            _screenSpaceOutlineMaterial.SetColor("_OutlineColor", settings.OutlineColor);
            _screenSpaceOutlineMaterial.SetFloat("_OutlineScale", settings.OutlineScale);

            _screenSpaceOutlineMaterial.SetFloat("_DepthThreshold", settings.DepthThreshold);
            _screenSpaceOutlineMaterial.SetFloat("_RobertsCrossMultiplier", settings.RobertsCrossMultiplier);

            _screenSpaceOutlineMaterial.SetFloat("_NormalThreshold", settings.NormalThreshold);

            _screenSpaceOutlineMaterial.SetFloat("_SteepAngleThreshold", settings.SteepAngleThreshold);
            _screenSpaceOutlineMaterial.SetFloat("_SteepAngleMultiplier", settings.SteepAngleMultiplier);
            _screenSpaceOutlineMaterial.SetFloat("_PulseFrequency", settings.PulseFrequency);

            if (settings.IsPulsating)
                _screenSpaceOutlineMaterial.EnableKeyword("_ISPULSATING");
            else
                _screenSpaceOutlineMaterial.DisableKeyword("_ISPULSATING");

            _filteringSettings = new FilteringSettings(RenderQueueRange.opaque, layerMask);

            _shaderTagIdList = new List<ShaderTagId> {
                new("UniversalForward"),
                new("UniversalForwardOnly"),
                new("LightweightForward"),
                new("SRPDefaultUnlit")
            };

            _normalsMaterial = new Material(Shader.Find("Hidden/ViewSpaceNormals"));
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            // Normals
            RenderTextureDescriptor textureDescriptor = renderingData.cameraData.cameraTargetDescriptor;
            textureDescriptor.colorFormat = _settings.ColorFormat;
            textureDescriptor.depthBufferBits = _settings.DepthBufferBits;
            RenderingUtils.ReAllocateIfNeeded(ref _normals, textureDescriptor, _settings.FilterMode);

            // Color Buffer
            textureDescriptor.depthBufferBits = 0;
            RenderingUtils.ReAllocateIfNeeded(ref _temporaryBuffer, textureDescriptor, FilterMode.Bilinear);

            ConfigureTarget(_normals, renderingData.cameraData.renderer.cameraDepthTargetHandle);
            ConfigureClear(ClearFlag.Color, _settings.BackgroundColor);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (_screenSpaceOutlineMaterial == null || _normalsMaterial == null ||
                renderingData.cameraData.renderer.cameraColorTargetHandle.rt == null || _temporaryBuffer.rt == null)
                return;

            CommandBuffer cmd = CommandBufferPool.Get();
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();

            // Normals
            DrawingSettings drawSettings = CreateDrawingSettings(_shaderTagIdList, ref renderingData, renderingData.cameraData.defaultOpaqueSortFlags);
            drawSettings.perObjectData = _settings.PerObjectData;
            drawSettings.enableDynamicBatching = _settings.EnableDynamicBatching;
            drawSettings.enableInstancing = _settings.EnableInstancing;
            drawSettings.overrideMaterial = _normalsMaterial;

            RendererListParams normalsRenderersParams = new(renderingData.cullResults, drawSettings, _filteringSettings);
            _normalsRenderersList = context.CreateRendererList(ref normalsRenderersParams);
            cmd.DrawRendererList(_normalsRenderersList);

            // Pass in RT for Outlines shader
            cmd.SetGlobalTexture(Shader.PropertyToID("_SceneViewSpaceNormals"), _normals.rt);

            using (new ProfilingScope(cmd, new ProfilingSampler("ScreenSpaceOutlines")))
            {

                Blitter.BlitCameraTexture(cmd, renderingData.cameraData.renderer.cameraColorTargetHandle, _temporaryBuffer, _screenSpaceOutlineMaterial, 0);
                Blitter.BlitCameraTexture(cmd, _temporaryBuffer, renderingData.cameraData.renderer.cameraColorTargetHandle);
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public void Release()
        {
            CoreUtils.Destroy(_screenSpaceOutlineMaterial);
            CoreUtils.Destroy(_normalsMaterial);
            _normals?.Release();

            if (_temporaryBuffer != null)
                _temporaryBuffer.Release();
        }

    }

    [SerializeField]
    RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;

    [SerializeField]
    LayerMask outlinesLayerMask;

    [SerializeField]
    ScreenSpaceOutlineSettings outlineSettings = new();

    ScreenSpaceOutlinePass screenSpaceOutlinePass;

    public override void Create()
    {
        if (renderPassEvent < RenderPassEvent.BeforeRenderingPrePasses)
            renderPassEvent = RenderPassEvent.BeforeRenderingPrePasses;

        screenSpaceOutlinePass = new ScreenSpaceOutlinePass(renderPassEvent, outlinesLayerMask, outlineSettings);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(screenSpaceOutlinePass);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            screenSpaceOutlinePass?.Release();
        }
    }
}