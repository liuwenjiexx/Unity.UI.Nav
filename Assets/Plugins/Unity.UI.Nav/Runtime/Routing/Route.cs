using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace UnityEngine.UI.Navs.Routing
{

    /// <summary>
    /// Url 路由
    /// 路由参考：https://docs.microsoft.com/zh-cn/previous-versions/aspnet/cc668201(v=vs.100)
    /// </summary>
    public class Route
    {
        private Regex patternRegex;
        private string pattern;
        private List<RouteParameter> parameters;
        private Dictionary<string, object> defaults;
        private Dictionary<string, string> constraints;
        private Dictionary<string, object> metadata;

        /// <summary>
        /// 参数为可选的
        /// </summary>
        public static readonly object Optional = Type.Missing;

        public Route(string pattern, IRouteHandler routeHandler)
            : this(pattern, null, null, null, routeHandler)
        {
        }

        public Route(string pattern, Dictionary<string, object> defaults, IRouteHandler routeHandler)
                    : this(pattern, defaults, null, null, routeHandler)
        {
        }

        public Route(string pattern, object defaults, IRouteHandler routeHandler)
                        : this(pattern, ObjectToDictionary(defaults), null, null, routeHandler)
        {
        }

        public Route(string pattern, Dictionary<string, object> defaults, Dictionary<string, string> constraints, Dictionary<string, object> metadata, IRouteHandler routeHandler)
        {
            if (pattern == null)
                throw new ArgumentNullException(nameof(pattern));
            if (routeHandler == null)
                throw new ArgumentNullException(nameof(routeHandler));
            this.parameters = new List<RouteParameter>();
            this.defaults = new Dictionary<string, object>();
            this.constraints = new Dictionary<string, string>();
            this.metadata = new Dictionary<string, object>();

            if (defaults != null)
            {
                foreach (var item in defaults)
                    this.defaults[item.Key] = item.Value;
            }
            if (constraints != null)
            {
                foreach (var item in constraints)
                    this.constraints[item.Key] = item.Value;
            }
            if (metadata != null)
            {
                foreach (var item in metadata)
                    this.metadata[item.Key] = item.Value;
            }
            this.RouteHandler = routeHandler;
            this.Pattern = pattern;
        }

        /// <summary>
        ///  URL 模式
        /// </summary>
        /// <example>
        /// {controller}/{action}/{id}                  /Products/show/beverages
        /// {table}/Details.aspx                         /Products/Details.aspx
        /// blog/{action}/{entry}                       /blog/show/123
        /// {reporttype}/{year}/{month}/{day}  /sales/2008/1/5
        /// {locale}/{action}                               /en-US/show
        /// {language}-{country}/{action}         /en-US/show
        /// </example>
        public string Pattern
        {
            get => pattern;
            set
            {
                this.pattern = value;
                parameters.Clear();
                int separatorIndex = -1;
                bool hasOptional = false;
                string pattern = this.pattern;
                if (!pattern.StartsWith("/"))
                    pattern = "/" + pattern;
                pattern = PatternParamRegex.Replace(pattern, m =>
               {
                   if (m.Groups["separator"].Success)
                   {
                       separatorIndex++;
                       int index = separatorIndex;
                       return $"(?<separ_{index}>/)?";
                   }

                   string paramName = m.Groups["name"].Value;
                   var param = new RouteParameter(paramName);
                   var gDefault = m.Groups["default"];
                   if (gDefault.Success)
                   {
                       param.HasDefaultValue = true;
                       param.DefaultValue = gDefault.Value;
                   }
                   else if (m.Groups["optional"].Success)
                   {
                       param.IsOptional = true;
                   }
                   param.SeparatorIndex = separatorIndex;
                   if (param.IsOptional)
                       hasOptional = true;
                   else
                   {
                       if (hasOptional)
                           throw new Exception($"not optional param can't after optional param <{paramName}>");
                   }
                   parameters.Add(param);
                   return $"(?<{paramName}>[^/]+)?";
               });

                pattern = "^" + pattern + "$";
                patternRegex = new Regex(pattern, RegexOptions.IgnoreCase);

            }
        }

        static Regex PatternParamRegex = new Regex("\\{\\s*(?<name>[^\\}]+?)((=(?<default>[^\\}]*?))|(?<optional>\\?))?\\}|(?<separator>/)");



        /// <summary>
        /// 参数名称对应的默认值
        /// </summary>
        public Dictionary<string, object> Defaults { get => defaults; }

        /// <summary>
        /// 参数名称对应的约束
        /// </summary>
        public Dictionary<string, string> Constraints { get => constraints; }

        /// <summary>
        /// 路由处理对象
        /// </summary>
        public IRouteHandler RouteHandler { get; private set; }


        public RouteData GetRouteData(string url, object parameter = null)
        {
            string path;

            if (url.IndexOf(':') >= 0)
            {
                Uri uri = new Uri(url);
                path = uri.LocalPath;
            }
            else
            {
                if (!url.StartsWith("/"))
                    path = "/" + url;
                else
                    path = url;
            }
            var m = patternRegex.Match(path);
            if (!m.Success)
                return null;

            //validate pattern

            for (int i = 0, j = -1; i < parameters.Count; i++)
            {
                RouteParameter param = parameters[i];
                var g = m.Groups[param.Name];
                if (!g.Success)
                {
                    if (!(param.IsOptional || param.HasDefaultValue))
                        return null;
                }
                else
                {
                    if (j < param.SeparatorIndex)
                    {
                        while (j < param.SeparatorIndex)
                        {
                            if (m.Groups["separ_" + (j + 1)].Success)
                            {
                                j++;
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }
                }
            }

            RouteData routeData = new RouteData();



            for (int i = 0; i < parameters.Count; i++)
            {
                RouteParameter param = parameters[i];
                var g = m.Groups[param.Name];
                if (g.Success)
                {
                    routeData.Values[param.Name] = g.Value;
                }
                else
                {
                    if (param.HasDefaultValue)
                    {
                        routeData.Values[param.Name] = param.DefaultValue;
                    }
                }
            }

            foreach (var item in defaults)
            {
                if (!routeData.Values.ContainsKey(item.Key))
                    routeData.Values[item.Key] = item.Value;
            }

            if (parameter != null)
            {
                IDictionary<string, object> dic = parameter as IDictionary<string, object>;
                if (dic == null)
                {
                    dic = ObjectToDictionary(parameter);
                }

                foreach (var item in dic)
                    routeData.Values[item.Key] = item.Value;
            }

            routeData.RouteHandler = RouteHandler;
            return routeData;
        }

        public static Dictionary<string, object> ObjectToDictionary(object value)
        {
            /* foreach (var pInfo in parameter.GetType().GetProperties())
                {
                    object value = pInfo.GetValue(parameter, null);
                    routeData.Values[pInfo.Name] = value;
                }*/
            if (value == null)
                return new Dictionary<string, object>();
            var dic = value as Dictionary<string, object>;
            if (dic != null)
                return dic;
            return TypeDescriptor.GetProperties(value)
                .OfType<PropertyDescriptor>()
                .ToDictionary(o => o.Name, o => o.GetValue(value));
        }

    }





}