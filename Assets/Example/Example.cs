//
//  Example.cs
//  PlayInstallReferrer
//
//  Created by Uglješa Erceg (@uerceg) on 12th April 2020.
//  Copyright © 2020-Present Uglješa Erceg. All rights reserved.
//

using System;
using UnityEngine;
using Ugi.PlayInstallReferrerPlugin;

public class Example : MonoBehaviour
{
    // same palette as uerceg.github.io
    private static readonly Color Background = new Color32(0x00, 0x00, 0x00, 0xFF);
    private static readonly Color Green = new Color32(0xAF, 0xFF, 0xA6, 0xFF);
    private static readonly Color Blue = new Color32(0xA4, 0xFF, 0xFF, 0xFF);
    private static readonly Color Dim = new Color32(0x77, 0x77, 0x77, 0xFF);
    private static readonly Color Danger = new Color32(0xFF, 0x8A, 0x80, 0xFF);

    private Texture2D pixel;
    private Font mono;

    private GUIStyle titleStyle, subtitleStyle, buttonStyle, keyStyle, valueStyle, errorStyle;
    private bool stylesReady;

    private PlayInstallReferrerDetails details;
    private bool waiting;

    private void OnDestroy()
    {
        Destroy(pixel);
        Destroy(mono);
    }

    private float Unit { get { return Mathf.Max(Screen.width, Screen.height) / 100f; } }

    private void BuildStyles()
    {
        pixel = new Texture2D(1, 1);
        pixel.SetPixel(0, 0, Color.white);
        pixel.Apply();

        int body = Mathf.RoundToInt(Unit * 1.5f);
        mono = Font.CreateDynamicFontFromOSFont(
            new[] { "Source Code Pro", "DroidSansMono", "Droid Sans Mono", "Courier New", "monospace" }, body);

        titleStyle = Style(Mathf.RoundToInt(Unit * 2.0f), Green, FontStyle.Bold);
        subtitleStyle = Style(Mathf.RoundToInt(Unit * 1.3f), Dim, FontStyle.Normal);
        keyStyle = Style(Mathf.RoundToInt(Unit * 1.4f), Dim, FontStyle.Normal);
        valueStyle = Style(body, Blue, FontStyle.Bold);
        errorStyle = Style(body, Danger, FontStyle.Bold);

        buttonStyle = Style(Mathf.RoundToInt(Unit * 1.7f), Green, FontStyle.Bold);
        buttonStyle.alignment = TextAnchor.MiddleLeft;
        buttonStyle.wordWrap = false;
        buttonStyle.hover.textColor = Green;
        buttonStyle.active.textColor = Green;

        stylesReady = true;
    }

    private GUIStyle Style(int size, Color color, FontStyle fontStyle)
    {
        var s = new GUIStyle { fontSize = size, wordWrap = true, fontStyle = fontStyle };
        if (mono != null) s.font = mono;
        s.normal.textColor = color;
        return s;
    }

    private void OnGUI()
    {
        if (!stylesReady) BuildStyles();

        Fill(new Rect(0, 0, Screen.width, Screen.height), Background);

        // start below the status bar / notch, never under it
        Rect safe = Screen.safeArea;
        float top = (Screen.height - safe.yMax) + Unit * 3f;
        float side = Unit * 3f;
        float x = safe.x + side;
        float width = safe.width - side * 2f;

        float y = top;
        y += Line(new Rect(x, y, width, 0f), "# play install referrer", titleStyle);
        y += Unit * 1.2f;
        y += Line(new Rect(x, y, width, 0f),
            waiting ? "reading..." : "tap to read the install referrer", subtitleStyle);

        y += Unit * 3f;
        string label = waiting ? "[ working ]" : "[ get install referrer ]";
        float buttonH = buttonStyle.CalcHeight(new GUIContent(label), width) + Unit * 1.6f;
        GUI.enabled = !waiting;
        if (GUI.Button(new Rect(x, y, width, buttonH), label, buttonStyle)) Read();
        GUI.enabled = true;
        y += buttonH + Unit * 3f;

        if (details == null)
        {
            Line(new Rect(x, y, width, 0f), waiting ? "> waiting for callback" : "> nothing read yet", keyStyle);
            return;
        }

        if (details.Error != null)
        {
            string msg = "> error, response code " + details.Error.ResponseCode;
            if (details.Error.Exception != null) msg += "\n  " + details.Error.Exception.Message;
            Line(new Rect(x, y, width, 0f), msg, errorStyle);
            return;
        }

        y += Field(x, y, width, "install referrer", details.InstallReferrer);
        y += Field(x, y, width, "referrer click", details.ReferrerClickTimestampSeconds.ToString());
        y += Field(x, y, width, "install begin", details.InstallBeginTimestampSeconds.ToString());
        y += Field(x, y, width, "referrer click (server)", details.ReferrerClickTimestampServerSeconds.ToString());
        y += Field(x, y, width, "install begin (server)", details.InstallBeginTimestampServerSeconds.ToString());
        y += Field(x, y, width, "install version", details.InstallVersion);
        y += Field(x, y, width, "google play instant", details.GooglePlayInstant.ToString());
    }

    // draws a label at its natural height and returns that height
    private float Line(Rect rect, string text, GUIStyle style)
    {
        float h = style.CalcHeight(new GUIContent(text), rect.width);
        GUI.Label(new Rect(rect.x, rect.y, rect.width, h), text, style);
        return h;
    }

    private float Field(float x, float y, float width, string key, string value)
    {
        float h = Line(new Rect(x, y, width, 0f), key, keyStyle);
        h += Unit * 1f;
        h += Line(new Rect(x, y + h, width, 0f), string.IsNullOrEmpty(value) ? "-" : value, valueStyle);
        return h + Unit * 2.4f;
    }

    private void Read()
    {
        waiting = true;
        details = null;
        PlayInstallReferrer.GetInstallReferrerInfo(result =>
        {
            waiting = false;
            details = result;
            if (result == null) { Debug.LogError("No install referrer details delivered"); return; }
            if (result.Error != null)
            {
                Debug.LogError("Install referrer error, response code: " + result.Error.ResponseCode);
                if (result.Error.Exception != null) Debug.LogError(result.Error.Exception);
                return;
            }
            Debug.Log("Install referrer: " + result.InstallReferrer);
        });
    }

    private void Fill(Rect r, Color c)
    {
        Color old = GUI.color;
        GUI.color = c;
        GUI.DrawTexture(r, pixel);
        GUI.color = old;
    }
}
