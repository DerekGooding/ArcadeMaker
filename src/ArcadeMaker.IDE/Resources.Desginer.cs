namespace GameStudioEngine.Properties
{
    using System;

    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Resources
    {
        //private static global::System.Resources.ResourceManager resourceMan;

        //private static global::System.Globalization.CultureInfo resourceCulture;

        //[global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        //internal Resources() { }

        ///// <summary>
        /////   Returns the cached ResourceManager instance used by this class.
        ///// </summary>
        //[global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        //internal static global::System.Resources.ResourceManager ResourceManager
        //{
        //    get
        //    {
        //        if (object.ReferenceEquals(resourceMan, null))
        //        {
        //            global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("GameStudioEngine.Properties.Resources", typeof(Resources).Assembly);
        //            resourceMan = temp;
        //        }
        //        return resourceMan;
        //    }
        //}

        ///// <summary>
        /////   Overrides the current thread's CurrentUICulture property for all
        /////   resource lookups using this strongly typed resource class.
        ///// </summary>
        //[global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        //internal static global::System.Globalization.CultureInfo Culture
        //{
        //    get
        //    {
        //        return resourceCulture;
        //    }
        //    set
        //    {
        //        resourceCulture = value;
        //    }
        //}

        public static event EventHandler<ResourceRequestEventArgs> ResourceRequest;

        public static object GetObject(string name)
        {
            ResourceRequestEventArgs args = new ResourceRequestEventArgs(name);
            ResourceRequest?.Invoke(null, args);
            return args.resource; //?? ResourceManager.GetObject(name, resourceCulture);
        }
    }

    public class ResourceRequestEventArgs : EventArgs
    {
        public string name;
        public object resource;

        public ResourceRequestEventArgs(string name)
        {
            this.name = name;
        }
    }
}