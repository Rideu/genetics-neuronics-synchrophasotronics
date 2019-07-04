﻿
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;

namespace Euristics
{

    public static class Control
    {

        private static MouseState OMS, NMS;
        private static KeyboardState OKS, NKS;
        public static float NMW, OMW, WheelVal;
        public static Vector2 MousePos => NMS.Position.ToVector2();

        public static bool LeftButtonPressed => NMS.LeftButton == ButtonState.Pressed;
        public static bool RightButtonPressed => NMS.RightButton == ButtonState.Pressed;
        public static bool PressedDownKey(Keys key) => (OKS.IsKeyUp(key) && NKS.IsKeyDown(key));

        public static void Update()
        {
            OKS = NKS;
            NKS = Keyboard.GetState();

            OMS = NMS;
            NMS = Mouse.GetState();

            OMW = NMW;
            NMW = Mouse.GetState().ScrollWheelValue;
            WheelVal = NMW - OMW;
        }

        public static bool MouseHoverOverG(Rectangle zone) => (Simplex.PtInsideRect(zone, MousePos));

        public static bool MouseHoverOverTex(Texture2D tex, Vector2 offset) => (Simplex.PtInsideRect(Simplex.OffsettedTexture(tex, offset), Mouse.GetState().Position.ToVector2()));

        public static bool LeftClickInTexture(Texture2D tex, Vector2 offset) => (LeftClick() && Simplex.PtInsideRect(Simplex.OffsettedTexture(tex, offset), Mouse.GetState().Position.ToVector2()));

        public static bool RightClickInTexture(Texture2D tex, Vector2 offset) => (RightClick() && Simplex.PtInsideRect(Simplex.OffsettedTexture(tex, offset), Mouse.GetState().Position.ToVector2()));

        public static bool LeftPressed() => Mouse.GetState().LeftButton == ButtonState.Pressed;

        public static bool RightPressed() => Mouse.GetState().RightButton == ButtonState.Pressed;

        public static bool MiddlePressed() => Mouse.GetState().MiddleButton == ButtonState.Pressed;

        public static bool LeftClick() => (OMS.LeftButton == ButtonState.Pressed && NMS.LeftButton == ButtonState.Released);
        
        public static bool RightClick() => (OMS.RightButton == ButtonState.Pressed && NMS.RightButton == ButtonState.Released);

        public static bool MidClick() => (OMS.MiddleButton == ButtonState.Pressed && NMS.MiddleButton == ButtonState.Released);

        public static bool PressedKey(Keys key) => (OKS.IsKeyDown(key) && NKS.IsKeyUp(key));

        public static bool AnyKeyPressed() => OKS.GetPressedKeys().Length > 0;

        public static bool IsKeyUp(Keys key) => Keyboard.GetState().IsKeyUp(key);

        public static bool IsKeyDown(Keys key) => Keyboard.GetState().IsKeyDown(key);

        public static Keys[] GetPressedKeys() => Keyboard.GetState().GetPressedKeys();

        public static void Debug(SpriteBatch batch)
        {
            //string presseds = "";
            //presseds = bindslist.FindAll(p => p.NewState == true).Count + "/" + bindslist.Capacity;

            //batch.DrawString(Game1.font, Mouse.GetState().Position+"", new Vector2(50), Color.White);
        }
    }
}
