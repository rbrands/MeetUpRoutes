using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlazorApp.Shared
{
    /// <summary>
    /// User object including principal and contact-info
    /// </summary>
    public class User
    {
        public ClientPrincipal Principal {get; set;}
        public UserContactInfo ContactInfo { get; set; }
    }
}
