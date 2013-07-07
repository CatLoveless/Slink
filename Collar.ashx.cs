using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SLink
{
    /// <summary>
    /// Summary description for Slink
    /// </summary>
    public class Collar : SlinkAPIBridge
    {
        [SLink.SlinkMethod]
        public SLink.List GetOwner(Key owner)
        {
            return new SLink.List() { Value = {"Yvonne", "Panda"} };
        }
    }
}