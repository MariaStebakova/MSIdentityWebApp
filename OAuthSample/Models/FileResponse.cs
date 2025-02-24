using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OAuthSample.Models
{
    public class FileResponse
    {
        public int Id { get; set; }
        public int FileTypeId { get; set; }
        public string FileTypeName { get; set; }
        public string Description { get; set; }
        public string FileNumberPart1 { get; set; }
        public string FileNumberPart2 { get; set; }
        public string FileNumberPart3 { get; set; }
        public DateTime DateCreated { get; set; }
        public List<FileAttribute> Attributes { get; set; }
    }

    public class FileAttribute
    {
        public string DisplayName { get; set; }
        public string Name { get; set; }
        public object Value { get; set; }
        public int AttributeType { get; set; }
    }
}