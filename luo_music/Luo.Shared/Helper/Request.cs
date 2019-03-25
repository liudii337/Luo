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

        public static string GetAllVol => $"http://{HOST}/tag/?p=";

        public static string GetTagVol(string tag)
        {
            return $"http://{HOST}/tag/{tag}?p=";
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
