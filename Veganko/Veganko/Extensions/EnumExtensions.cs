using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Veganko.Extensions
{
    public static class EnumExtensionMethods
    {
        public static string GetDescription(this Enum GenericEnum)
        {
            Type genericEnumType = GenericEnum.GetType();
            MemberInfo[] memberInfo = genericEnumType.GetMember(GenericEnum.ToString());
            if ((memberInfo != null && memberInfo.Length > 0))
            {
                var _Attribs = memberInfo[0].GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
                if ((_Attribs != null && _Attribs.Count() > 0))
                {
                    return ((System.ComponentModel.DescriptionAttribute)_Attribs.ElementAt(0)).Description;
                }
            }
            return GenericEnum.ToString();
        }

        public static List<string> GetDescriptions(this Enum @enum)
        {
            List<string> desc = new List<string>();
            foreach (var enumVal in @enum.GetType().GetEnumValues())
            {
                desc.Add(GetDescription((Enum)enumVal));
            }

            return desc;
        }
    }
}
