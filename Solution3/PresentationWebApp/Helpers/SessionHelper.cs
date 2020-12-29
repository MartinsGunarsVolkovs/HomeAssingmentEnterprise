using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PresentationWebApp.Helpers
{

    //All of the cookie related implementations are inspired by https://www.youtube.com/watch?v=C2FX_37XBqM&t=500s&ab_channel=KaushikRoyChowdhury, thus some code could be similar to the code of the video
    public static class SessionHelper
    {

        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            //Serializes objects with the particular "key" string
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonConvert.DeserializeObject<T>(value);
        }

    }
}
