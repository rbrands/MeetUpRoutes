using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace BlazorApp.Shared
{
    public class ExtendedComment
    {
        public Comment Core { get; set; }
        public UserContactInfo Author { get; set; }
        public string AuthorDisplayName { get; set; }
        public ExtendedComment()
        {
            Core = new Comment();
        }
        public ExtendedComment(Comment comment)
        {
            Core = comment;
        }

    }
}
