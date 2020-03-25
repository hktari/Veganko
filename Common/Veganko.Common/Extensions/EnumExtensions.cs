using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Veganko.Common.Extensions
{
    public static class EnumExtensionMethods
    {
        public static string GetName(this Enum GenericEnum)
        {
            Type genericEnumType = GenericEnum.GetType();
            MemberInfo[] memberInfo = genericEnumType.GetMember(GenericEnum.ToString());
            if ((memberInfo != null && memberInfo.Length > 0))
            {
                var _Attribs = memberInfo[0].GetCustomAttributes(typeof(System.Runtime.Serialization.EnumMemberAttribute), false);
                if ((_Attribs != null && _Attribs.Count() > 0))
                {
                    return ((System.Runtime.Serialization.EnumMemberAttribute)_Attribs.ElementAt(0)).Value;
                }
            }
            return GenericEnum.ToString();
        }
    }
}
