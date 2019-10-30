﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luo.Shared.Helper
{
    public static class Request
    {
        public static string HOST => "www.luoo.net";

        public static string GetAllVol => $"http://{HOST}/tag/?p=";

        public static string GetTagVol(string tag)
        {
            return $"http://{HOST}/tag/{tag}?p=";
        }


        public static string HOST_w => "www.luoow.com";

        public static string GetAllVol_w => $"https://{HOST_w}/";

        public static string RequestParse(int page)
        {
            switch (page)
            {
                case 1:
                    return "901_1000.html";
                case 2:
                    return "801_900.html";
                case 3:
                    return "701_800.html";
                case 4:
                    return "601_700.html";
                case 5:
                    return "501_600.html";
                case 6:
                    return "401_500.html";
                case 7:
                    return "301_400.html";
                case 8:
                    return "201_300.html";
                case 9:
                    return "101_200.html";
                case 10:
                    return "1_100.html";
                case 11:
                    return "r/";
                case 12:
                    return "e/";
                default:
                    return "r";
            }
        }

        //example: tag="世界音乐"
        public static string GetTagVol_w(string tag)
        {
            return $"https://{HOST}/tag/{tag}";
        }


        //public static string SearchImages => $"http://{HOST}/search/photos?";

        //public static string GetRandomImages => $"http://{HOST}/photos/random?";

        //public static string GetCategories => $"http://{HOST}/categories?";

        //public static string GetFeaturedImages => $"http://{HOST}/collections/featured?";

        //public static string GetImageDetail => $"http://{HOST}/photos/";

        //public static string GetTodayWallpaper => "https://juniperphoton.net/myersplash/wallpapers";

        //public static string GetTodayThumbWallpaper => "https://juniperphoton.net/myersplash/wallpapers/thumbs";
    }
}
