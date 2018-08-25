using MediterraneoBack.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MediterraneoBack.Classes
{
    public class DBHelper
    {

        public static Response SaveChanges(MediterraneoContext db)
        {
            try
            {
                db.SaveChanges();
                return new Response { IsSucces = true, };
            }
            catch (Exception ex)
            {
                var response = new Response { IsSucces = false, };
                if (ex.InnerException != null &&
                    ex.InnerException.InnerException != null &&
                    ex.InnerException.InnerException.Message.Contains("_Index"))
                {
                    response.Message = "There is a record with the same value";
                }
                else if (ex.InnerException != null &&
                      ex.InnerException.InnerException != null &&
                      ex.InnerException.InnerException.Message.Contains("REFERENCE"))
                {
                    response.Message = "The record can't be delete because it has related records";
                }
                else
                {
                    response.Message = ex.Message;
                }

                return response;
            }
        }

        public static int GetState(string description, MediterraneoContext db)
        {
            var state = db.States.Where(s => s.Description == description).FirstOrDefault();
            if (state == null)
            {
                state = new State { Description = description, };
                db.States.Add(state);
                db.SaveChanges();
            }

            return state.StateId;
        }
    }
}