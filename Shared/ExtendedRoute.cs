using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace BlazorApp.Shared
{
    public class ExtendedRoute
    {
        public Route Core {get; set;}

        public ExtendedRoute()
        {

        }
        /// <summary>
        /// "Copy" constructor with instance of base class
        /// </summary>
        /// <param name="route"></param>
        public ExtendedRoute(Route route)
        {
            Core = route;
        }
    }
}
