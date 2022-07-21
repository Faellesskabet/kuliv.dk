
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dikubot.DataLayer.Database.Guild.Models.Calendar.Equipment;
using Dikubot.Extensions.Models.Equipment;
using Dikubot.Webapp.Authentication;
using Microsoft.AspNetCore.Http;
using Syncfusion.Blazor;
using Syncfusion.Blazor.Schedule;
using Syncfusion.Blazor.Data;
using Syncfusion.Blazor.Navigations;
using UserIdentity = Dikubot.Webapp.Authentication.UserIdentity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Authorization;

using Microsoft.AspNetCore.Identity;

namespace Dikubot.Extensions.Calendar.Equipment
{
    public class EquipmentAdaptor : DataAdaptor
     {
         
         private EquipmentServices services;
         private UserIdentity user;
         private Authenticator authenticator;
         
         public Authenticator AuthenticationStateProvider { get; set; }

         static int CurrentYear = DateTime.Today.Year;

         public EquipmentAdaptor()
         {
             services = new EquipmentServices("815997079850713130");
         }
         public EquipmentAdaptor(string guildID)
         {
             services = new EquipmentServices(guildID);
         }
         
         public async override Task<object> ReadAsync(DataManagerRequest dataManagerRequest, string key = null)
        {//KULIV_null hmm
            return dataManagerRequest.RequiresCounts ? new DataResult() { Result = services.GetAll(), Count = services.GetAll().Count } : (object)services.GetAll();
        }
        
        public async override Task<object> InsertAsync(DataManager dataManager, object data, string key)
        {
            
            return (object) services.Upsert(data as EquipmentModel);
        }

        public async override Task<object> UpdateAsync(DataManager dataManager, object data, string keyField, string key)
        {
            return (object) services.Upsert(data as EquipmentModel);
        }
        
        public async override Task<object> RemoveAsync(DataManager dataManager, object data, string keyField, string key) //triggers on appointment deletion through public method DeleteEvent
        {
            
            Guid value = (Guid)data;
            services.Remove(services.Get((AppointmentData) => AppointmentData.Id == value));
            return data;
        }
        
        public async override Task<object> BatchUpdateAsync(DataManager dataManager, object changedRecords, object addedRecords, object deletedRecords, string keyField, string key, int? dropIndex)
        {
            
            object records = deletedRecords;
            List<EquipmentModel> deleteData = deletedRecords as List<EquipmentModel>;
            foreach (var data in deleteData)
            {
                services.Remove(services.Get((AppointmentData) => AppointmentData.Id == data.Id));
            }
            List<EquipmentModel> addData = addedRecords as List<EquipmentModel>;
            foreach (var data in addData)
            {
                services.Upsert(data as EquipmentModel);
                records = addedRecords;
            }
            List<EquipmentModel> updateData = changedRecords as List<EquipmentModel>;
            foreach (var data in updateData)
            {
                var val = (data as EquipmentModel);
                var appointment = services.Get((AppointmentData) => AppointmentData.Id == val.Id);
                if (appointment != null)
                    if (appointment != null)
                    {
                        appointment.Id = val.Id;
                        appointment.Subject = val.Subject;
                        appointment.StartTime = val.StartTime;
                        appointment.EndTime = val.EndTime;
                        appointment.Location = val.Location;
                        appointment.Description = val.Description;
                        appointment.IsAllDay = val.IsAllDay;
                        appointment.RecurrenceException = val.RecurrenceException;
                        appointment.RecurrenceRule = val.RecurrenceRule;
                    }
                records = changedRecords;
            }
            return records;
        }

    }
}

