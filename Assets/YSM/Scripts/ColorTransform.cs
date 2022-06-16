using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace YSM
{
    public enum PlayerColorType
    {
        BLACK,
        GRAY,
        RED_THICK,
        RED,
        ORANGE,
        YELLOW,
        GREEN,
        BLUE,
        NAVY,
        PURPLE,
        WHITE,
        GRAY_LIGHT,
        BROWN,
        PINK,
        GOLD,
        YELLOW_LIGHT,
        LIME,
        BLUE_LIGHT,
        GRAY_BLUE,
        PURPLE_LIGHT,
        COUNT, //오류
    }


    public class ColorTransform :MonoBehaviour
    {

        static public string EnumToString(PlayerColorType index)
        {
            int idx = (int)index;
            switch (idx)
            {
                case (int)PlayerColorType.BLACK:            return "검정";
                case (int)PlayerColorType.GRAY:             return "회색";
                case (int)PlayerColorType.RED_THICK:        return "진한빨강";
                case (int)PlayerColorType.RED:              return "빨강";
                case (int)PlayerColorType.ORANGE:           return "주황";
                case (int)PlayerColorType.YELLOW:           return "노랑";
                case (int)PlayerColorType.GREEN:            return "초록";
                case (int)PlayerColorType.BLUE:             return "하늘";
                case (int)PlayerColorType.NAVY:             return "남색";
                case (int)PlayerColorType.PURPLE:           return "보라";
                case (int)PlayerColorType.WHITE:            return "흰색";
                case (int)PlayerColorType.GRAY_LIGHT:       return "연한회색";
                case (int)PlayerColorType.BROWN:            return "갈색";
                case (int)PlayerColorType.PINK:             return "분홍";
                case (int)PlayerColorType.GOLD:             return "황금색";
                case (int)PlayerColorType.YELLOW_LIGHT:     return "연한노랑";
                case (int)PlayerColorType.LIME:             return "라임";
                case (int)PlayerColorType.BLUE_LIGHT:       return "밝은청";
                case (int)PlayerColorType.GRAY_BLUE:        return "청회색";
                case (int)PlayerColorType.PURPLE_LIGHT:     return "연한보라";
                default: return "오류";
            }
        }
        
        static public Color EnumToColor(PlayerColorType index)
        {
            int idx = (int)index;
            switch (idx)
            {
                case (int)PlayerColorType.BLACK:            return new Color32(0, 0, 0, 255);
                case (int)PlayerColorType.GRAY:             return new Color32(127, 127, 127, 255);
                case (int)PlayerColorType.RED_THICK:        return new Color32(136, 0, 21, 255);
                case (int)PlayerColorType.RED:              return new Color32(237, 28, 36, 255);
                case (int)PlayerColorType.ORANGE:           return new Color32(255, 127, 39, 255);
                case (int)PlayerColorType.YELLOW:           return new Color32(255, 242, 0, 255);
                case (int)PlayerColorType.GREEN:            return new Color32(34, 177, 76, 255);
                case (int)PlayerColorType.BLUE:             return new Color32(0, 162, 232, 255);
                case (int)PlayerColorType.NAVY:             return new Color32(63, 72, 204, 255);
                case (int)PlayerColorType.PURPLE:           return new Color32(163, 73, 164, 255);
                case (int)PlayerColorType.WHITE:            return new Color32(255, 255, 255, 255);
                case (int)PlayerColorType.GRAY_LIGHT:       return new Color32(195, 195, 195, 255);
                case (int)PlayerColorType.BROWN:            return new Color32(185, 122, 87, 255);
                case (int)PlayerColorType.PINK:             return new Color32(255, 174, 201, 255);
                case (int)PlayerColorType.GOLD:             return new Color32(255, 201, 14, 255);
                case (int)PlayerColorType.YELLOW_LIGHT:     return new Color32(239, 228, 176, 255);
                case (int)PlayerColorType.LIME:             return new Color32(181, 230, 29, 255);
                case (int)PlayerColorType.BLUE_LIGHT:       return new Color32(153, 217, 234, 255);
                case (int)PlayerColorType.GRAY_BLUE:        return new Color32(112, 146, 190, 255);
                case (int)PlayerColorType.PURPLE_LIGHT:     return new Color32(200, 191, 231, 255);
                default: return Color.magenta;
            }
        }


        
        static public string EnumToTextString(PlayerColorType index)
        {
             int idx = (int)index;
             switch (idx)
             {
                 case (int)PlayerColorType.BLACK:            return Convert.ToString(0,  16) + Convert.ToString(0,  16) + Convert.ToString(0,  16);
                 case (int)PlayerColorType.GRAY:             return Convert.ToString(127,16) + Convert.ToString(127,16) + Convert.ToString(127,16);
                 case (int)PlayerColorType.RED_THICK:        return Convert.ToString(136,16) + Convert.ToString(0,  16) + Convert.ToString(21, 16);
                 case (int)PlayerColorType.RED:              return Convert.ToString(237,16) + Convert.ToString(28, 16) + Convert.ToString(36, 16);
                 case (int)PlayerColorType.ORANGE:           return Convert.ToString(255,16) + Convert.ToString(127,16) + Convert.ToString(39, 16);
                 case (int)PlayerColorType.YELLOW:           return Convert.ToString(255,16) + Convert.ToString(242,16) + Convert.ToString(0,  16);
                 case (int)PlayerColorType.GREEN:            return Convert.ToString(34, 16) + Convert.ToString(177,16) + Convert.ToString(76, 16);
                 case (int)PlayerColorType.BLUE:             return Convert.ToString(0,  16) + Convert.ToString(162,16) + Convert.ToString(232,16);
                 case (int)PlayerColorType.NAVY:             return Convert.ToString(63, 16) + Convert.ToString(72, 16) + Convert.ToString(204,16);
                 case (int)PlayerColorType.PURPLE:           return Convert.ToString(163,16) + Convert.ToString(73, 16) + Convert.ToString(164,16);
                 case (int)PlayerColorType.WHITE:            return Convert.ToString(255,16) + Convert.ToString(255,16) + Convert.ToString(255,16);
                 case (int)PlayerColorType.GRAY_LIGHT:       return Convert.ToString(195,16) + Convert.ToString(195,16) + Convert.ToString(195,16);
                 case (int)PlayerColorType.BROWN:            return Convert.ToString(185,16) + Convert.ToString(122,16) + Convert.ToString(87, 16);
                 case (int)PlayerColorType.PINK:             return Convert.ToString(255,16) + Convert.ToString(174,16) + Convert.ToString(201,16);
                 case (int)PlayerColorType.GOLD:             return Convert.ToString(255,16) + Convert.ToString(201,16) + Convert.ToString(14, 16);
                 case (int)PlayerColorType.YELLOW_LIGHT:     return Convert.ToString(239,16) + Convert.ToString(228,16) + Convert.ToString(176,16);
                 case (int)PlayerColorType.LIME:             return Convert.ToString(181,16) + Convert.ToString(230,16) + Convert.ToString(29, 16);
                 case (int)PlayerColorType.BLUE_LIGHT:       return Convert.ToString(153,16) + Convert.ToString(217,16) + Convert.ToString(234,16);
                 case (int)PlayerColorType.GRAY_BLUE:        return Convert.ToString(112,16) + Convert.ToString(146,16) + Convert.ToString(190,16);
                 case (int)PlayerColorType.PURPLE_LIGHT:     return Convert.ToString(200,16) + Convert.ToString(191,16) + Convert.ToString(231,16);
                 default: return "Error";
             }


        }

    }

    
}