using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SLink
{
    public class SlinkFormat
    {

  public static string First = @"key {0}Id;
{0}({3})
{{ {0}Id=llHTTPRequest(""{1}"", [HTTP_METHOD,""POST""],{4}); }}

{0}Callback({2}  data){{   }}";

  public static string Second = @"if ({0}Id == request_id){{ {0}Callback({2}); }}";

  public static string SecondWrap = @"

default {{ 

http_response(key request_id, integer status, list metadata, string body){{ 
    {0}
    }} 

}}";


    }
}