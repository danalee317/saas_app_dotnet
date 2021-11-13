namespace MultiFamilyPortal
{
    public static class FileTypeLookup
    {
        public static FileTypeInfo? GetFileTypeInfo(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLower();
            if(extension is null)
                return null;

            return _fileTypes.FirstOrDefault(x => x.Extension == extension);
        }

        private static readonly FileTypeInfo[] _fileTypes = new[]
        {
            new FileTypeInfo
            {
                Extension = ".pdf",
                Name = "PDF",
                Icon = "fa-regular fa-file-pdf",
                MimeType = "application/pdf"
            },
            new FileTypeInfo
            {
                Extension = ".xlsx",
                Name = "Excel",
                Icon = "fa-regular fa-file-excel",
                MimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            },
            new FileTypeInfo
            {
                Extension = ".xltx",
                Name = "Excel",
                Icon = "fa-regular fa-file-excel",
                MimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.template"
            },
            new FileTypeInfo
            {
                Extension = ".xlsm",
                Name = "Excel",
                Icon = "fa-regular fa-file-excel",
                MimeType = "application/vnd.ms-excel.sheet.macroEnabled.12"
            },
            new FileTypeInfo
            {
                Extension = ".xltm",
                Name = "Excel",
                Icon = "fa-regular fa-file-excel",
                MimeType = "application/vnd.ms-excel.template.macroEnabled.12"
            },
            new FileTypeInfo
            {
                Extension = ".xlam",
                Name = "Excel",
                Icon = "fa-regular fa-file-excel",
                MimeType = "application/vnd.ms-excel.addin.macroEnabled.12"
            },
            new FileTypeInfo
            {
                Extension = ".xlsb",
                Name = "Excel",
                Icon = "fa-regular fa-file-excel",
                MimeType = "application/vnd.ms-excel.sheet.binary.macroEnabled.12"
            },
            new FileTypeInfo
            {
                Extension = ".xls",
                Name = "Excel",
                Icon = "fa-regular fa-file-excel",
                MimeType = "application/vnd.ms-excel"
            },
            new FileTypeInfo
            {
                Extension = ".xlt",
                Name = "Excel",
                Icon = "fa-regular fa-file-excel",
                MimeType = "application/vnd.ms-excel"
            },
            new FileTypeInfo
            {
                Extension = ".xla",
                Name = "Excel",
                Icon = "fa-regular fa-file-excel",
                MimeType = "application/vnd.ms-excel"
            },


            new FileTypeInfo
            {
                Extension = ".doc",
                Name = "Word",
                Icon = "fa-regular fa-file-word",
                MimeType = "application/msword"
            },
            new FileTypeInfo
            {
                Extension = ".dot",
                Name = "Word",
                Icon = "fa-regular fa-file-word",
                MimeType = "application/msword"
            },
            new FileTypeInfo
            {
                Extension = ".docx",
                Name = "Word",
                Icon = "fa-regular fa-file-word",
                MimeType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
            },
            new FileTypeInfo
            {
                Extension = ".dotx",
                Name = "Word",
                Icon = "fa-regular fa-file-word",
                MimeType = "application/vnd.openxmlformats-officedocument.wordprocessingml.template"
            },
            new FileTypeInfo
            {
                Extension = ".docm",
                Name = "Word",
                Icon = "fa-regular fa-file-word",
                MimeType = "application/vnd.ms-word.document.macroEnabled.12"
            },
            new FileTypeInfo
            {
                Extension = ".dotm",
                Name = "Word",
                Icon = "fa-regular fa-file-word",
                MimeType = "application/vnd.ms-word.template.macroEnabled.12"
            },

            new FileTypeInfo
            {
                Extension = ".ico",
                Name = "Icon",
                Icon = "fa-regular fa-file-image",
                MimeType = "image/x-icon"
            },
            new FileTypeInfo
            {
                Extension = ".jpeg",
                Name = "JPEG",
                Icon = "fa-regular fa-file-image",
                MimeType = "image/jpeg"
            },
            new FileTypeInfo
            {
                Extension = ".jpg",
                Name = "JPEG",
                Icon = "fa-regular fa-file-image",
                MimeType = "image/jpeg"
            },
            new FileTypeInfo
            {
                Extension = ".png",
                Name = "PNG",
                Icon = "fa-regular fa-file-image",
                MimeType = "image/png"
            },
            new FileTypeInfo
            {
                Extension = ".svg",
                Name = "SVG",
                Icon = "fa-regular fa-file-image",
                MimeType = "image/svg+xml"
            }
        };
    }
}
