using System;

namespace Assets.Code.Bon
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class GraphContextMenuItem : Attribute
    {
        private readonly string menuPath;
        public string Path { get { return menuPath; } }

        private readonly string itemName;
        public string Name { get { return itemName; } }

        public GraphContextMenuItem(string menuPath) : this(menuPath, null)
        {
        }

        public GraphContextMenuItem(string menuPath, string itemName)
        {
            this.menuPath = menuPath;
            this.itemName = itemName;
        }

    }
}
