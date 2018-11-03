using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luo.Shared.Helper
{
    public static class Request
    {
        public static string HOST => "www.luoo.net";

        public static string GetAllVol => $"http://{HOST}/music/";

        public static string SearchImages => $"https://{HOST}/search/photos?";

        public static string GetRandomImages => $"https://{HOST}/photos/random?";

        public static string GetCategories => $"https://{HOST}/categories?";

        public static string GetFeaturedImages => $"https://{HOST}/collections/featured?";

        public static string GetImageDetail => $"https://{HOST}/photos/";

        public static string GetTodayWallpaper => "https://juniperphoton.net/myersplash/wallpapers";

        public static string GetTodayThumbWallpaper => "https://juniperphoton.net/myersplash/wallpapers/thumbs";
    }
}
