using ContactsCore.Common.Enums;
using ContactsCore.Common.Interfaces;
using System.Collections.Generic;

namespace ContactsCore.Common
{
    public class ManagerResponse<TModel>
        where TModel : IBase
    {
        public List<TModel> Result { get; set; }
        public ManagerResponseResult ResultStatus { get; set; }
        public string ErrorMessage { get; set; }

        public PagingMetadata PageMeta { get; set; }
    }
}
