using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShareX.UploadersLib
{
    public interface IUploaderService
    {
        bool CheckConfig(UploadersConfig config);

        string ServiceName { get; }
    }
}