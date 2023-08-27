using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using ReLogic.Threading;
using Terraria;
using Terraria.Graphics.Light;
using Terraria.ModLoader;
using Vector3 = System.Numerics.Vector3;
using XnaVector3 = Microsoft.Xna.Framework.Vector3;

namespace Hikari.Content.Lighting;

public sealed class HikariLightingEngine : ILightingEngine {
    private enum EngineState {
        MinimapUpdate,
        ExportMetrics,
        Scan,
        Blur,
        Max,
    }

    private const int area_padding = 28;
    private const int non_visible_padding = 18;
    private readonly Dictionary<Point, Vector3> perFrameLights = new();
    private readonly HikariTileLightScanner tileScanner = new();
    private HikariLightMap activeLightMap = new();
    private Rectangle activeProcessedArea = Rectangle.Empty;
    private HikariLightMap workingLightMap = new();
    private Rectangle workingProcessedArea = Rectangle.Empty;
    private EngineState state = EngineState.MinimapUpdate;

    public void Rebuild() {
        activeProcessedArea = Rectangle.Empty;
        workingProcessedArea = Rectangle.Empty;
        state = EngineState.MinimapUpdate;
        activeLightMap.Clear();
        workingLightMap.Clear();
    }

    public unsafe void AddLight(int x, int y, XnaVector3 color) {
        perFrameLights[new Point(x, y)] = FromXnaVector3(&color);
    }

    public void ProcessArea(Rectangle area) {
        Main.renderCount = (Main.renderCount + 1) % 4;

        switch (state) {
            case EngineState.MinimapUpdate:
                if (Main.mapDelay > 0)
                    Main.mapDelay--;
                else
                    ExportToMiniMap();
                break;

            case EngineState.ExportMetrics:
                area.Inflate(area_padding, area_padding);
                Main.SceneMetrics.ScanAndExportToMain(new SceneMetricsScanSettings {
                    VisualScanArea = area,
                    BiomeScanCenterPositionInWorld = Main.LocalPlayer.Center,
                    ScanOreFinderData = Main.LocalPlayer.accOreFinder,
                });
                break;

            case EngineState.Scan:
                ProcessScan(area);
                break;

            case EngineState.Blur:
                ProcessBlur();
                Present();
                break;

            case EngineState.Max:
                break;

            default:
                throw new InvalidOperationException();
        }

        state = (EngineState)(((int)state + 1) % (int)EngineState.Max);
    }

    public unsafe XnaVector3 GetColor(int x, int y) {
        if (!activeProcessedArea.Contains(x, y))
            return XnaVector3.Zero;

        x -= activeProcessedArea.X;
        y -= activeProcessedArea.Y;

        var color = activeLightMap[x, y];
        return ToXnaVector3(&color);
    }

