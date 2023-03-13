using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Claims;
using WayfJwtConnector.Models;

namespace Dikubot.Webapp.Authentication.Identities;

public sealed class WayfIdentity : ClaimsIdentity
{

    private WayfClaims _wayfClaims;
    public WayfIdentity(WayfClaims wayfClaims)
    {
        _wayfClaims = wayfClaims;
        List<Claim> claims = new List<Claim>();
        foreach (PropertyInfo property in typeof(WayfClaims).GetProperties())
        {
            object? value = property.GetValue(wayfClaims);
            if (value == null)
            {
                continue;
            }

            string stringVal = value.ToString();
            if (string.IsNullOrEmpty(stringVal))
            {
                continue;
            }
            // Debugging on production yeehaw
            Console.WriteLine(stringVal);
            claims.Add(new Claim($"wayf:{property.Name}", stringVal));
        }
        AddClaims(claims);
    }
    
    public string AuthenticationType => "WayfUser";

}