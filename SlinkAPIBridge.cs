using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SLink
{
    public class SlinkMethod : System.Attribute { }

    public class Vector3d
    {
        public Vector3d(float x, float y, float z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }
        public static bool TryParse(string inData, out Vector3d outdata)
        {
            bool result = true;
            outdata = null;
            System.Text.RegularExpressions.Regex rex = new System.Text.RegularExpressions.Regex("<([^,]*),([^,]*),([^,]*)>");
            if (rex.IsMatch(inData))
            {
                var match = rex.Match(inData);
                string x_ = match.Groups[1].Value;
                string y_ = match.Groups[2].Value;
                string z_ = match.Groups[3].Value;

                float x;
                float y;
                float z;



                if (!float.TryParse(x_, out x) || !float.TryParse(y_, out y) || !float.TryParse(z_, out z))
                {
                    result = false;
                }
                else
                {
                    outdata = new Vector3d(x, y, z);
                }



            }
            else
            {
                result = false;
            }
            return result;
        }



        public float X;
        public float Y;
        public float Z;
    }

    public class Rotation3d
    {
        public Rotation3d(float x, float y, float z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }
        public static bool TryParse(string inData, out Rotation3d outdata)
        {
            bool result = true;
            outdata = null;
            System.Text.RegularExpressions.Regex rex = new System.Text.RegularExpressions.Regex("<([^,]*),([^,]*),([^,]*)>");
            if (rex.IsMatch(inData))
            {
                var match = rex.Match(inData);
                string x_ = match.Groups[1].Value;
                string y_ = match.Groups[2].Value;
                string z_ = match.Groups[3].Value;

                float x;
                float y;
                float z;



                if (!float.TryParse(x_, out x) || !float.TryParse(y_, out y) || !float.TryParse(z_, out z))
                {
                    result = false;
                }
                else
                {
                    outdata = new Rotation3d(x, y, z);
                }



            }
            else
            {
                result = false;
            }
            return result;
        }



        public float X;
        public float Y;
        public float Z;
    }
   
    public interface SLType 
    {
        object ToObject();
        string ConvertMethod(string name);
    }
    public interface ISLType<T> : SLType
    {
        T Value { get; }
        void SetFromSL(string value);
     
    }



    public class Key : ISLType<Guid>
    {


        private Guid value;
        public Guid Value
        {
            get { return value; }
        }
        public void SetFromSL(string value)
        {
            Guid result;

            if (Guid.TryParse(value, out result))
            {
                this.value = result;
            }

        }
        public override string ToString()
        {
            return value.ToString();
        }

        public object ToObject(){ return value; }

        public string ConvertMethod(string name)
        {
            return string.Format("(key){0}", name);
        }
    }
    

    public class Integer : ISLType<Int32>
    {
        private Int32 value;
        public Int32 Value
        {
            get { return value; }
        }
        public void SetFromSL(string value)
        {
            Int32 result;

            if (Int32.TryParse(value, out result))
            {
                this.value = result;
            }

        }
        public override string ToString()
        {
            return value.ToString();
        }
        public object ToObject(){ return value; }
        public string ConvertMethod(string name)
        {
            return string.Format("(integer){0}", name);
        }
    }

    public class String : ISLType<System.String>
    {

        private System.String value;
  
        public override string ToString()
        {
            return value.ToString();
        }

        public System.String Value
        {
            get { return value; }
        }

        public void SetFromSL(System.String value)
        {

            this.value = value;


        }

        public object ToObject() { return value; }

        public string ConvertMethod(string name)
        {
            return  name;
        }
    }

    public class List : ISLType<List<System.String>>
    {
        public List()
        {
            this.value = new List<string>();
        }
        private System.Collections.Generic.List<string> value;
        public System.Collections.Generic.List<string> Value
        {
            get { return value; }
        }
        public void SetFromSL(string value)
        {
            this.value = value.Split(new char[] { ',' }).ToList();

        }
        public override string ToString()
        {
            return System.String.Join(",",value.Cast<object>().ToArray());
        }
        public object ToObject(){ return value; }

        public string ConvertMethod(string name)
        {
            return string.Format(@"llParseString2List({0},["",""],[])",name); 

        }
    }

    public class Vector : ISLType<Vector3d>
    {
    

      private Vector3d value;

      public Vector3d Value
        {
            get { return value; }
        }
        public void SetFromSL(string value)
        {
            Vector3d result;

            if (Vector3d.TryParse(value, out result))
            {
                this.value = result;
            }
    

        }
        public override string ToString()
        {
            return string.Format("<{0},{1},{2}>",value.X,value.Y,value.Z);
        }
                public object ToObject(){ return value; }

                public string ConvertMethod(string name)
                {
                    return string.Format("(vector){0}", name);

                }

    }

    public class Rotation : ISLType<Rotation3d>
    {
        private Rotation3d value;
        public Rotation3d Value
        {
            get { return value; }
        }
        public void SetFromSL(string value)
        {
            Rotation3d result;

            if (Rotation3d.TryParse(value, out result))
            {
                this.value = result;
            }


        }
        public override string ToString()
        {
            return string.Format("<{0},{1},{2}>", value.X, value.Y, value.Z);
        }
        public object ToObject(){ return value; }
        public string ConvertMethod(string name)
                {
                    return string.Format("(rotation){0}", name);
                }
    }

    public class SlinkAPIBridge : IHttpHandler
    {



        private bool ContainsAttribute<T>(object[] attributes)
        {
            var result = attributes.Count(n => n.GetType() == typeof(T)) > 0;
            return result;
        }
        private bool Inherits( Type derivedclass,Type baseclass)
        {
           return derivedclass.IsAssignableFrom(baseclass);

        }


        private KeyValuePair<string,string> GetSlType(System.Reflection.ParameterInfo info)
        {
            if (info.ParameterType.GetInterfaces().Contains(typeof(SLType)))
            {

                return new KeyValuePair<string, string>(info.Name, info.ParameterType.Name.ToLower());

            }
            else
            {
                throw new ApplicationException("incompatibleType");
            }
        
        }

        public static string Ip
        {
            get
            {

                string ip;
                  
                        using (System.Net.WebClient client = new System.Net.WebClient())
                        {
                            ip = client.DownloadString("http://dns.uniweb.se/checkip.php").Replace("Current IP Address: ", "");
                        }
                    
          

                return ip;

            }
        }

        public void getScript()
        {

            Type methodType = typeof(SLink.SlinkMethod);

            var slinkMethods = this.GetType().GetMethods().Where(n => ContainsAttribute<SLink.SlinkMethod>(n.GetCustomAttributes(true)));
            System.Text.StringBuilder startBit = new System.Text.StringBuilder();
            System.Text.StringBuilder endBit = new System.Text.StringBuilder();
            
           
            foreach (var method in slinkMethods)
            {
                var MethodName = method.Name;


                string returnType = method.ReturnType.Name.ToLower();

                if (!Inherits(typeof(SLink.SLType), method.ReturnType))
                {
                    throw new ApplicationException("unsupported return type");
                }


                var rawParameters = method.GetParameters().Select(n => GetSlType(n));

                var parameters = string.Join<string>(",", rawParameters.Select(n => string.Format(" {1} {0}", n.Key, n.Value)));

                List<string> parametersValues = new List<string>();
                parametersValues.Add(string.Format(@"""?action={0}", MethodName));

                parametersValues.AddRange(rawParameters.Select(n => string.Format(@"&{0}=""+ (string){0}", n.Key)));
         
         





             SLType theSlReturntype = (SLType)Activator.CreateInstance( method.ReturnType);





                var url = string.Format("{0}/{1}.ashx",Ip,  this.GetType().Name );
                startBit.AppendFormat(SlinkFormat.First,
                    MethodName, 
                    url, 
                    returnType, 
                    parameters,
                    string.Join("", parametersValues)
              
                    );



                endBit.AppendFormat(SlinkFormat.Second, MethodName, returnType,
                    theSlReturntype.ConvertMethod("body")
                    
                    );


            
            }

            var test = slinkMethods.Count();

            startBit.AppendFormat(SlinkFormat.SecondWrap, endBit.ToString());
            HttpContext.Current.Response.Write(startBit.ToString());
        }


        public bool IsReusable
        {
            get { throw new NotImplementedException(); }
        }

        public List<KeyValuePair<string,string>> GetStreamContent()
        {
            
          System.IO.TextReader reader = new System.IO.StreamReader(HttpContext.Current.Request.GetBufferedInputStream());



          var data_ = reader.ReadToEnd().Replace("?", "");
            var data2 = data_.Split(new char[] { '&' });
            List<KeyValuePair<string, string>> data3 = null;
            if (data_ != "")
            {

                data3 = data2.Select(n =>
                {
                    var temp = n.Split(new char[] { '=' });


                    return new KeyValuePair<string, string>(temp[0], temp[1]);

                }
                ).ToList();
            }

            return data3;


        }


        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";


            var body = GetStreamContent();

            var action = HttpContext.Current.Request.QueryString["action"] ?? body.First(n => n.Key == "action").Value  ;


            if (action != "__GETSCRIPT")
            {
           // var action = context.Request.Params["Action"];
            var actionMethod = this.GetType().GetMethods().First(n=> 
                
                  ContainsAttribute<SLink.SlinkMethod>( n.GetCustomAttributes(true))
                &&
                n.Name == action                
                );

         



            List<object> parametersList = new List<object>();
            foreach (var parameter in actionMethod.GetParameters())
            {
                var data = body.First(n => n.Key == parameter.Name).Value; 
               SLink.SLType type = Activator.CreateInstance(parameter.ParameterType) as SLink.SLType;
               parametersList.Add(type);
            }

            var returnobj = actionMethod.Invoke(this, parametersList.ToArray()) as SLink.SLType;

          

            if (returnobj != null)
            {
                HttpContext.Current.Response.Write(        
                    
                    returnobj.ToString()
                    
                    );
            }

            

            }
            else{
            getScript();
            }


        }
    }
}