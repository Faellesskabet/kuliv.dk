using System;
using System.Threading.Tasks;
using Syncfusion.Blazor;
using Syncfusion.Blazor.Data;

namespace Dikubot.DataLayer.Database
{
    public class CustomAdaptor<TModel, TService> : DataAdaptor
        where TModel : MainModel where TService : Services<TModel>, new()
    {
        private Services<TModel> _service;

        public CustomAdaptor()
        {
            _service = new TService();
        }
        
        public CustomAdaptor(TService service)
        {
            _service = service;
        }

        public override async Task<object> ReadAsync(DataManagerRequest dataManagerRequest, string key = null)
        {
            //KULIV_null hmm
            return dataManagerRequest.RequiresCounts
                ? new DataResult() {Result = _service.GetAll(), Count = _service.GetAll().Count}
                : (object) _service.GetAll();
        }

        public async override Task<object> InsertAsync(DataManager dataManager, object data, string key)
        {

            return (object) _service.Upsert(data as TModel);
        }

        public async override Task<object> UpdateAsync(DataManager dataManager, object data, string keyField,
            string key)
        {
            return (object) _service.Upsert(data as TModel);
        }

        public async override Task<object> RemoveAsync(DataManager dataManager, object data, string keyField,
            string key) //triggers on appointment deletion through public method DeleteEvent
        {

            Guid value = (Guid) data;
            _service.Remove(_service.Get((model) => model.Id == value));
            return data;
        }



    }
}
    
