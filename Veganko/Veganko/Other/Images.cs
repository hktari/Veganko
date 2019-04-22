using System;
using System.Collections.Generic;
using System.Text;

namespace Veganko.Other
{
    public static class Images
    {
        public static string GetProfileBackgroundImageById(string id)
        {
            switch (id)
            {
                case "0":
                    return "pbg_1.png";
                case "1":
                    return "pbg_2.png";
                case "2":
                    return "pbg_3.png";
                case "3":
                    return "pbg_4.png";
                case "4":
                    return "pbg_5.png";
                case "5":
                    return "pbg_6.png";
                default:
                    throw new ArgumentException("Invalid id for profile background!");
            }
        }

        internal static string GetProfileAvatarById(string avatarId)
        {
            switch (avatarId)
            {

                default:
                    break;
            }
        }
        //public static string GetAvatarImageById(string id)
        //{
        //    switch (id)
        //    {
        //        case "0":

        //        default:
        //            break;
        //    }
        //}
    }
}