    public void Clear() {
        activeLightMap.Clear();
        workingLightMap.Clear();
        perFrameLights.Clear();
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private void ExportToMiniMap() {
        if (!Main.mapEnabled || activeProcessedArea.Width <= 0 || activeProcessedArea.Height <= 0)
            return;

        var area = new Rectangle(activeProcessedArea.X + area_padding, activeProcessedArea.Y + area_padding, activeProcessedArea.Width - area_padding * 2, activeProcessedArea.Height - area_padding * 2);
        var value = new Rectangle(0, 0, Main.maxTilesX, Main.maxTilesX);
        value.Inflate(-40, -40);
        area = Rectangle.Intersect(area, value);
        Main.mapMinX = area.Left;
        Main.mapMinY = area.Top;
        Main.mapMaxX = area.Right;
        Main.mapMaxY = area.Bottom;
        FastParallel.For(
            area.Left,
            area.Right,
            (start, end, _) => {
                for (var x = start; x < end; x++)
                for (var y = area.Top; y < area.Bottom; y++) {
                    var color = activeLightMap[x - activeProcessedArea.X, y - activeProcessedArea.Y];
                    var brightest = Math.Max(Math.Max(color.X, color.Y), color.Z);
                    var light = (byte)Math.Min(255, (int)(brightest * 255f));
                    Main.Map.UpdateLighting(x, y, light);
                }
            }
        );

        Main.updateMap = true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private void ProcessScan(Rectangle area) {
        area.Inflate(area_padding, area_padding);
        workingProcessedArea = area;
        workingLightMap.SetSize(area.Width, area.Height);
        workingLightMap.NonVisiblePadding = non_visible_padding;
        tileScanner.Update();
        tileScanner.ExportTo(
            area,
            workingLightMap,
            new TileLightScannerOptions {
                DrawInvisibleWalls = Main.ShouldShowInvisibleWalls(),
            }
        );
    }

    private void ProcessBlur() {
        UpdateLightDecay();
        ApplyPerFrameLights();
        workingLightMap.Blur();
    }

    private void UpdateLightDecay() {
        workingLightMap.LightDecayThroughAir = 0.91f;
        workingLightMap.LightDecayThroughSolid = 0.56f;
        workingLightMap.LightDecayThroughHoney = new Vector3(0.75f, 0.7f, 0.6f) * 0.91f;

        switch (Main.waterStyle) {
            case 0:
            case 1:
            case 7:
            case 8:
                workingLightMap.LightDecayThroughWater = new Vector3(0.88f, 0.96f, 1.015f) * 0.91f;
                break;

            case 2:
                workingLightMap.LightDecayThroughWater = new Vector3(0.94f, 0.85f, 1.01f) * 0.91f;
                break;

            case 3:
                workingLightMap.LightDecayThroughWater = new Vector3(0.84f, 0.95f, 1.015f) * 0.91f;
                break;

            case 4:
                workingLightMap.LightDecayThroughWater = new Vector3(0.9f, 0.86f, 1.01f) * 0.91f;
                break;

            case 5:
                workingLightMap.LightDecayThroughWater = new Vector3(0.84f, 0.99f, 1.01f) * 0.91f;
                break;

            case 6:
                workingLightMap.LightDecayThroughWater = new Vector3(0.83f, 0.93f, 0.98f) * 0.91f;
                break;

            case 9:
                workingLightMap.LightDecayThroughWater = new Vector3(1f, 0.88f, 0.84f) * 0.91f;
                break;

            case 10:
                workingLightMap.LightDecayThroughWater = new Vector3(0.83f, 1f, 1f) * 0.91f;
                break;

            case 12:
                workingLightMap.LightDecayThroughWater = new Vector3(0.95f, 0.98f, 0.85f) * 0.91f;
                break;

            case 13:
                workingLightMap.LightDecayThroughWater = new Vector3(0.9f, 1f, 1.02f) * 0.91f;
                break;
        }

        var throughWaterR = workingLightMap.LightDecayThroughWater.X;
        var throughWaterG = workingLightMap.LightDecayThroughWater.Y;
        var throughWaterB = workingLightMap.LightDecayThroughWater.Z;
        LoaderManager.Get<WaterStylesLoader>().LightColorMultiplier(Main.waterStyle, 0.91f, ref throughWaterR, ref throughWaterB, ref throughWaterB);
        workingLightMap.LightDecayThroughWater = new Vector3(throughWaterR, throughWaterG, throughWaterB);

        if (Main.LocalPlayer.nightVision) {
            workingLightMap.LightDecayThroughAir *= 1.03f;
            workingLightMap.LightDecayThroughSolid *= 1.03f;
        }

        if (Main.LocalPlayer.blind) {
            workingLightMap.LightDecayThroughAir *= 0.95f;
            workingLightMap.LightDecayThroughSolid *= 0.95f;
        }

        if (Main.LocalPlayer.blackout || Main.LocalPlayer.headcovered) {
            workingLightMap.LightDecayThroughAir *= 0.85f;
            workingLightMap.LightDecayThroughSolid *= 0.85f;
        }

        workingLightMap.LightDecayThroughAir *= Player.airLightDecay;
        workingLightMap.LightDecayThroughSolid *= Player.solidLightDecay;

        var throughAir = 1f;
        var throughSolid = 1f;

        SystemLoader.ModifyLightingBrightness(ref throughAir, ref throughSolid);

        workingLightMap.LightDecayThroughAir *= throughAir;
        workingLightMap.LightDecayThroughSolid *= throughSolid;
    }

    private void ApplyPerFrameLights() {
        foreach (var (position, color) in perFrameLights) {
            if (!workingProcessedArea.Contains(position))
                continue;

            var b = workingLightMap[position.X - workingProcessedArea.X, position.Y - workingProcessedArea.Y];
            workingLightMap[position.X - workingProcessedArea.X, position.Y - workingProcessedArea.Y] = Vector3.Max(color, b);
        }

        perFrameLights.Clear();
    }

    private void Present() {
        Utils.Swap(ref activeLightMap, ref workingLightMap);
        Utils.Swap(ref activeProcessedArea, ref workingProcessedArea);
    }

    /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static int FastMax(int a, int b) {
        var diff = a - b;
        var sign = diff >> 31;
        return a - (diff & sign);
    }*/

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static unsafe Vector3 FromXnaVector3(XnaVector3* value) {
        return Unsafe.ReadUnaligned<Vector3>(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static unsafe XnaVector3 ToXnaVector3(Vector3* value) {
        return Unsafe.ReadUnaligned<XnaVector3>(value);
    }
}
