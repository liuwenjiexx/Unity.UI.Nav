namespace UnityEngine.UI.Navs.Routing
{

    internal class RouteParameter
    {

        public RouteParameter(string name)
        {
            this.Name = name;
        }
        public RouteParameter(string name, bool isOptional)
        {
            this.Name = name;
            this.IsOptional = isOptional;
        }

        public string Name { get; private set; }

        public bool IsOptional { get;  set; }

        public bool HasDefaultValue { get; set; }

        public int SeparatorIndex { get; set; }

        public object DefaultValue { get; set; }

    }

}
