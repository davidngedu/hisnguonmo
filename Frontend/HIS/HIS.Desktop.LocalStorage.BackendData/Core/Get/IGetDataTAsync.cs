﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.LocalStorage.BackendData
{
    interface IGetDataTAsync
    {
        Task<object> ExecuteAsync<T>();
    }
}
