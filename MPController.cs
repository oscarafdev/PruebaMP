using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MercadoPago;
using MercadoPago.Resources;
using MercadoPago.DataStructures.Preference;

namespace WebApplication2.Controllers
{
    public class MPController : ApiController
    {
        // GET api/MercadoPago/GetInitPoint
        /*Devuelve un URL para iniciar el proceso de pago (se utiliza en web)*/
        [Route("api/MercadoPago/GetInitPoint")]
        public string GetInitPoint()
        {
            Preference preference = GetPreference(true);
            return preference.InitPoint;
        }

        // GET api/MercadoPago/GetPreferenceID 
        /*Devuelve un PreferenceID para iniciar el proceso de pago (se utiliza en android)*/
        [Route("api/MercadoPago/GetPreferenceID")]
        public string GetPreferenceID()
        {
            Preference preference = GetPreference(false);
            return preference.Id;
        }

        /*Si backURL es true agregará endpoints para traer la respuesta, esto es importante en la version WEB para obtener el paymentID.*/
        public Preference GetPreference(bool backURL)
        {
            try
            {
                MercadoPago.SDK.AccessToken = "TEST-2923378732896017-042314-8dd026589610e931b4de5459ea08b9b5-547387423";
            }
            catch
            {

            }
            Preference preference = new Preference();
            preference.Payer = new Payer()
            {
                Name = "Oscar",
                Surname = "Fernandez",
                Email = "oscarafdev@gmail.com",
                Phone = new Phone()
                {
                    AreaCode = "011",
                    Number = "39508719"
                },

                Identification = new Identification()
                {
                    Type = "DNI",
                    Number = "46267858"
                },

                Address = new Address()
                {
                    StreetName = "Pedro F. Uriarte",
                    StreetNumber = int.Parse("176"),
                    ZipCode = "1663"
                }
            };

            // Crea un ítem en la preferencia
            preference.Items.Add(
              new Item()
              {
                  Title = "Mi producto",
                  Quantity = 1,
                  CurrencyId = MercadoPago.Common.CurrencyId.ARS,
                  UnitPrice = (decimal)100.00
              }
            );
            if(backURL)
            {
                preference.BackUrls = new BackUrls()
                {
                    Success = "http://localhost:8000/mp/result",
                    Failure = "http://localhost:8000/mp/result",
                    Pending = "http://localhost:8000/mp/result"
                };
            }
            preference.Save();
            return preference;
        }
        // GET api/MercadoPago/Refund
        /*
         * El PaymentID lo trae al realizar el pago
         * En android lo obtenemos en onActivityResult
         * En web se obtiene con los BackUrl con el metodo GET.
         * Este PaymentID debería guardarse en el servidor, es de suma importancia para el refund.
         */
        [Route("api/MercadoPago/Refund")]
        public string Refund(long PaymentID)
        {
            Payment payment = Payment.FindById(PaymentID);
            payment.Refund();
            return "exito";
        }
    }
}